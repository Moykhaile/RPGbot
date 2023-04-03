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
			string jsonstring = File.ReadAllText($"../../db/g_data/itens.json");

			List<Item> list = JsonSerializer.Deserialize<List<Item>>(jsonstring);

			return list;
		}
		public Item Get(string nome)
		{
			nome = PlayerResponse.FormatID(nome);

			string jsonstring = File.ReadAllText($"../../db/g_data/itens.json");

			List<Item> list = JsonSerializer.Deserialize<List<Item>>(jsonstring);
			
			return list.Find(e => PlayerResponse.FormatID(e.Name) == nome);
		}
		public void Post(Item obj, string id)
		{
			string jsonstring = File.ReadAllText("../../db/g_data/itens.json");
			List<Item> list = JsonSerializer.Deserialize<List<Item>>(jsonstring);

			list.Add(obj);
			File.WriteAllText("../../db/g_data/itens.json", JsonSerializer.Serialize(list));
		}
		public void Put(Item obj)
		{
			string jsonstring = File.ReadAllText("../../db/g_data/itens.json");
			List<Item> list = JsonSerializer.Deserialize<List<Item>>(jsonstring);

			int index = list.FindIndex(e => e.Name == obj.Name);

			list[index] = obj;

			File.WriteAllText("../../db/g_data/itens.json", JsonSerializer.Serialize(list));
		}
		public void Delete(Item obj)
		{
			string jsonstring = File.ReadAllText("../../db/g_data/itens.json");
			List<Item> list = JsonSerializer.Deserialize<List<Item>>(jsonstring);

			list.Remove(obj);

			File.WriteAllText("../../db/g_data/itens.json", JsonSerializer.Serialize(list));
		}
	}

	public class DBmagia : IDB<Magia>
	{
		public List<Magia> GetAll()
		{
			string jsonstring = File.ReadAllText($"../../db/g_data/magias.json");

			List<Magia> list = JsonSerializer.Deserialize<List<Magia>>(jsonstring);

			return list;
		}
		public Magia Get(string nome)
		{
			nome = PlayerResponse.FormatID(nome);

			string jsonstring = File.ReadAllText($"../../db/g_data/magias.json");

			List<Magia> list = JsonSerializer.Deserialize<List<Magia>>(jsonstring);

			return list.Find(e => PlayerResponse.FormatID(e.Name) == nome);
		}
		public void Post(Magia obj, string id)
		{
			string jsonstring = File.ReadAllText("../../db/g_data/magias.json");
			List<Magia> magiaList = JsonSerializer.Deserialize<List<Magia>>(jsonstring);

			magiaList.Add(obj);
			File.WriteAllText("../../db/g_data/magias.json", JsonSerializer.Serialize(magiaList));
		}
		public void Put(Magia obj)
		{
			string jsonstring = File.ReadAllText("../../db/g_data/magias.json");
			List<Magia> magiaList = JsonSerializer.Deserialize<List<Magia>>(jsonstring);

			int index = magiaList.FindIndex(e => e.Name == obj.Name);

			magiaList[index] = obj;

			File.WriteAllText("../../db/g_data/magias.json", JsonSerializer.Serialize(magiaList));
		}
		public void Delete(Magia obj)
		{
			string jsonstring = File.ReadAllText("../../db/g_data/magias.json");
			List<Magia> magiaList = JsonSerializer.Deserialize<List<Magia>>(jsonstring);

			magiaList.Remove(obj);
			File.WriteAllText("../../db/g_data/magias.json", JsonSerializer.Serialize(magiaList));
		}
	}

	public class DBpericia : IDB<Pericia>
	{
		public List<Pericia> GetAll()
		{
			string jsonstring = File.ReadAllText($"../../db/g_data/pericias.json");

			List<Pericia> list = JsonSerializer.Deserialize<List<Pericia>>(jsonstring);

			return list;
		}
		public Pericia Get(string nome)
		{
			nome = PlayerResponse.FormatID(nome);

			string jsonstring = File.ReadAllText($"../../db/g_data/pericias.json");

			List<Pericia> list = JsonSerializer.Deserialize<List<Pericia>>(jsonstring);

			return list.Find(e => PlayerResponse.FormatID(e.Nome) == nome);
		}
		public void Post(Pericia obj, string id)
		{
			string jsonstring = File.ReadAllText("../../db/g_data/pericias.json");
			List<Pericia> magiaList = JsonSerializer.Deserialize<List<Pericia>>(jsonstring);

			magiaList.Add(obj);
			File.WriteAllText("../../db/g_data/pericias.json", JsonSerializer.Serialize(magiaList));
		}
		public void Put(Pericia obj)
		{
			string jsonstring = File.ReadAllText("../../db/g_data/pericias.json");
			List<Pericia> magiaList = JsonSerializer.Deserialize<List<Pericia>>(jsonstring);

			int index = magiaList.FindIndex(e => e.Nome == obj.Nome);

			magiaList[index] = obj;

			File.WriteAllText("../../db/g_data/pericias.json", JsonSerializer.Serialize(magiaList));
		}
		public void Delete(Pericia obj)
		{
			string jsonstring = File.ReadAllText("../../db/g_data/pericias.json");
			List<Pericia> magiaList = JsonSerializer.Deserialize<List<Pericia>>(jsonstring);

			magiaList.Remove(obj);
			File.WriteAllText("../../db/g_data/pericias.json", JsonSerializer.Serialize(magiaList));
		}
	}
}