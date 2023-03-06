using RPGbot.Classes;
using RPGbot.Racas;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace RPGbot.db
{
	interface IDB<T>
	{
		List<T> GetAll();
		T Get(string id);
		void Post(T value, string id);
		void Put(T value);
		void Delete(T value);
	}

	public class DBpersonagem : IDB<Personagem>
	{
		readonly JsonSerializerOptions options = new JsonSerializerOptions
		{
			Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
		};

		public List<Personagem> GetAll()
		{
			string[] personagensStr = Directory.GetFiles($"../../db/c_player/");
			List<Personagem> personagens = new List<Personagem>();

			foreach (string personagemStr in personagensStr)
			{
				personagens.Add(JsonSerializer.Deserialize<Personagem>(File.ReadAllText(personagemStr)));
			}

			return personagens;
		}

		public Personagem Get(string id)
		{
			Personagem personagem = JsonSerializer.Deserialize<Personagem>(File.ReadAllText($"../../db/c_player/nullPlayer.json"));
			if (File.Exists($"../../db/c_player/{id}.json"))
			{
				personagem = JsonSerializer.Deserialize<Personagem>(File.ReadAllText($"../../db/c_player/{id}.json"));
			}

			return personagem;
		}

		public void Post(Personagem personagem, string id)
		{
			File.WriteAllText($"../../db/c_player/{id}.json", JsonSerializer.Serialize(personagem, options));
		}

		public void Put(Personagem personagem)
		{
			personagem.Saldo = float.Parse(personagem.Saldo.ToString("N2"));

			File.WriteAllText($"../../db/c_player/{personagem.Id}.json", JsonSerializer.Serialize(personagem, options));
		}

		public void Delete(Personagem personagem)
		{
			File.Delete($"../../db/c_player/{personagem.Id}.json");
		}
	}

	public class DBclasse : IDB<Classe>
	{
		public List<Classe> GetAll()
		{
			string classesPath = File.ReadAllText($"../../db/g_data/classes.json");

			List<Classe> classes = JsonSerializer.Deserialize<List<Classe>>(classesPath);

			return classes;
		}
		public Classe Get(string classe)
		{
			classe = PlayerResponse.FormatID(classe);

			string classesPath = File.ReadAllText($"../../db/g_data/classes.json");

			List<Classe> classes = JsonSerializer.Deserialize<List<Classe>>(classesPath);

			return classes.Find(e => PlayerResponse.FormatID(e.Mname) == classe);
		}

		public void Post(Classe classe, string id) { }
		public void Put(Classe classe) { }
		public void Delete(Classe classe) { }
	}

	public class DBraca : IDB<Raca>
	{
		public List<Raca> GetAll()
		{
			string racasPath = File.ReadAllText($"../../db/g_data/racas.json");

			List<Raca> racas = JsonSerializer.Deserialize<List<Raca>>(racasPath);

			return racas;
		}
		public Raca Get(string raca)
		{
			raca = PlayerResponse.FormatID(raca);

			string racasPath = File.ReadAllText($"../../db/g_data/racas.json");

			List<Raca> classes = JsonSerializer.Deserialize<List<Raca>>(racasPath);

			return classes.Find(e => PlayerResponse.FormatID(e.Mname) == raca);
		}

		public void Post(Raca raca, string id) { }
		public void Put(Raca raca) { }
		public void Delete(Raca raca) { }
	}

	public class DBitem : IDB<Item>
	{
		public List<Item> GetAll() { return null; }
		public Item Get(string item)
		{
			item = PlayerResponse.FormatID(item);

			string itensPath = File.ReadAllText($"../../db/g_data/itens.json");

			List<Item> classes = JsonSerializer.Deserialize<List<Item>>(itensPath);

			return classes.Find(e => PlayerResponse.FormatID(e.Name) == item);
		}
		public void Post(Item item, string id)
		{
			string itensstring = File.ReadAllText("../../db/g_data/itens.json");
			List<Item> itemList = JsonSerializer.Deserialize<List<Item>>(itensstring);

			itemList.Add(item);
			File.WriteAllText("../../db/g_data/itens.json", JsonSerializer.Serialize(itemList));
		}
		public void Put(Item item) { }
		public void Delete(Item item) { }
	}

	public class DBmagia : IDB<Magia>
	{
		public List<Magia> GetAll() { return null; }
		public Magia Get(string magia)
		{
			magia = PlayerResponse.FormatID(magia);

			string magiasPath = File.ReadAllText($"../../db/g_data/magias.json");

			List<Magia> magias = JsonSerializer.Deserialize<List<Magia>>(magiasPath);

			return magias.Find(e => PlayerResponse.FormatID(e.Name) == magia);
		}
		public void Post(Magia magia, string id)
		{
			string magiasPath = File.ReadAllText("../../db/g_data/magias.json");
			List<Magia> magiaList = JsonSerializer.Deserialize<List<Magia>>(magiasPath);

			magiaList.Add(magia);
			File.WriteAllText("../../db/g_data/magias.json", JsonSerializer.Serialize(magiaList));
		}
		public void Put(Magia magia) { }
		public void Delete(Magia magia) { }
	}
}