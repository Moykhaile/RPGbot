using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace RPGbot.Modules
{
	public class GlobalModule : InteractionModuleBase<SocketInteractionContext>
	{
		private readonly DiscordSocketClient _client;

		public GlobalModule(DiscordSocketClient client)
		{
			_client = client;
		}

		public InteractionService Commands { get; set; }

		[SlashCommand("ping", "Teste a conexão do bot")]
		public async Task HandlePingCommand()
		{
			await RespondAsync($"Pong!  🛰️  {_client.Latency}ms", ephemeral: true);
		}
	}
}
