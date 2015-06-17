﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public interface IOnline
    {        
        double Ping { get; }
        void Disconnect();

        event Action DisconnectEvent;
    }


    public class Online : IOnline, Regulus.Remoting.Ghost.IGhost
    {
        Guid _Id;

        public Guid Id { get { return _Id;  } }
        public delegate void DisconnectCallback();
        public event DisconnectCallback DisconnectEvent;
        public Online()
        {
            _Id = Guid.NewGuid();
        }

        public Online(Remoting.IAgent agent) : this()
        {            
            this._Agent = agent;
            
        }
        double IOnline.Ping
        {
            get { return System.TimeSpan.FromTicks(_Agent.Ping).TotalSeconds; }
        }

        void IOnline.Disconnect()
        {
            DisconnectEvent();       
        }

        void Regulus.Remoting.Ghost.IGhost.OnEvent(string name_event, object[] args)
        {
            throw new NotImplementedException();
        }

        Guid Regulus.Remoting.Ghost.IGhost.GetID()
        {
            return _Id;
        }

        void Regulus.Remoting.Ghost.IGhost.OnProperty(string name, byte[] value)
        {
            throw new NotImplementedException();
        }
        
        private Remoting.IAgent _Agent;
        event Action IOnline.DisconnectEvent
        {
            add { _Agent.DisconnectEvent += value; }
            remove { _Agent.DisconnectEvent -= value; }
        }






        bool Remoting.Ghost.IGhost.IsReturnType()
        {
            return false;
        }
    }


}
