﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts
{
    internal class User : IUser
    {
        Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IConnect> _ConnectProvider;
        Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IOnline> _OnlineProvider;        
        Regulus.Utility.StageMachine _Machine;
        Regulus.Remoting.IAgent _Agent;

        Regulus.Utility.Updater _Updater;

        public User(Regulus.Remoting.IAgent agent)
        {
            _Agent = agent;
            _ConnectProvider = new Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IConnect>();
            _OnlineProvider = new Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IOnline>();
            _Machine = new Regulus.Utility.StageMachine();
            _Updater = new Regulus.Utility.Updater();
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Update();
            _Machine.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Updater.Add(_Agent);
            _ToOffline();
        }

        private void _ToOffline()
        {
            var stage = new OfflineStage(_Agent , _ConnectProvider);

            stage.DoneEvent += _ToOnline;

            _Machine.Push(stage);
        }

        private void _ToOnline()
        {
            var stage = new OnlineStage(_Agent, _OnlineProvider);

            stage.BreakEvent += _ToOffline;
            
            _Machine.Push(stage);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Machine.Termination();
            _Updater.Shutdown();
        }

        Regulus.Remoting.Ghost.IProviderNotice<Regulus.Utility.IConnect> IUser.ConnectProvider
        {
            get { return _ConnectProvider; }
        }

        Regulus.Remoting.Ghost.IProviderNotice<Regulus.Utility.IOnline> IUser.OnlineProvider
        {
            get { return _OnlineProvider; }
        }
    }
}
