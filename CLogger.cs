using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPGbot
{
	public class CLogger
	{
		public static ulong logChannel = 1069333219004137632;

		public static async Task Log(string command, IInteractionContext context, IReadOnlyCollection<IApplicationCommandInteractionDataOption> applicationCommandInteractionOption, IResult result)
		{
			string erro = $"\n\n**{result.Error}**\n{result.ErrorReason}";
			string args = "";
			foreach (var arg in applicationCommandInteractionOption)
			{
				args += $" {arg.Value}";
			}
			var user = (context.User as SocketGuildUser);

			Embed embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{context.User.Username}", IconUrl = $"{context.User.GetAvatarUrl()}" },
				Description = $"/{command}{args} {(result.IsSuccess ? "" : erro)}",
				Footer = new EmbedFooterBuilder() { Text = $"{(result.IsSuccess ? "✅ Sucesso na execução!" : "⛔ ERRO!")}" },
				Color = user.Roles.Contains(context.Guild.GetRole(720507968663191614)) ? new Color(0xED4245) : 0x57F287
			}.Build();

			var channel = await context.Guild.GetChannelAsync(logChannel) as IMessageChannel;

			await channel.SendMessageAsync($"/{command}", embed: embed);
		}
		public static async Task Log(string command, IInteractionContext context, IResult result)
		{
			string erro = $"\n\n**{result.Error}**\n{result.ErrorReason}";

			var user = (context.User as SocketGuildUser);

			Embed embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{context.User.Username}", IconUrl = $"{context.User.GetAvatarUrl()}" },
				Description = $"[{command}] {(result.IsSuccess ? "" : erro)}",
				Footer = new EmbedFooterBuilder() { Text = $"{(result.IsSuccess ? "✅ Sucesso na execução!" : "⛔ ERRO!")}" },
				Color = user.Roles.Contains(context.Guild.GetRole(720507968663191614)) ? new Color(0xED4245) : 0x57F287
			}.Build();

			var channel = await context.Guild.GetChannelAsync(logChannel) as IMessageChannel;

			await channel.SendMessageAsync($"/{command}", embed: embed);
		}

	}
}
