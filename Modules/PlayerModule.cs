using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using RPGbot.Classes;
using RPGbot.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
			try
			{
				Player player = DbHandler.LoadPlayer(Context.User.Id.ToString());
				if (player == null)
				{
					await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
				}

				await RespondAsync($"Sua ficha de personagem!", ephemeral: true, embed: PlayerResponse.GerarFicha(player));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		[SlashCommand("vida", "Adiciona ou remove vida do personagem")]
		public async Task HandleVidaCommand(int qntd)
		{
			if (qntd > 999 || qntd < -999 || qntd == 0)
			{
				await RespondAsync($"Valor inválido. Tente novamente com um número entre -999 e 999.", ephemeral: true);
				return;
			}

			Player player = DbHandler.LoadPlayer(Context.User.Id.ToString());
			if (player == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			int old_vida = player.vida;
			player.vida =
				player.vida + qntd > player.vidamax ? player.vidamax :
				player.vida + qntd < -4 ? -4 :
				player.vida + qntd;

			bool saved = DbHandler.SavePlayer("324605986683748352", player);

			Embed embed = PlayerResponse.GerarValor("Vida", player, player.vida, old_vida, qntd);

			if (saved)
				await RespondAsync($"Vida do personagem alterada!", embed: embed);
			else
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true);
		}

		[SlashCommand("xp", "Adiciona pontos de experiência (XP) ao personagem")]
		public async Task HandleXpCommand(int qntd)
		{
			qntd = Math.Abs(qntd);
			if (qntd > 9999 || qntd == 0)
			{
				await RespondAsync($"Valor inválido. Tente novamente com um número entre 0 e 9999.", ephemeral: true); return;
			}

			Player player = DbHandler.LoadPlayer(Context.User.Id.ToString());
			if (player == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			int old_xp = player.xp;
			player.xp += qntd;
			player.xp = player.xp > 355000 ? 355000 : player.xp;

			bool saved = DbHandler.SavePlayer("324605986683748352", player);

			Embed embed = PlayerResponse.GerarValor("XP", player, player.xp, old_xp, qntd);

			if (saved)
				await RespondAsync($"Pontos de experiência do personagem alterados! Cheque o nível do personagem.", embed: embed);
			else
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true);
		}
	}
}
