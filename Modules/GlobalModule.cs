using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		[SlashCommand("ping", "Receive a pong!")]
		public async Task HandlePingCommand()
		{
			await RespondAsync($"Pong!  🛰️  {_client.Latency}ms");
		}
	}
}
