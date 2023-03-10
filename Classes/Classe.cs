using System.Collections.Generic;

namespace RPGbot.Classes
{
	public class Classe
	{
		public string Fname { get; set; }
		public string Mname { get; set; }
		public int Dice { get; set; }
		public int SaldoDice { get; set; }
		public int SaldoDiceNum { get; set; }
		public int SaldoDiceMod { get; set; }
		public bool Magico { get; set; }
		public List<int> Magias { get; set; }
	}
}
