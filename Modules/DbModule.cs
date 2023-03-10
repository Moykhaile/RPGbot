using Discord.Interactions;
using Newtonsoft.Json;
using RPGbot.Classes;
using RPGbot.db;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace RPGbot.Modules
{
	public class DbModule : InteractionModuleBase<SocketInteractionContext>
	{
		//[RequireRole("Mestre")]
		[SlashCommand("dbadditem", "Adiciona item à base de dados do RPGbot")]
		public async Task AddItem(string Name, string Tipo, float Preco, float Peso)
		{
			try
			{
				string itensstring = File.ReadAllText("../../db/g_data/itens.json");
				List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(itensstring);

				itemList.Add(new Item
				{
					Name = Name,
					Peso = Peso,
					Dano = string.Empty,
					Defesa = 0,
					Tipo = Tipo,
					ModNome = string.Empty,
					Propriedades = string.Empty,
					Preco = Preco
				});
				var itemOutput = JsonConvert.SerializeObject(itemList, Formatting.Indented);
				File.WriteAllText("../../db/g_data/itens.json", itemOutput);

				await RespondAsync($"Item '{Name}' adicionado à base de dados!");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		//[RequireRole("Mestre")]
		[SlashCommand("dbaddarma", "Adiciona arma à base de dados do RPGbot")]
		public async Task AddArma(string Name, string Tipo, float Preco, string Dano, float Peso, string Propriedades)
		{
			try
			{
				string itensstring = File.ReadAllText("../../db/g_data/itens.json");
				List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(itensstring);

				itemList.Add(new Item
				{
					Name = Name,
					Peso = Peso,
					Dano = Dano,
					Defesa = 0,
					Tipo = Tipo,
					ModNome = string.Empty,
					Propriedades = Propriedades,
					Preco = Preco
				});
				var itemOutput = JsonConvert.SerializeObject(itemList, Formatting.Indented);
				File.WriteAllText("../../db/g_data/itens.json", itemOutput);

				await RespondAsync($"Item '{Name}' adicionado à base de dados!");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		//[RequireRole("Mestre")]
		[SlashCommand("dbaddarmadura", "Adiciona armadura à base de dados do RPGbot")]
		public async Task AddArmadura(string Name, string Tipo, float Preco, int Defesa, string ModNome, float Peso, string Propriedades)
		{
			try
			{
				if (ModNome == ".") ModNome = "";
				string itensstring = File.ReadAllText("../../db/g_data/itens.json");
				List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(itensstring);

				itemList.Add(new Item
				{
					Name = Name,
					Peso = Peso,
					Dano = string.Empty,
					Defesa = Defesa,
					Tipo = Tipo,
					ModNome = ModNome,
					Propriedades = Propriedades,
					Preco = Preco
				});
				var itemOutput = JsonConvert.SerializeObject(itemList, Formatting.Indented);
				File.WriteAllText("../../db/g_data/itens.json", itemOutput);

				await RespondAsync($"Item '{Name}' adicionado à base de dados!");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		[RequireRole("Mestre")]
		[SlashCommand("dbaddmagia", "Adiciona magia à base de dados do RPGbot")]
		public async Task AddMagia(string Nome, string Lançamento, string Alcance, string Componentes, string Duração, string Descrição)
		{
			Magia magia = new Magia() { Name = Nome, Casting = Lançamento, Range = Alcance, Components = Componentes, Duration = Duração, Description = Descrição };

			new DBmagia().Post(magia, magia.Name);

			await RespondAsync($"Magia ``{magia.Name}`` adicionada à base");
		}

		[RequireRole("Mestre")]
		[SlashCommand("dbaddpericia", "Adiciona perícia à base de dados do RPGbot")]
		public async Task AddPericia(string nome, PlayerResponse.Atributos atributo, string descricao)
		{
			Pericia pericia = new Pericia() { Nome = nome, Atributo = atributo, Descricao = descricao };

			new DBpericia().Post(pericia, pericia.Nome);

			await RespondAsync($"Perícia ``{pericia.Nome}`` adicionada à base");
		}
	}
}
