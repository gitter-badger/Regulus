﻿using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

using Regulus.Extension;

using Regulus.Remoting;

namespace VGame.Project.FishHunter.Storage
{
    public class Server : Regulus.Remoting.ICore, IStorage
    {

        Regulus.Utility.Updater _Updater;
        VGame.Project.FishHunter.Storage.Center _Center;
        Regulus.Remoting.ICore _Core { get { return _Center; } }
        Regulus.NoSQL.Database _Database;
        private Regulus.Utility.LogFileRecorder _LogRecorder;
        private string _Ip;
        private string _Name;
        private string _DefaultAdministratorName;

        public Server()
        {
            _LogRecorder = new Regulus.Utility.LogFileRecorder("Storage");
            _DefaultAdministratorName = "vgameadmini";
            _Ip = "mongodb://127.0.0.1:27017";
            _Name = "VGame";
            _Updater = new Regulus.Utility.Updater();
            _Database = new Regulus.NoSQL.Database(_Ip);
            _Center = new Center(this);
        }

        void Regulus.Remoting.ICore.AssignBinder(Regulus.Remoting.ISoulBinder binder)
        {
            _Core.AssignBinder(binder);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            Regulus.Utility.Log.Instance.RecordEvent += _LogRecorder.Record;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _Updater.Add(_Center);
            _Database.Launch(_Name);

            _HandleAdministrator();
            
            _HandleGuest();

            //_HandleTradeRecord();
            
        }

        

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            _LogRecorder.Record(ex.ToString());
            _LogRecorder.Save();
        }

        private async void _HandleAdministrator()
        {
            var accounts = await _Database.Find<Data.Account>(a => a.Name == _DefaultAdministratorName);

            if (accounts.Count == 0)
            {
                var account = new Data.Account()
                {
                    Id = Guid.NewGuid(),
                    Name = _DefaultAdministratorName,
                    Password = "vgame",
                    Competnces = Data.Account.AllCompetnce()
                };

                await _Database.Add(account);
            }
        }

        async private void _HandleGuest()
        {
            var accounts = await _Database.Find<Data.Account>(a => a.Name == "Guest");

            if (accounts.Count == 0)
            {
                var account = new Data.Account()
                {
                    Id = Guid.NewGuid(),
                    Name = "Guest",
                    Password = "vgame",
                    Competnces = new Regulus.CustomType.Flag<Data.Account.COMPETENCE>(Data.Account.COMPETENCE.FORMULA_QUERYER)
                };
                await _Database.Add(account);
            }
        }

