using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RPGbot.Classes;
using RPGbot.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Discord.Commands;

namespace RPGbot.Modules
{
	public class PlayerModule : InteractionModuleBase<SocketInteractionContext>
	{
		[SlashCommand("ficha", "Apresenta a ficha do personagem")]
		public async Task Ficha()
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			Console.WriteLine($"> {player.Nome} : /ficha");
			await RespondAsync($"Sua ficha de personagem!", ephemeral: true, embed: PlayerResponse.GerarFicha(player));
		}

		[SlashCommand("vida", "Adiciona ou remove vida do personagem")]
		public async Task Vida([MaxValue(999), MinValue(-999)] int qntd)
		{
			if (qntd > 999 || qntd < -999 || qntd == 0)
			{
				await RespondAsync($"Valor inválido. Tente novamente com um número entre -999 e 999.", ephemeral: true);
				return;
			}

			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			int old_vida = player.Vida;
			player.Vida =
				player.Vida + qntd > player.VidaMax ? player.VidaMax :
				player.Vida + qntd < -4 ? -4 :
				player.Vida + qntd;

			DbHandler.SavePlayer(Context.User.Id.ToString(), player);

			Console.WriteLine($"> {player.Nome} : /vida {qntd}");
			await RespondAsync($"Vida do personagem alterada! Anterior: {old_vida}", embed: PlayerResponse.GerarFicha(player), ephemeral: true);
		}

		[SlashCommand("xp", "Adiciona pontos de experiência ao personagem")]
		public async Task XP([MaxValue(999), MinValue(1)] int qntd)
		{
			qntd = Math.Abs(qntd);
			if (qntd > 9999 || qntd == 0)
			{
				await RespondAsync($"Valor inválido. Tente novamente com um número entre 0 e 9999.", ephemeral: true); return;
			}

			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			int old_xp = player.XP;
			player.XP += qntd;

			DbHandler.SavePlayer(Context.User.Id.ToString(), player);

			Console.WriteLine($"> {player.Nome} : /xp {qntd}");
			await RespondAsync($"Pontos de experiência do personagem alterados! Anterior: {old_xp}", embed: PlayerResponse.GerarFicha(player), ephemeral: true);
		}

		[SlashCommand("saldo", "Adiciona dinheiro à carteira do personagem")]
		public async Task Saldo([MaxValue(9999), MinValue(-9999)] int qntd)
		{
			if (qntd > 9999 || qntd < -9999 || qntd == 0)
			{
				await RespondAsync($"Valor inválido. Tente novamente com um número entre -9999 e 9999.", ephemeral: true);
				return;
			}

			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			int old_saldo = player.Saldo;
			player.Saldo += qntd;
			player.Saldo = player.Saldo < 0 ? 0 : player.Saldo;

			DbHandler.SavePlayer(Context.User.Id.ToString(), player);

			Console.WriteLine($"> {player.Nome} : /saldo {qntd}");
			await RespondAsync($"Saldo do personagem alterado! Anterior: {old_saldo}", embed: PlayerResponse.GerarFicha(player), ephemeral: true);
		}

