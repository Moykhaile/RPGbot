using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using RPGbot.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGbot.Modules
{
	public class AddPlayerModule : InteractionModuleBase<SocketInteractionContext>
	{
		public InteractionService Commands { get; set; }

		public AddPlayerModule(DiscordSocketClient client) => _client = client;
		private readonly DiscordSocketClient _client;


		[SlashCommand("add", "Adiciona um personagem ao jogo. Só é permitido 1 personagem por jogador")]
		public async Task HandleAddCommand()
		{
			Player player = DbHandler.LoadPlayer(Context.User.Id.ToString());
			if (player != null)
			{
				await RespondAsync($"Você já tem um personagem! {player.name} já existe.", null, false, true); return;
			}

			await RespondAsync();
		}
	}
}

public class AddModal : IModal
{
	public string Title => "Crie seu personagem!";

	[InputLabel("Nome completo do jogador")]
	[ModalTextInput("jogador", TextInputStyle.Short, "Seu nome...", maxLength:100)]
	public string jogador { get; set; }

	[InputLabel("Nome do personagem")]
	[ModalTextInput("name", TextInputStyle.Short, "Nome do personagem...", maxLength:40)]
	public string name { get; set; }

	[InputLabel("Idade do personagem (número inteiro)")]
	[ModalTextInput("age", TextInputStyle.Short, "Idade do personagem...", maxLength: 3)]
	public string age { get; set; }

	[InputLabel("Peso do personagem (número inteiro em kg)")]
	[ModalTextInput("weight", TextInputStyle.Short, "Peso do personagem...", maxLength: 3)]
	public string weight { get; set; }

	[InputLabel("Altura do personagem (em cm)")]
	[ModalTextInput("height", TextInputStyle.Short, "Altura do personagem...", maxLength: 3)]
	public string height { get; set; }
}