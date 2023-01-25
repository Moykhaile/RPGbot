using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
	public class MasterModule : InteractionModuleBase<SocketInteractionContext>
	{
		public InteractionService Commands { get; set; }

		public MasterModule(DiscordSocketClient client) => _client = client;
		private readonly DiscordSocketClient _client;

		public enum Atributos
		{
			Nome,
			Jogador,
			Genero,
			Raca,
			Classe,
			Sexualidade,
			Posicao,
			Idade,
			Peso,
			Altura,
			Forca,
			Destreza,
			Inteligencia,
			Consituicao,
			Sabedoria,
			Carisma,
			VidaMax,
			Vida,
			Saldo,
			XP
		}

		[RequireRole("720507968663191614")]
		[SlashCommand("editplayer", "Editar informação do personagem")]
		public async Task EditPlayer(Atributos atributo, string valor, IMentionable user)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Player player = new Player().GetPlayer((user as SocketGuildUser).Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			JObject playerObj = JObject.Parse(JsonConvert.SerializeObject(player));
			if (playerObj.GetValue(atributo.ToString()) == null)
			{
				await RespondAsync($"Atributo \"{atributo}\" não encontrado.", ephemeral: true); return;
			}

			playerObj[atributo] = valor;

			DbHandler.SavePlayer((user as SocketGuildUser).Id.ToString(), JsonConvert.DeserializeObject<Player>(playerObj.ToString()));
			await RespondAsync($"Valor do atributo {atributo} alterado!", ephemeral: true);
		}

		[RequireRole("720507968663191614")]
		[SlashCommand("mostrarficha", "Apresenta a ficha de outro personagem")]
		public async Task MostrarFicha(IMentionable user)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Player player = new Player().GetPlayer((user as SocketGuildUser).Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			await RespondAsync($"Ficha de personagem de {(user as SocketGuildUser).DisplayName}!", ephemeral: true, embed: PlayerResponse.GerarFicha(player));
		}
	}
}
