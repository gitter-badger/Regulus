﻿using Regulus.Framework;
using Regulus.Remoting;


using VGame.Project.FishHunter;

namespace Console
{
	public class ModeCreator
	{
		private readonly ICore core;

		public ModeCreator(ICore core)
		{
			// TODO: Complete member initialization
			this.core = core;
		}

		internal void OnSelect(GameModeSelector<IUser> selector)
		{
			selector.AddFactoty("standalong", new StandalongUserFactory(core));
			selector.AddFactoty("remoting", new RemotingUserFactory());

			// var provider = selector.CreateUserProvider("standalong");
			var provider = selector.CreateUserProvider("remoting");

			provider.Spawn("1");
			provider.Select("1");
		}
	}
}
