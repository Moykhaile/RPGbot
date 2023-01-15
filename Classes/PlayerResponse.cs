using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RPGbot.Classes
{
	public class PlayerResponse
	{
		public static Embed GerarFicha(Player player)
		{
			Embed embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{player.name}   {player.vida}/{player.vidamax}hp" },
				Description = $"{player.classe}   -   {player.position}   -   {player.xp}xp",
				Footer = new EmbedFooterBuilder() { Text = $"🪙 {player.saldo}po   -   {player.jogador}" },
				Fields = new List<EmbedFieldBuilder>()
				{
					new EmbedFieldBuilder()
					{
						Name = "Atributos",
						Value = $"```FOR {GerarMod(player.forca)}   DES {GerarMod(player.destreza)}\nINT {GerarMod(player.inteligencia)}   CON {GerarMod(player.constituicao)}\nSAB {GerarMod(player.sabedoria)}   CAR {GerarMod(player.carisma)}```",
						IsInline = true
					},
					new EmbedFieldBuilder()
					{
						Name = "| ",
						Value = "| \n| \n| \n| ",
						IsInline = true
					},
					new EmbedFieldBuilder()
					{
						Name = $"Nível {GerarNivel(player.xp)}",
						Value = $"{player.weight}kg   {player.height}cm\n{player.age} anos de idade",
						IsInline = true
					}
				},
				Color = GerarCorVida(player.vida, player.vidamax)
			}.Build();

			return embed;
		}
		public static Embed GerarValor(string nomev, Player player, int newv, int oldv, int v)
		{
			Embed embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{player.name}" },
				Description = $"{nomev}:   {oldv} {(v < 0 ? '-' : '+')} {Math.Abs(v)} =   **{newv}**",
				Footer = new EmbedFooterBuilder() { Text = $"{player.jogador}" },
				Color = GerarCorVida(player.vida, player.vidamax)
			}.Build();
			return embed;
		}

		public static string GerarMod(int valor)
		{
			if (valor <= 1) return "-5";
			else if (valor <= 3) return "-4";
			else if (valor <= 5) return "-3";
			else if (valor <= 7) return "-2";
			else if (valor <= 9) return "-1";
			else if (valor <= 11) return "+0";
			else if (valor <= 13) return "+1";
			else if (valor <= 15) return "+2";
			else if (valor <= 17) return "+3";
			else if (valor <= 19) return "+4";
			else if (valor <= 21) return "+5";
			else if (valor <= 23) return "+6";
			else if (valor <= 25) return "+7";
			else if (valor <= 27) return "+8";
			else if (valor <= 29) return "+9";
			else return "+10";
		}
		public static Color GerarCorVida(int vida, int vidamax)
		{
			Color verde = new Color(87, 242, 135);
			Color vermelho = new Color(237, 66, 69);
			Color laranja = new Color(230, 126, 34);
			Color branco = new Color(255, 255, 255);
			Color preto = new Color(35, 39, 42);

			return vida == vidamax ? branco :
				vida > vidamax * 0.75f ? verde :
				vida > vidamax * 0.25f ? laranja :
				vida > -4 ? vermelho : preto;
		}
		public static int GerarNivel(int xp)
		{
			return
				xp < 300 ? 1 :
				xp < 900 ? 2 :
				xp < 2700 ? 3 :
				xp < 6500 ? 4 :
				xp < 14000 ? 5 :
				xp < 23000 ? 6 :
				xp < 34000 ? 7 :
				xp < 48000 ? 8 :
				xp < 64000 ? 9 :
				xp < 85000 ? 10 :
				xp < 100000 ? 11 :
				xp < 120000 ? 12 :
				xp < 140000 ? 13 :
				xp < 165000 ? 14 :
				xp < 195000 ? 15 :
				xp < 225000 ? 16 :
				xp < 265000 ? 17 :
				xp < 305000 ? 18 :
				xp < 355000 ? 19 :
				20;
		}
	}
}
