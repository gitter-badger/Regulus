using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     ���o�D��
	/// </summary>
	public class SpecialItemRule
	{
		private readonly FarmDataVisitor _FarmVisitor;

		public SpecialItemRule(FarmDataVisitor farm_visitor)
		{
			_FarmVisitor = farm_visitor;
		}

		public void Run()
		{
			// �Z���X�{����+1
//			_FarmVisitor.PlayerRecord.NowWeaponPower.WinFrequency++;
//
//			// �Z���X�{����+1
//			var stageWeaponData =
//				_FarmVisitor.FocusFishFarmData.RecordData.SpecialWeaponDatas.Find(x => x.SpId == _FarmVisitor.PlayerRecord.NowWeaponPower.SpId);
//			stageWeaponData.WinFrequency++;
		}
	}
}