        private void _HandleTradeRecord()
        {
            //_QueryAllAccount().OnValue += ((accounts) =>
            //{
            //    foreach (var acc in accounts)
            //    {
            //        var trades = _TradeCorder.FindHistory(acc.Id);


                  
            //    }
            //);

            
        }

        

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Database.Shutdown();
            _Updater.Shutdown();

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            Regulus.Utility.Log.Instance.RecordEvent -= _LogRecorder.Record;
        }

        Regulus.Remoting.Value<Data.Account> IAccountFinder.FindAccountByName(string name)
        {
            var account = _Find(name);

            if (account != null)
                return account;

            return new Regulus.Remoting.Value<Data.Account>(null);
        }

        private Data.Account _Find(string name)
        {
            var task = _Database.Find<Data.Account>(a => a.Name == name);
            task.Wait();
            return task.Result.FirstOrDefault();
        }
        private Data.Account _Find(Guid id)
        {
            var task = _Database.Find<Data.Account>(a =>a.Id == id);
            task.Wait();
            return task.Result.FirstOrDefault();
        }

        Regulus.Remoting.Value<ACCOUNT_REQUEST_RESULT> VGame.Project.FishHunter.IAccountManager.Create(Data.Account account)
        {
            var result = _Find(account.Name);
            if(result != null)
            {
                return ACCOUNT_REQUEST_RESULT.REPEAT;
            }
            _Database.Add(account);
            return ACCOUNT_REQUEST_RESULT.OK;
        }

        Regulus.Remoting.Value<ACCOUNT_REQUEST_RESULT> VGame.Project.FishHunter.IAccountManager.Delete(string account)
        {
            var result = _Find(account);
            if (result != null && _Database.Remove<Data.Account>(a => a.Id == result.Id))
            {                
                return ACCOUNT_REQUEST_RESULT.OK;
            }

            return ACCOUNT_REQUEST_RESULT.NOTFOUND;
        }

        Regulus.Remoting.Value<Data.Account[]> _QueryAllAccount()
        {
            var val = new Regulus.Remoting.Value<Data.Account[]>();
            var t = _Database.Find<Data.Account>(a => true);
            t.ContinueWith(list => { val.SetValue(list.Result.ToArray()); });
            return val;            
        }
        Regulus.Remoting.Value<Data.Account[]> IAccountManager.QueryAllAccount()
        {
            return _QueryAllAccount();
        }


        Regulus.Remoting.Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Update(Data.Account account)
        {
            if (_Database.Update(account, a => a.Id == account.Id))            
                return ACCOUNT_REQUEST_RESULT.OK;
            return ACCOUNT_REQUEST_RESULT.NOTFOUND;
        }


        Regulus.Remoting.Value<Data.Account> IAccountFinder.FindAccountById(Guid accountId)
        {
            return _Find(accountId);
        }

        Regulus.Remoting.Value<Data.Record> IRecordQueriers.Load(Guid id)
        {
            var val = new Regulus.Remoting.Value<Data.Record>();
            var account = _Find(id);
            if(account.IsPlayer())
            {
                var recordTask = _Database.Find<Data.Record>(r => r.Owner == id);
                recordTask.ContinueWith((task) =>
                {
                   
                    if(task.Result.Count > 0)
                    {
                        val.SetValue(task.Result.FirstOrDefault());
                    }
                    else
                    {
                        var newRecord = new Data.Record() { Owner = id, Money = 100 };
                        _Database.Add(newRecord).Wait();
                        val.SetValue(newRecord);
                    }
                    
                });
            }
            else
            {
                val.SetValue(null);
            }
            return val;
        }

        void IRecordQueriers.Save(Data.Record record)
        {
            _Database.Update<Data.Record>(record, r => r.Id == record.Id);
        }

        Regulus.Remoting.Value<TradeNotes> ITradeAccount.Find(Guid id)
        {
            return _LoadTradeNotes(id);
        }

        Regulus.Remoting.Value<TradeNotes> ITradeAccount.Load(Guid id)
        {
            return _LoadTradeNotes(id);
        }

        private Regulus.Remoting.Value<TradeNotes> _LoadTradeNotes(Guid id)
        {
            var val = new Regulus.Remoting.Value<TradeNotes>();
            var account = _Find(id);
            if (account.IsPlayer())
            {
                var tradeTask = _Database.Find<TradeNotes>(t => t.OwnerId == id);
                tradeTask.ContinueWith((task) =>
                {
                    Regulus.Utility.Log.Instance.Write(string.Format("TradeNotes Find Done."));
                    if(task.Exception != null)
                    {
                        Regulus.Utility.Log.Instance.Write(string.Format("TradeNotes Exception {0}.", task.Exception.ToString()));
                    }
                    
                    if (task.Result.Count > 0)
                    {
                        val.SetValue(task.Result.FirstOrDefault());

                        Regulus.Utility.Log.Instance.Write(string.Format("have TradeNotes . id = {0}" , id));
                    }
                    else
                    {
                        var newPlayerNotes = new TradeNotes(id);
                        _Database.Add(newPlayerNotes).Wait();
                        val.SetValue(newPlayerNotes);
                        Regulus.Utility.Log.Instance.Write(string.Format("new TradeNotes . id = {0}", id));

                    }
                });
            }
            else
            {
                val.SetValue(null);
            }
            return val;
        }


        Regulus.Remoting.Value<Data.TradeData> ITradeAccount.Saving(Data.TradeData data)
        {
           var notes = _LoadTradeNotes(data.BuyerId).Result();
           notes.TradeData.Add(data);

            _Database.Update<TradeNotes>(notes, a => notes.OwnerId == a.OwnerId);

            return null;
        }
    }
}
