using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace RPGbot.db
{
	public class DbHandler
	{
		static string playerPath = "../../db/c_player";

		public static bool SavePlayer(string playerID, Player player)
		{
			playerPath += $"{playerID}.json";
			if (!File.Exists(playerPath)) return false;

			try
			{
				File.WriteAllText(playerPath, JsonSerializer.Serialize(player));
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public static Player LoadPlayer(string playerID)
		{
			playerPath += $"{playerID}.json";
			if (!File.Exists(playerPath)) return null;

			Player player = JsonSerializer.Deserialize<Player>(File.ReadAllText(playerPath));

			return player;
		}
	}
}