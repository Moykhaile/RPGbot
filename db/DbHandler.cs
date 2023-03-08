using RPGbot.Classes;
using RPGbot.Racas;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
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
			Personagem personagem = null;
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
		readonly JsonSerializerOptions options = new JsonSerializerOptions
		{
			Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
		};

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

		public void Post(Classe classe, string id)
		{
			string classesPath = File.ReadAllText($"../../db/g_data/classes.json");

			List<Classe> classes = JsonSerializer.Deserialize<List<Classe>>(classesPath);
			classes.Add(classe);

			File.WriteAllText($"../../db/g_data/classes.json", JsonSerializer.Serialize(classe, options));
		}
		public void Put(Classe classe)
		{
			string classesPath = File.ReadAllText($"../../db/g_data/classes.json");

			List<Classe> classes = JsonSerializer.Deserialize<List<Classe>>(classesPath);

			int index = classes.FindIndex(e => e.Mname == classe.Mname);

			classes[index] = classe;

			File.WriteAllText($"../../db/g_data/classes.json", JsonSerializer.Serialize(classe, options));
		}
		public void Delete(Classe classe)
		{
			string classesPath = File.ReadAllText($"../../db/g_data/classes.json");

			List<Classe> classes = JsonSerializer.Deserialize<List<Classe>>(classesPath);

			classes.Remove(classe);

			File.WriteAllText($"../../db/g_data/classes.json", JsonSerializer.Serialize(classe, options));
		}
	}

	public class DBraca : IDB<Raca>
	{
		readonly JsonSerializerOptions options = new JsonSerializerOptions
		{
			Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
		};

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

			List<Raca> racas = JsonSerializer.Deserialize<List<Raca>>(racasPath);

			return racas.Find(e => PlayerResponse.FormatID(e.Mname) == raca);
		}

		public void Post(Raca raca, string id)
		{
			string racasPath = File.ReadAllText($"../../db/g_data/racas.json");

			List<Raca> racas = JsonSerializer.Deserialize<List<Raca>>(racasPath);
			racas.Add(raca);

			File.WriteAllText($"../../db/g_data/racas.json", JsonSerializer.Serialize(raca, options));
		}
		public void Put(Raca raca)
		{
			string racasPath = File.ReadAllText($"../../db/g_data/racas.json");

			List<Raca> racas = JsonSerializer.Deserialize<List<Raca>>(racasPath);

			int index = racas.FindIndex(e => e.Mname == raca.Mname);

			racas[index] = raca;

			File.WriteAllText($"../../db/g_data/racas.json", JsonSerializer.Serialize(raca, options));
		}
		public void Delete(Raca raca)
		{
			string racasPath = File.ReadAllText($"../../db/g_data/racas.json");

			List<Raca> racas = JsonSerializer.Deserialize<List<Raca>>(racasPath);

			racas.Remove(raca);

			File.WriteAllText($"../../db/g_data/racas.json", JsonSerializer.Serialize(racas, options));
		}
	}

	public class DBitem : IDB<Item>
	{
		public List<Item> GetAll()
		{
			string itensPath = File.ReadAllText($"../../db/g_data/itens.json");

			List<Item> itens = JsonSerializer.Deserialize<List<Item>>(itensPath);

			return itens;
		}
		public Item Get(string item)
		{
			item = PlayerResponse.FormatID(item);

			string itensPath = File.ReadAllText($"../../db/g_data/itens.json");

			List<Item> itens = JsonSerializer.Deserialize<List<Item>>(itensPath);

			return itens.Find(e => PlayerResponse.FormatID(e.Name) == item);
		}
		public void Post(Item item, string id)
		{
			string itensstring = File.ReadAllText("../../db/g_data/itens.json");
			List<Item> itemList = JsonSerializer.Deserialize<List<Item>>(itensstring);

			itemList.Add(item);
			File.WriteAllText("../../db/g_data/itens.json", JsonSerializer.Serialize(itemList));
		}
		public void Put(Item item)
		{
			string itensstring = File.ReadAllText("../../db/g_data/itens.json");
			List<Item> itemList = JsonSerializer.Deserialize<List<Item>>(itensstring);

			int index = itemList.FindIndex(e => e.Name == item.Name);

			itemList[index] = item;

			File.WriteAllText("../../db/g_data/itens.json", JsonSerializer.Serialize(itemList));
		}
		public void Delete(Item item)
		{
			string itensstring = File.ReadAllText("../../db/g_data/itens.json");
			List<Item> itemList = JsonSerializer.Deserialize<List<Item>>(itensstring);

			itemList.Remove(item);

			File.WriteAllText("../../db/g_data/itens.json", JsonSerializer.Serialize(itemList));
		}
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
		public void Put(Magia magia)
		{
			string magiasPath = File.ReadAllText("../../db/g_data/magias.json");
			List<Magia> magiaList = JsonSerializer.Deserialize<List<Magia>>(magiasPath);

			int index = magiaList.FindIndex(e => e.Name == magia.Name);

			magiaList[index] = magia;

			File.WriteAllText("../../db/g_data/magias.json", JsonSerializer.Serialize(magiaList));
		}
		public void Delete(Magia magia)
		{
			string magiasPath = File.ReadAllText("../../db/g_data/magias.json");
			List<Magia> magiaList = JsonSerializer.Deserialize<List<Magia>>(magiasPath);

			magiaList.Remove(magia);
			File.WriteAllText("../../db/g_data/magias.json", JsonSerializer.Serialize(magiaList));
		}
	}
}