using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RPGbot.Racas
{
	public class PlayerRaca
	{
		public PlayerRaca GetRaca(string nomeRaca)
		{
			string racasstring = File.ReadAllText($"../../db/g_data/racas.json");
			JToken racaObj = JObject.Parse(racasstring)[nomeRaca];
			string racastring = racaObj.ToString();
			PlayerRaca _playerRaca = JsonConvert.DeserializeObject<PlayerRaca>(racastring);

			Mname = _playerRaca.Mname;
			Fname = _playerRaca.Fname;

			return this;
		}

		public string Mname { get; set; }
		public string Fname { get; set; }
	}

	public class PlayerRacas
	{
		public static List<PlayerRaca> GetRacas()
		{
			string racasstring = File.ReadAllText($"../../db/g_data/racas.json");

			Dictionary<string, dynamic> values = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(racasstring);
			//var values = JsonConvert.DeserializeObject<List<MatrixModel>>(racasstring);
			//var values = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(racasstring);

			List<PlayerRaca> playerRacas = new List<PlayerRaca>();

			foreach (KeyValuePair<string, dynamic> raca in values)
			{
				playerRacas.Add(JsonConvert.DeserializeObject<PlayerRaca>(raca.Value.ToString()));
			}

			return playerRacas;
		}
	}
}