		[SlashCommand("addmagia", "Adiciona uma magia aos conhecimentos do personagem")]
		public async Task AddMagia(string magia)
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!new PlayerClass().GetClass(player.Classe).magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}

			byte[] tempBytes;
			tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(magia);
			magia = Encoding.UTF8.GetString(tempBytes).ToLower();

			string magias = File.ReadAllText($"../../db/g_data/magias.json");
			if (JObject.Parse(magias).GetValue(magia) == null)
			{
				await RespondAsync($"A magia informada não existe.", ephemeral: true); return;
			}

			if (player.Magias != null)
			{
				if (player.Magias.Contains(magia))
				{
					await RespondAsync($"Seu personagem já conhece esta magia.", ephemeral: true); return;
				}

				player.Magias.Add(magia);
			}
			else
				player.Magias = new List<string> { magia };

			DbHandler.SavePlayer(Context.User.Id.ToString(), player);

			Console.WriteLine($"> {player.Nome} : /addmagia {magia}");
			await RespondAsync($"Magia \"{JObject.Parse(magias).GetValue(magia)["name"]}\" adicionada ao personagem!", ephemeral: true);
		}

		[SlashCommand("removemagia", "Remove uma magia dos conhecimentos do personagem")]
		public async Task RemoveMagia(int indice)
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!new PlayerClass().GetClass(player.Classe).magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}

			if (player.Magias == null)
			{
				await RespondAsync($"O seu personagem não conhece nenhuma magia.", ephemeral: true); return;
			}

			player.Magias.RemoveAt(indice);

			DbHandler.SavePlayer(Context.User.Id.ToString(), player);

			Console.WriteLine($"> {player.Nome} : /removemagia {indice}");
			await RespondAsync($"Magia removida do personagem!", ephemeral: true);
		}

		[SlashCommand("magias", "Apresenta as mágias conhecidas pelo personagem")]
		public async Task Magias()
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!new PlayerClass().GetClass(player.Classe).magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}
			if (player.Magias == null)
			{
				await RespondAsync($"O seu personagem não conhece nenhuma magia.", ephemeral: true); return;
			}

			Console.WriteLine($"> {player.Nome} : /magias");
			await RespondAsync($"As magias do seu personagem!", ephemeral: true, embed: PlayerResponse.GerarMagias(player.Magias, player));
		}

		[SlashCommand("inventario", "Apresenta o inventário do personagem")]
		public async Task Inventario()
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			Inventory inventory = new Inventory(Context.User.Id.ToString());
			if (inventory.Items.Count == 0)
			{
				await RespondAsync($"{player.Nome} não tem itens em seu inventário.", ephemeral: true); return;
			}

			Console.WriteLine($"> {player.Nome} : /inventario");
			await RespondAsync($"Seu inventário de personagem!", ephemeral: true, embed: PlayerResponse.GerarInventario(inventory.Items, player));
		}

		[SlashCommand("customitem", "Adiciona um item personalizado ao inventário do personagem")]
		public async Task Customitem(string nome)
		{
			try
			{
				Player player = new Player().GetPlayer(Context.User.Id.ToString());
				if (player.Nome == null)
				{
					await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
				}

				nome = char.ToUpper(nome[0]) + nome.Substring(1).ToLower();

				Inventory inventory = new Inventory(Context.User.Id.ToString());

				inventory.Items.Add(new Item() { Name = nome });

				DbHandler.SaveInventory(Context.User.Id.ToString(), inventory);

				Console.WriteLine($"> {player.Nome} : /customitem {nome}");
				await RespondAsync($"Seu inventário de personagem!", ephemeral: true, embed: PlayerResponse.GerarInventario(inventory.Items, player));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		[SlashCommand("arma", "Adiciona uma arma ao inventário do personagem")]
		public async Task Arma(string nome, string dano)
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			nome = char.ToUpper(nome[0]) + nome.Substring(1).ToLower();

			Inventory inventory = new Inventory(Context.User.Id.ToString());

			inventory.Items.Add(new Item() { Name = nome, Dano = dano, Tipo = Tipo.Arma });

			DbHandler.SaveInventory(Context.User.Id.ToString(), inventory);

			Console.WriteLine($"> {player.Nome} : /arma {nome} | {dano}");
			await RespondAsync($"Seu inventário de personagem!", ephemeral: true, embed: PlayerResponse.GerarInventario(inventory.Items, player));
		}

		[SlashCommand("armadura", "Adiciona uma armadura ao inventário do personagem")]
		public async Task AddItem(string nome, int defesa)
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			nome = char.ToUpper(nome[0]) + nome.Substring(1).ToLower();

			Inventory inventory = new Inventory(Context.User.Id.ToString());

			inventory.Items.Add(new Item() { Name = nome, Defesa = defesa, Tipo = Tipo.Armadura });

			DbHandler.SaveInventory(Context.User.Id.ToString(), inventory);

			Console.WriteLine($"> {player.Nome} : /armadura {nome} | {defesa}");
			await RespondAsync($"Seu inventário de personagem!", ephemeral: true, embed: PlayerResponse.GerarInventario(inventory.Items, player));
		}

		[SlashCommand("item", "Adiciona um item ao inventário do personagem")]
		public async Task Item(string nome)
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			nome = char.ToUpper(nome[0]) + nome.Substring(1).ToLower();

			Inventory inventory = new Inventory(Context.User.Id.ToString());

			inventory.Items.Add(new Item() { Name = nome, Tipo = Tipo.Item });

			DbHandler.SaveInventory(Context.User.Id.ToString(), inventory);

			Console.WriteLine($"> {player.Nome} : /item {nome}");
			await RespondAsync($"Seu inventário de personagem!", ephemeral: true, embed: PlayerResponse.GerarInventario(inventory.Items, player));
		}
	}
}
