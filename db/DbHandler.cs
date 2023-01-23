using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using RPGbot.Classes;
using Newtonsoft.Json.Linq;

namespace RPGbot.db
{
	public class DbHandler
	{
		public static void SavePlayer(string playerID, Player player)
		{
			File.WriteAllText($"../../db/c_player/{playerID}.json", JsonSerializer.Serialize(player));
		}
	}
}