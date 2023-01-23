using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RPGbot.Classes
{
	public class PlayerClass
	{
		public PlayerClass GetClass(string nomeClasse)
		{
			string classesstring = File.ReadAllText($"../../db/g_data/classes.json");
			JToken classeObj = JObject.Parse(classesstring).GetValue(nomeClasse);
			string classestring = classeObj.ToString();
			PlayerClass _playerClass = JsonConvert.DeserializeObject<PlayerClass>(classestring);

			Fname = _playerClass.Fname;
			Mname = _playerClass.Mname;
			Dice = _playerClass.Dice;

			return this;
		}

		public string Fname { get; set; }
		public string Mname { get; set; }
		public int Dice { get; set; }
	}

	public class PlayerClasses
	{
		public static List<PlayerClass> GetClasses()
		{
			try
			{
				string classesstring = File.ReadAllText($"../../db/g_data/classes.json");

				Dictionary<string, dynamic> values = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(classesstring);

				List<PlayerClass> playerClasses = new List<PlayerClass>();

				foreach (KeyValuePair<string, dynamic> classe in values)
				{
					playerClasses.Add(JsonConvert.DeserializeObject<PlayerClass>(classe.Value.ToString()));
				}

				return playerClasses;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return null;
			}
		}
	}
}
