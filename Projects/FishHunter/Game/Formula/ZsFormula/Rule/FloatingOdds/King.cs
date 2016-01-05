using System;
using System.Linq;

using NLog;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.FloatingOdds
{
	public class King : IFloatingCalculator
	{
		void IFloatingCalculator.Calculate(RequsetFishData[] fish_data)
		{
			var kings = fish_data.Where(x => x.FishStatus == FISH_STATUS.KING);

			foreach(var king in kings.Where(king => king.GraveGoods.Any()))
			{
				if(king.GraveGoods.Any(x => x.FishType != king.FishType))
				{
					// _OnException.Invoke("king.GraveGoods�A���������A����");
					Singleton<Log>.Instance.WriteInfo("king.GraveGoods�A���������A����");

					LogManager.GetCurrentClassLogger()
							.Fatal("king.GraveGoods�A���������A����");
					continue;
				}

				king.FishOdds += king.GraveGoods.Sum(x => x.FishOdds);
			}
		}
	}
}
