using Discord;
using Discord.Interactions;
using RPGbot.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGbot
{
	public class CLogger
	{
		public static ulong logChannel = 1069333219004137632;

		public static async Task Log(SocketInteractionContext Context, string command, int[] values)
		{
			Embed embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{Context.User.Username}" },
				Description = $"{values[0]} {(values[1] > -1 ? '+' : '-')} {Math.Abs(values[1])} = {values[2]}",
				Footer = new EmbedFooterBuilder() { Text = $"{DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss:ff")}" },
				Color = 0x00ff00
			}.Build();

			var channel = Context.Guild.GetChannel(logChannel) as IMessageChannel;

			await channel.SendMessageAsync($"/{command}", embed: embed);
		}
	}
}
