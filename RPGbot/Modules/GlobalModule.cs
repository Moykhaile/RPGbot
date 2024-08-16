using Discord.Interactions;
using Discord.WebSocket;
using RPGbotAPI.Controllers;
using RPGbotAPI.Services;

namespace RPGbot.Modules
{
	public class GlobalModule : InteractionModuleBase<SocketInteractionContext>
	{
		private readonly DiscordSocketClient _client;

		readonly BotInfoController botInfoController = new(new BotInfoService());

		public GlobalModule(DiscordSocketClient client)
		{
			_client = client;
		}

		public InteractionService? Commands { get; set; }

		[SlashCommand("ping", "Teste a conexão do bot")]
		public async Task HandlePingCommand()
		{
			await RespondAsync($"Pong!  🛰️  {_client.Latency}ms\n\nRPGbot, por Moykhaile\n*{botInfoController.Get().Result.Versao}*", ephemeral: true);
		}
	}
}
