﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting.Soul.Native
{
    

	class Peer : Regulus.Remoting.IRequestQueue, Regulus.Remoting.IResponseQueue , Regulus.Utility.IUpdatable
	{

        public static bool IsIdle { get { return TotalRequest <= 0 && TotalResponse <= 0; } }
        class Request
        {
            public Guid EntityId { get; set; }
            public string MethodName { get; set; }
            public Guid ReturnId { get; set; }
            public byte[][] MethodParams { get; set; }
        }

		System.Net.Sockets.Socket _Socket;
		Regulus.Remoting.Soul.SoulProvider _SoulProvider;
		PackageQueue _Responses;
        PackageQueue _Requests;


        Regulus.Remoting.Native.PackageReader _Reader;
        Regulus.Remoting.Native.PackageWriter _Writer;
		Regulus.Utility.StageMachine _ReadMachine;
		Regulus.Utility.StageMachine _WriteMachine;
        volatile bool _Enable;


        static object _LockRequest = new object();        
        public static int TotalRequest { get; private set; }

        static object _LockResponse = new object();        
        public static int TotalResponse { get; private set; }
        public Peer(System.Net.Sockets.Socket client)
		{
            
			_Socket = client;
			_SoulProvider = new Remoting.Soul.SoulProvider(this, this);
            _Responses = new PackageQueue();
            _Requests = new PackageQueue();
			_ReadMachine = new Utility.StageMachine();
			_WriteMachine = new Utility.StageMachine();
            _Enable = true;

            _Reader = new Remoting.Native.PackageReader();
            _Writer = new Remoting.Native.PackageWriter();
			
		}
		private void _HandleWrite(Package[] packages)
		{
            var pkgs = packages;
            var responseCount = pkgs.Length;
            lock (_LockResponse)
            {
                TotalResponse -= responseCount;
            }

            var stage = new NetworkStreamWriteStage(_Socket, pkgs);                
            stage.WriteCompletionEvent += () =>
            {
                _HandleWriteWait();
            };
            stage.ErrorEvent += () => 
            {
                _Enable = false; 
            };
            _WriteMachine.Push(stage);
			
		}

        void _HandleWriteWait()
        {
            var stage = new WaitQueueStage(_Responses);
            stage.DoneEvent += _HandleWrite;
            _WriteMachine.Push(stage);
        }
		private void _HandleRead()
		{
			var stage = new NetworkStreamReadStage(_Socket);
			stage.ReadCompletionEvent += (package) =>
			{
                if (package != null)
                {

                    _RequestPush(package);
                        
                }				    
				_HandleRead();
			};
            stage.ErrorEvent += () => { _Enable = false; };
			_ReadMachine.Push(stage);
		}

        private void _RequestPush(Package package)
        {
            lock (_LockRequest)
            {
                _Requests.Enqueue(package);
                TotalRequest++;
            }
        }

        private Request _TryGetRequest(Package package )
		{
			if (package.Code == (byte)ClientToServerOpCode.Ping)
			{
				
				(this as Regulus.Remoting.IResponseQueue).Push((int)ServerToClientOpCode.Ping, new Dictionary<byte, byte[]>());
                return null;
			}
            else if (package.Code == (byte)ClientToServerOpCode.CallMethod)
            {

                var entityId = new Guid(package.Args[0]);
                var methodName = System.Text.Encoding.Default.GetString(package.Args[1]);
                    
                    
                byte[] par = null;
                Guid returnId = Guid.Empty;
                if (package.Args.TryGetValue(2, out par))
                {
                    returnId = new Guid(par as byte[]);
                }

                var methodParams = (from p in package.Args
                                    where p.Key >= 3
                                    orderby p.Key
                                    select p.Value).ToArray();

                return _ToRequest(entityId, methodName, returnId, methodParams);
                
            }
            else if (package.Code == (byte)ClientToServerOpCode.Release)
            {
                var entityId = new Guid(package.Args[0]);
                _SoulProvider.Unbind(entityId);
                return null;
            }
            return null;
		}
        private Request _ToRequest(Guid entity_id, string method_name, Guid return_id, byte[][] method_params)
        {
            return new Request() { EntityId = entity_id, MethodName = method_name, MethodParams = method_params, ReturnId = return_id };
        }
        

		

		private bool _Connected()
		{
            return _Enable && _Socket.Connected  ;
		}

		void Remoting.IResponseQueue.Push(byte cmd, Dictionary<byte, byte[]> args)
		{
            lock (_LockResponse)
            {
                TotalResponse++;
                _Responses.Enqueue(new Regulus.Remoting.Package() { Code = cmd, Args = args });
            }
		}

        event Action<Guid, string, Guid, byte[][]> _InvokeMethodEvent;
        event Action<Guid, string, Guid, byte[][]> Remoting.IRequestQueue.InvokeMethodEvent
		{
			add
			{
                _InvokeMethodEvent += value;
			}
			remove
			{
                _InvokeMethodEvent -= value;
			}
		}

        event Action _BreakEvent;
		event Action Remoting.IRequestQueue.BreakEvent
		{
            add { _BreakEvent += value; }
            remove { _BreakEvent -= value; }
		}


        internal void Disconnect()
        {
            if (_BreakEvent != null)
                _BreakEvent();
        }

        public ISoulBinder Binder { get { return _SoulProvider; } }
        public CoreThreadRequestHandler Handler { get { return new CoreThreadRequestHandler(this); } }

        bool Utility.IUpdatable.Update()
        {
            if (_Connected())
            {
                _SoulProvider.Update();

                _ReadMachine.Update();
                _WriteMachine.Update();

                _TestConnect();
                

                return true;
            }
            Disconnect();
            return false;
        }

        private void _TestConnect()
        {
            
        }

        

        void Framework.ILaunched.Launch()
        {
            //_HandleWriteWait();
            //_HandleRead();

            _Reader.DoneEvent += _RequestPush ;
            _Reader.ErrorEvent += () => { _Enable = false; };
            _Reader.Start(_Socket);


            _Writer.ErrorEvent += () => { _Enable = false; };
            _Writer.CheckSourceEvent += _ResponsePop;
            _Writer.Start(_Socket);
        }

        private Package[] _ResponsePop()
        {
            lock (_LockResponse)
            {
                var pkgs = _Responses.DequeueAll();
                TotalResponse -= pkgs.Length;
                return pkgs;
            }
        }

    

        void Framework.ILaunched.Shutdown()
        {
            _Socket.Close();
            _Reader.Stop();
            _Writer.Stop();
            _ReadMachine.Termination();
            _WriteMachine.Termination();

            lock (_LockResponse)
            {
                var pkgs = _Responses.DequeueAll();
                TotalResponse -= pkgs.Length;
            }
                

            lock (_LockRequest)
            {
                var pkgs = _Requests.DequeueAll();
                TotalRequest -= pkgs.Length;
            }
                
        }


        void IRequestQueue.Update()
        {

            Package[] pkgs = null;
            lock (_LockRequest)
            {
                pkgs = _Requests.DequeueAll();
                TotalRequest -= pkgs.Length;
            }
                
            foreach(var pkg in pkgs)
            {
                var request = _TryGetRequest(pkg);
                
                if (request != null)
                {
                    
                    if(_InvokeMethodEvent != null  )
                        _InvokeMethodEvent(request.EntityId, request.MethodName, request.ReturnId, request.MethodParams);
                }

                
                
            }
                
            
        }
    }
	
}
