﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Regulus.Utility;

namespace Regulus.BehaviourTree
{
    class ActionNode<T> : ITicker , IDeltaTimeRequester
    {
        
        private float _Delta;
        

        private IEnumerator<TICKRESULT> _Iterator;

        private Action _Start;

        private Func<float, TICKRESULT> _Tick;

        private Action _End;

        enum STATUS { NONE ,START , RUNNING , END}

        private STATUS _Status;

        private Expression<Func<T>> _InstanceProvider;

        private Expression<Func<T, Func<float, TICKRESULT>>> _TickExpression;

        private Expression<Func<T, Action>> _StartExpression;

        private Expression<Func<T, Action>> _EndExpression;

        private bool _LazyInitial;

        public ActionNode(Expression<Func<T>> instance_provider
            , Expression< Func<T,Func<float , TICKRESULT> > > tick
            , Expression<Func<T, Action >> start
            , Expression<Func<T, Action>> end
             )
        {

            _InstanceProvider = instance_provider;
            _TickExpression = tick;
            _StartExpression = start;
            _EndExpression = end;

            

            _Reset();
        }

        IEnumerable<TICKRESULT> _GetIterator()
        {
            if (_LazyInitial == false)
            {
                var instance = _InstanceProvider.Compile()();
                _Start = _StartExpression.Compile()(instance);
                _Tick = _TickExpression.Compile()(instance);
                _End = _EndExpression.Compile()(instance);
                _LazyInitial = true;

            }
            
            while (true)
            {
                _Start();
                _Status = STATUS.START;
                TICKRESULT result;

                do
                {
                    result = _Tick(_RequestDelta());
                    _Status = STATUS.RUNNING;
                    if (result == TICKRESULT.RUNNING)
                        yield return result;
                }
                while (result == TICKRESULT.RUNNING);

                _End();
                yield return result;
                _Status = STATUS.END;
            }
            
            
        }

        void ITicker.Reset()
        {
            if (_Status != STATUS.NONE || _Status != STATUS.END ) 
            {
                _End();
            }

            _Reset();
        }

        private void _Reset()
        {
            _Status = STATUS.NONE;
            _Iterator = _GetIterator().GetEnumerator();            
        }

        TICKRESULT ITicker.Tick(float delta)
        {
            _Delta += delta;

            _Iterator.MoveNext();
            var result =  _Iterator.Current;            
            return result;
        }

        float IDeltaTimeRequester.Request()
        {
            return _RequestDelta();
        }

        private float _RequestDelta()
        {
            var d = _Delta;
            _Delta = 0f;
            return d;
        }
    }
}
