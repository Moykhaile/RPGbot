using System.Collections.Generic;

namespace RPGbot.Classes
{
	public class Personagem
	{
		public ulong Id { get; set; }

		public string Nome { get; set; }
		public string Jogador { get; set; }
		public string Genero { get; set; }
		public string Raca { get; set; }
		public string Classe { get; set; }
		public string Sexualidade { get; set; }
		public string Posicao { get; set; }

		public int Idade { get; set; }
		public int Peso { get; set; }
		public int Altura { get; set; }

		public string[] ImgLink { get; set; }

		public int Forca { get; set; }
		public int Destreza { get; set; }
		public int Inteligencia { get; set; }
		public int Constituicao { get; set; }
		public int Sabedoria { get; set; }
		public int Carisma { get; set; }

		public int VidaMax { get; set; }
		public int Vida { get; set; }
		public float Saldo { get; set; }
		public int XP { get; set; }

		public string Armadura { get; set; }
		public string Escudo { get; set; }

		public List<string> Inventario { get; set; }
		public List<string> Magias { get; set; }
		public List<string> Pericias { get; set; }

		public int Exaustão { get; set; }
	}
}
