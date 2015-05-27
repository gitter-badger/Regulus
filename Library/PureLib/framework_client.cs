﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework
{
    public class Client<TUser> : Regulus.Utility.IUpdatable
       where TUser : class, Regulus.Utility.IUpdatable
    {
        Regulus.Utility.StageMachine _Machine;

        Regulus.Utility.Console _Console;
        Regulus.Utility.Command _Command { get { return _Console.Command; } }
        public Regulus.Utility.Command Command { get { return _Command; } }
        Regulus.Utility.Console.IViewer _View;        

        public delegate void OnModeSelector(Regulus.Framework.GameModeSelector<TUser> selector);
        public event OnModeSelector ModeSelectorEvent;
        
        public bool Enable { get; private set; }
        
        public Client(Regulus.Utility.Console.IViewer view , Regulus.Utility.Console.IInput input ) 
        {
            Enable = true;
            _Machine = new Regulus.Utility.StageMachine();

            _View = view;
            _Console = new Utility.Console(input, view);
            Selector = new Regulus.Framework.GameModeSelector<TUser>(_Command, _View);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Machine.Update();
            return Enable;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            _Command.Register("Quit" , _ToShutdown );
            _ToSelectMode();
        }

        private void _ToSelectMode()
        {
            
            var stage = new Stage.SelectMode<TUser>(Selector, _Command);
            stage.DoneEvent += _ToOnBoard;
            stage.InitialedEvent += () =>
            {
                if (ModeSelectorEvent != null)
                    ModeSelectorEvent(Selector);
            };
            _Machine.Push(stage);

        }

        private void _ToOnBoard(Regulus.Framework.UserProvider<TUser> user_provider)
        {
            
            _View.WriteLine("Onboard ready.");
            var stage = new Stage.OnBoard<TUser>(user_provider , _Command);
            stage.DoneEvent += _ToShutdown;
            _Machine.Push(stage);
        }

        private void _ToShutdown()
        {
            Enable = false;
            _Machine.Termination();
            _Command.Unregister("Quit");
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _ToShutdown();
        }



        public GameModeSelector<TUser> Selector { get; private set; }
    }


    namespace Extension
    {
        public static class ClientExtension
        {
            public static void Run(this Regulus.Utility.IUpdatable updatable)
            {

            }
        }
    }
}
