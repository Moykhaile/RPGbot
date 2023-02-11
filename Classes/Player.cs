using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RPGbot.Classes
{
	public class Player
	{
		public Player GetPlayer(string playerID)
		{
			Player _player = JsonSerializer.Deserialize<Player>(File.ReadAllText($"../../db/c_player/nullPlayer.json"));
			if (File.Exists($"../../db/c_player/{playerID}.json"))
			{
				_player = JsonSerializer.Deserialize<Player>(File.ReadAllText($"../../db/c_player/{playerID}.json"));
			}

			Nome = _player.Nome;
			Jogador = _player.Jogador;
			Genero = _player.Genero;
			Raca = _player.Raca;
			Classe = _player.Classe;
			Sexualidade = _player.Sexualidade;
			Posicao = _player.Posicao;

			Idade = _player.Idade;
			Peso = _player.Peso;
			Altura = _player.Altura;

			ImgLink = _player.ImgLink;

			Forca = _player.Forca;
			Destreza = _player.Destreza;
			Inteligencia = _player.Inteligencia;
			Constituicao = _player.Constituicao;
			Sabedoria = _player.Sabedoria;
			Carisma = _player.Carisma;

			VidaMax = _player.VidaMax;
			Vida = _player.Vida;
			Saldo = _player.Saldo;
			XP = _player.XP;

			Magias = _player.Magias;

			return this;
		}

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
		public int Saldo { get; set; }
		public int XP { get; set; }

		public List<string> Magias { get; set; }
	}
}
