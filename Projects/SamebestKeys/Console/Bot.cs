﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class Bot : Regulus.Utility.IUpdatable
    {
        //const string IpAddress = "192.168.40.133";
        //const string IpAddress = "114.34.90.217";
        //const string IpAddress = "127.0.0.1";
        //const string IpAddress = "60.250.141.90";
        //const string IpAddress = "23.97.70.8";

        //public static string IpAddress = "60.250.141.88";
        public static string IpAddress = "192.168.120.38";
        //public static string IpAddress = "127.0.0.1";
        public static int Port = 12345;

        private Regulus.Project.SamebestKeys.IUser _User;        
        Regulus.Utility.StageMachine _Machine;
        
        string _Account;
        public Bot(Regulus.Project.SamebestKeys.IUser user,string account)
        {
            _Machine = new Regulus.Utility.StageMachine();
            this._User = user;
            _Account = account;
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Machine.Update();
            return true;
        }
        void Regulus.Framework.ILaunched.Shutdown()
        {
            _User.OnlineProvider.Supply -= OnlineProvider_Supply;
        }
        void Regulus.Framework.ILaunched.Launch()
        {
            _ToConnect(IpAddress, Port);
            _User.OnlineProvider.Supply += OnlineProvider_Supply;
        }

        Action _OnlineOnDisconnect;
        void OnlineProvider_Supply(Regulus.Project.SamebestKeys.IOnline obj)
        {
            _OnlineOnDisconnect = () => 
            {
                obj.DisconnectEvent -= _OnlineOnDisconnect;
                _ToConnect(IpAddress, Port);
            };
            obj.DisconnectEvent += _OnlineOnDisconnect;
        }

        

        private void _ToConnect(string ip, int port)
        {
            var stage = new BotConnectStage(_User , ip ,port );
            stage.ResultEvent += (result) =>
            {
                if (result)
                {
                    _ToVerify(_Account);
                }
                else
                {
                    _ToConnect(IpAddress, Port);
                }
            };
            _Machine.Push(stage);
        }

        private void _ToVerify(string account)
        {
            var stage = new BotVerifyStage(_User , account );
            stage.ResultEvent += (result) =>
            {                
                if (result == Regulus.Project.SamebestKeys.LoginResult.Success)
                {
                    _ToSelectActor(account);
                }
                else if (result == Regulus.Project.SamebestKeys.LoginResult.Error)
                {
                    _ToCreateAccount(account);
                }
                else if (result == Regulus.Project.SamebestKeys.LoginResult.RepeatLogin)
                {
                    _ToVerify(account);
                }
            };
            _Machine.Push(stage);
        }

        private void _ToCreateAccount(string account)
        {
            var stage = new BotCreateAccountStage(_User, account);
            stage.ResultEvent += (result) =>
            {
                if (result)
                {
                    _ToVerify(account);
                }
                else
                {
                    throw new SystemException("建立不存在的帳號");
                }
            };
            _Machine.Push(stage);            
        }

        private void _ToSelectActor(string account)
        {
            var stage = new BotSelectActorStage(_User , account );
            stage.ResultEvent += (result) =>
            {
                if (result != string.Empty)
                {
                    _ToRoom();
                }
                else
                {
                    _ToCreateActor(account);
                }
            };
            _Machine.Push(stage);            
        }

        private void _ToRoom()
        {
            var stage = new BotRoomStage(_User);
            stage.DoneEvent += () =>
            {
                _ToMap();
            };            

            _Machine.Push(stage);            
        }

        private void _ToCreateActor(string account)
        {
            var stage = new BotCreateActorStage(_User, account);
            stage.ResultEvent += (result) =>
            {
                if (result)
                {
                    _ToSelectActor(account);
                }
                else
                {
                    throw new SystemException("建立不存在的人物");
                }

            };

            _Machine.Push(stage);            
        }
        private void _ToMap()
        {
            var stage = new BotMapStage(_User);
            stage.ResultResetEvent += (point) =>
            {
                _ToMap(point);
            };
            stage.ResultConnectEvent += () =>
            {
                _ToConnect(IpAddress, Port);
            };

            _Machine.Push(stage);
        }
        private void _ToMap(Regulus.CustomType.Point born_point)
        {
            var stage = new BotMapStage(_User, born_point);
            stage.ResultResetEvent += (point) => 
            {
                _ToMap(point);
            };
            stage.ResultConnectEvent += () =>
            {
                _ToConnect(IpAddress, Port);
            };
            
            _Machine.Push(stage);            
        }
        
    }
}
