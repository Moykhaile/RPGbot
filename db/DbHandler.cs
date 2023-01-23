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
			try
			{
				File.WriteAllText($"../../db/c_player/{playerID}.json", JsonSerializer.Serialize(player));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
		/*public static PlayerClasses LoadClasses()
		{
			classPath += $"classes.json";
			if (!File.Exists(classPath)) return null;

			PlayerClasses classes = JsonSerializer.Deserialize<PlayerClasses>(File.ReadAllText(classPath));

			return classes;
		}*/
	}
}