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
		public InteractionService Commands { get; set; }

		public PlayerModule(DiscordSocketClient client) => _client = client;
		private readonly DiscordSocketClient _client;

		[SlashCommand("ficha", "Apresenta a ficha do personagem")]
		public async Task Ficha()
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

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

			Embed embed = PlayerResponse.GerarValor("Vida", player, player.Vida, old_vida, qntd, Context);

			await RespondAsync($"Vida do personagem alterada em {qntd}!");
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

			Embed embed = PlayerResponse.GerarValor("XP", player, player.XP, old_xp, qntd, Context);

			await RespondAsync($"Pontos de experiência do personagem alterados!", embed: embed);
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

			Embed embed = PlayerResponse.GerarValor("Saldo", player, player.Saldo, old_saldo, qntd, Context);

			await RespondAsync($"Saldo do personagem alterado!", embed: embed);
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

			await RespondAsync($"Seu inventário de personagem!", ephemeral: true, embed: PlayerResponse.GerarInventario(inventory.Items, player));
		}

		[SlashCommand("customitem", "Adiciona um item personalizado ao inventário do personagem")]
		public async Task customitem(string nome)
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

				await Context.Channel.SendMessageAsync($"Item adicionado ao inventário de {player.Nome}!");
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

			await Context.Channel.SendMessageAsync($"Item adicionado ao inventário de {player.Nome}!");
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

			await Context.Channel.SendMessageAsync($"Item adicionado ao inventário de {player.Nome}!");
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

			await Context.Channel.SendMessageAsync($"Item adicionado ao inventário de {player.Nome}!");
			await RespondAsync($"Seu inventário de personagem!", ephemeral: true, embed: PlayerResponse.GerarInventario(inventory.Items, player));
		}
	}
}
