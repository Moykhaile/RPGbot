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
		public async Task HandleFichaCommand()
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			await RespondAsync($"Sua ficha de personagem!", ephemeral: true, embed: PlayerResponse.GerarFicha(player));
		}

		[SlashCommand("vida", "Adiciona ou remove vida do personagem")]
		public async Task HandleVidaCommand(int qntd)
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

			Embed embed = PlayerResponse.GerarValor("Vida", player, player.Vida, old_vida, qntd);

			await RespondAsync($"Vida do personagem alterada!", embed: embed);
		}

		[SlashCommand("xp", "Adiciona pontos de experiência ao personagem")]
		public async Task HandleXpCommand(int qntd)
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

			Embed embed = PlayerResponse.GerarValor("XP", player, player.XP, old_xp, qntd);

			await RespondAsync($"Pontos de experiência do personagem alterados!", embed: embed);
		}

		[SlashCommand("saldo", "Adiciona dinheiro à carteira do personagem")]
		public async Task HandleSaldoCommand(int qntd)
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

			Embed embed = PlayerResponse.GerarValor("Saldo", player, player.Saldo, old_saldo, qntd);

			await RespondAsync($"Saldo do personagem alterado!", embed: embed);
		}
	}
}
