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
			List<Item> items = new List<Item>();

			if (File.Exists($"../../db/c_inventory/{playerID} - itens.json"))
			{
				Dictionary<string, dynamic> inventoryDic = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText($"../../db/c_inventory/{playerID} - itens.json"));


				//Items = inventoryDic.Select(p => new Item { Name = p.Value.ToString() }).ToList();
				foreach (KeyValuePair<string, dynamic> itens in inventoryDic)
				{
					foreach (JObject itemObj in itens.Value)
						items.Add(itemObj.ToObject<Item>());
				}
			}

			Items = items;
		}
		public Inventory(string playerID, int atributo, int tipo)
		{
			List<Item> items = new List<Item>();

			if (tipo == 1)
				if (File.Exists($"../../db/c_inventory/{playerID} - armas.json"))
				{
					Dictionary<string, dynamic> inventoryDic = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText($"../../db/c_inventory/{playerID} - armas.json"));


					//Items = inventoryDic.Select(p => new Item { Name = p.Value.ToString() }).ToList();
					foreach (KeyValuePair<string, dynamic> itens in inventoryDic)
					{
						foreach (JObject itemObj in itens.Value)
							items.Add(itemObj.ToObject<Item>());
					}
				}
				else { }
			else
			{
				if (File.Exists($"../../db/c_inventory/{playerID} - armaduras.json"))
				{
					Dictionary<string, dynamic> inventoryDic = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText($"../../db/c_inventory/{playerID} - armaduras.json"));


					//Items = inventoryDic.Select(p => new Item { Name = p.Value.ToString() }).ToList();
					foreach (KeyValuePair<string, dynamic> itens in inventoryDic)
					{
						foreach (JObject itemObj in itens.Value)
							items.Add(itemObj.ToObject<Item>());
					}
				}
			}

			Items = items;
		}

		public List<Item> Items { get; set; }
	}
	public class Item
	{
		public string Name { get; set; }

		public int Dano { get; set; }
		public int Defesa { get; set; }
	}
}