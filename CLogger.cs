using Discord;
using Discord.Interactions;
using System.Threading.Tasks;

namespace RPGbot
{
	public class CLogger
	{
		public static ulong logChannel = 1069333219004137632;

		public static async Task Log(string command, IInteractionContext context, IResult result)
		{
			string erro = $"\n\n**{result.Error}**\n{result.ErrorReason}";

			Embed embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{context.User.Username}", IconUrl = $"{context.User.GetAvatarUrl()}" },
				Description = $"/{command}{(result.IsSuccess ? "" : erro)}",
				Footer = new EmbedFooterBuilder() { Text = $"{(result.IsSuccess ? "✅ Sucesso na execução!" : "⛔ ERRO!")}" },
				Color = 0x00ff00
			}.Build();

			var channel = await context.Guild.GetChannelAsync(logChannel) as IMessageChannel;

			await channel.SendMessageAsync($"/{command}", embed: embed);
		}
	}
}
