using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     ���o�D��
	/// </summary>
	public class SpecialItemRule
	{
		private readonly StageDataVisitor _StageVisitor;

		public SpecialItemRule(StageDataVisitor stage_visitor)
		{
			_StageVisitor = stage_visitor;
		}

		public void Run()
		{
			// �Z���X�{����+1
//			_StageVisitor.PlayerRecord.NowWeaponPower.WinFrequency++;
//
//			// �Z���X�{����+1
//			var stageWeaponData =
//				_StageVisitor.FocusFishFarmData.RecordData.SpecialWeaponDatas.Find(x => x.SpId == _StageVisitor.PlayerRecord.NowWeaponPower.SpId);
//			stageWeaponData.WinFrequency++;
		}
	}
}
