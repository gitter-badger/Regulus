﻿using System.Linq;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using Regulus.Game;


using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;


using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace GameTest.ZsFormulaTest
{
	[Binding]
	[Scope(Feature = "ScoreOdds")]
	public class ScoreOddsSteps
	{
		private ScoreOddsTable _SorceOddsTable;

		[Given(@"賠率表")]
		public void Given賠率表(Table table)
		{
			_SorceOddsTable = new ScoreOddsTable(_ToData(table));
		}

		[When(@"機率是(.*)")]
		public void When機率是(float p0)
		{
			var value = _SorceOddsTable.Dice(p0);
			ScenarioContext.Current.Set(value, "ScoreOdds");
		}

		[Then(@"賠率為(.*)")]
		public void Then賠率為(int p0)
		{
			var odds = ScenarioContext.Current.Get<int>("ScoreOdds");
			Assert.AreEqual(p0, odds);
		}

		private ChancesTable<int>.Data[] _ToData(Table table)
		{
			var datas = table.CreateSet<ChancesTable<int>.Data>();
			return datas.ToArray();
		}
	}
}
