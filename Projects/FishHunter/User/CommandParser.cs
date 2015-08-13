﻿using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter
{
	public class CommandParser : ICommandParsable<IUser>
	{
		private readonly Command _Command;

		private readonly IUser _User;

		private readonly Console.IViewer _View;

		public CommandParser(Command command, Console.IViewer view, IUser user)
		{
			_Command = command;
			_View = view;
			_User = user;
		}

		void ICommandParsable<IUser>.Clear()
		{
			_DestroySystem();
		}

		void ICommandParsable<IUser>.Setup(IGPIBinderFactory factory)
		{
			_CreateSystem();

			_CreateConnect(factory);

			_CreateOnline(factory);

			_CreateSelectLevel(factory);

			_CreateVerify(factory);

			_CreatePlayer(factory);
		}

		private void _DestroySystem()
		{
			_Command.Unregister("Agent");
		}

		private void _ConnectResult(bool result)
		{
			_View.WriteLine(string.Format("Connect result {0}", result));
		}

		private void _CreateSelectLevel(IGPIBinderFactory factory)
		{
			var binder = factory.Create(_User.LevelSelectorProvider);

			binder.Bind<byte, Value<bool>>((gpi, level) => gpi.Select(level), _SelectLevelQueryResult);
		}

		private void _SelectLevelQueryResult(Value<bool> obj)
		{
			obj.OnValue += result => _View.WriteLine(string.Format("SelectLevelQueryResult = {0}", result));
		}

		private void _CreateSystem()
		{
		}

		private void _CreatePlayer(IGPIBinderFactory factory)
		{
			var player = factory.Create(_User.PlayerProvider);

			player.Bind(
				"Hit[bulletid,fishid]", 
				gpi =>
				{
					return new CommandParamBuilder().Build<int, int>(
						(b, f) =>
						{
							gpi.Hit(
								b, 
								new[]
								{
									f
								});
						});
				});
			player.Bind(
				"RequestBullet", 
				gpi => { return new CommandParamBuilder().BuildRemoting(gpi.RequestBullet, _GetBullet); });

			player.SupplyEvent += _RegisgetPlayerEvent;
		}

		private void _GetBullet(int obj)
		{
			_View.WriteLine("get WEAPON_TYPE id" + obj);
		}

		private void _RegisgetPlayerEvent(IPlayer source)
		{
			
		}

		private void _CreateVerify(IGPIBinderFactory factory)
		{
			var verify = factory.Create(_User.VerifyProvider);
			verify.Bind(
				"Login[result,id,password]", 
				gpi => { return new CommandParamBuilder().BuildRemoting<string, string, bool>(gpi.Login, _VerifyResult); });
		}

		private void _CreateOnline(IGPIBinderFactory factory)
		{
			var online = factory.Create(_User.Remoting.OnlineProvider);
			online.Bind(
				"Ping", 
				gpi => { return new CommandParamBuilder().Build(() => { _View.WriteLine("Ping : " + gpi.Ping); }); });
			online.Bind(gpi => gpi.Disconnect());
		}

		private void _CreateConnect(IGPIBinderFactory factory)
		{
			var connect = factory.Create(_User.Remoting.ConnectProvider);
			connect.Bind(
				"Connect[result , ipaddr ,port]", 
				gpi => { return new CommandParamBuilder().BuildRemoting<string, int, bool>(gpi.Connect, _ConnectResult); });
		}

		private void _VerifyResult(bool result)
		{
			_View.WriteLine(string.Format("Verify result {0}", result));
		}
	}
}
