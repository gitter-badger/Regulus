﻿using System;
using System.Net.Sockets;


using Regulus.Utility;

namespace Regulus.Remoting
{
	public delegate void OnErrorCallback();

	public delegate void OnByteDataCallback(byte[] bytes);

	public delegate void OnPackageCallback(Package package);

	public class PackageReader
	{
		public event OnPackageCallback DoneEvent
		{
			add { _DoneEvent += value; }
			remove { _DoneEvent -= value; }
		}

		public event OnErrorCallback ErrorEvent;

		private enum Status
		{
			Begin, 

			End
		}

		private const int _HeadSize = 4;

		private OnPackageCallback _DoneEvent;

		private SocketReader _Reader;

		private Socket _Socket;

		private volatile bool _Stop;

		public void Start(Socket socket)
		{
			Singleton<Log>.Instance.WriteInfo("pakcage read start.");
			_Stop = false;
			_Socket = socket;
			_ReadHead();
		}

		private void _ReadHead()
		{
			_Reader = new SocketReader(_Socket);
			_Reader.DoneEvent += _ReadBody;
			_Reader.ErrorEvent += ErrorEvent;

			_Reader.Read(PackageReader._HeadSize);
		}

		private void _ReadBody(byte[] bytes)
		{
			var bodySize = BitConverter.ToInt32(bytes, 0);
			_Reader = new SocketReader(_Socket);
			_Reader.DoneEvent += _Package;
			_Reader.ErrorEvent += ErrorEvent;

			_Reader.Read(bodySize);
		}

		private void _Package(byte[] bytes)
		{
			var pkg = TypeHelper.Deserialize<Package>(bytes);

			_DoneEvent.Invoke(pkg);

			if(_Stop == false)
			{
				_ReadHead();
			}
		}

		public void Stop()
		{
			_Stop = true;
			_Socket = null;

			Singleton<Log>.Instance.WriteInfo("pakcage read stop.");
		}
	}

	internal class SocketReader
	{
		public event OnByteDataCallback DoneEvent;

		public event OnErrorCallback ErrorEvent;

		private readonly Socket _Socket;

		private byte[] _Buffer;

		private int _Offset;

		public SocketReader(Socket _Socket)
		{
			this._Socket = _Socket;
		}

		internal void Read(int size)
		{
			_Offset = 0;
			_Buffer = new byte[size];
			try
			{
				_Socket.BeginReceive(_Buffer, _Offset, _Buffer.Length - _Offset, 0, _Readed, null);
			}
			catch(SystemException e)
			{
				if(ErrorEvent != null)
				{
					ErrorEvent();
				}
			}
		}

		private void _Readed(IAsyncResult ar)
		{
			try
			{
				var readSize = _Socket.EndReceive(ar);
				_Offset += readSize;

				if(readSize == 0)
				{
					ErrorEvent();
				}
				else if(_Offset == _Buffer.Length)
				{
					DoneEvent(_Buffer);
				}
				else
				{
					_Socket.BeginReceive(
						_Buffer, 
						_Offset, 
						_Buffer.Length - _Offset, 
						SocketFlags.None, 
						_Readed, 
						null);
				}
			}
			catch(SystemException e)
			{
				if(ErrorEvent != null)
				{
					ErrorEvent();
				}
			}
			finally
			{
			}
		}
	}
}
