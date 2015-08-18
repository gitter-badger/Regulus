// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveDeathFishHistory.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   �O���D����o����
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Linq;
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	/// �O���S����o����
	/// </summary>
	public class SaveDeathFishHistory
	{
		private readonly RequsetFishData _Fish;

		private readonly StageDataVisitor _StageVisitor;

		public SaveDeathFishHistory(StageDataVisitor stage_visitor, RequsetFishData fish)
		{
			_Fish = fish;
			_StageVisitor = stage_visitor;
		}

		public void Run()
		{
			// �splayer��������
			_StageVisitor.PlayerRecord.FindStageRecord(_StageVisitor.FocusFishFarmData.FarmId)
			             .FishHitReuslt.Items.Where(x => x.FishType == _Fish.FishType).First().KillCount++;
			             
			// �sstage�����`����
			_StageVisitor.FocusFishFarmData.RecordData.FishHitReuslt.Items.Where(x => x.FishType == _Fish.FishType).First().KillCount++;
		}
	}
}
