using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     ���o�D��
	/// </summary>
	public class SpecialItemRule
	{
		private readonly DataVisitor _Visitor;

		public SpecialItemRule(DataVisitor visitor)
		{
			_Visitor = visitor;
		}

		public void Run()
		{
			// �Z���X�{����+1
//			_Visitor.PlayerRecord.NowWeaponPower.WinFrequency++;
//
//			// �Z���X�{����+1
//			var stageWeaponData =
//				_Visitor.Farm.Record.SpecialWeaponDatas.Find(x => x.SpId == _Visitor.PlayerRecord.NowWeaponPower.SpId);
//			stageWeaponData.WinFrequency++;
		}
	}
}
