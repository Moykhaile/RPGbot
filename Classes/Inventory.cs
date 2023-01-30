using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RPGbot.Classes
{
	public class Inventory
	{
		public Inventory(string playerID)
		{
			if (File.Exists($"../../db/c_inventory/{playerID}.json"))
			{
				JToken itemObj = JObject.Parse(File.ReadAllText($"../../db/c_inventory/{playerID}.json"))["Items"];
				string itemstring = itemObj.ToString();
				List<Item> inventoryDic = JsonConvert.DeserializeObject<List<Item>>(itemstring);

				Items = inventoryDic;
			}
			else
				Items = new List<Item>();
		}

		public List<Item> Items { get; set; }
	}

	public enum Tipo { Arma, Armadura, Item }
	public class Item
	{
		public string Name { get; set; }
		public string Dano { get; set; }
		public int Defesa { get; set; }
		public Tipo Tipo { get; set; }
	}
}