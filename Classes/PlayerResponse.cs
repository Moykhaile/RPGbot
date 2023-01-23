using Discord;
using RPGbot.Racas;
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
			PlayerClass classePlayer = new PlayerClass().GetClass(player.Classe);
			PlayerRaca racaPlayer = new PlayerRaca().GetRaca(player.Raca);

			Embed embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{player.Nome}   {player.Vida}/{player.VidaMax}hp" },
				Description = $"{(player.Genero == "Feminino" ? racaPlayer.Fname : racaPlayer.Mname)}   -   {(player.Genero == "Feminino" ? classePlayer.Fname : classePlayer.Mname)}   -   {player.Posicao}   -   {player.XP}/{niveisXP[GerarNivel(player.XP) - 1]}xp",
				Footer = new EmbedFooterBuilder() { Text = $"💰 {player.Saldo}po   -   {player.Jogador}" },
				Fields = new List<EmbedFieldBuilder>()
				{
					new EmbedFieldBuilder()
					{
						Name = "Atributos",
						Value = $"```FOR {GerarMod(player.Forca)}   DES {GerarMod(player.Destreza)}\nINT {GerarMod(player.Inteligencia)}   CON {GerarMod(player.Constituicao)}\nSAB {GerarMod(player.Sabedoria)}   CAR {GerarMod(player.Carisma)}```",
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
						Name = $"Nível {GerarNivel(player.XP)}",
						Value = $"{player.Peso}kg   {player.Altura}cm\n{player.Idade} anos de idade\n{player.Genero} - {player.Sexualidade}",
						IsInline = true
					}
				},
				Color = GerarCorVida(player.Vida, player.VidaMax)
			}.Build();

			return embed;
		}
		public static Embed GerarValor(string nomev, Player player, int newv, int oldv, int v)
		{
			Embed embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{player.Nome}" },
				Description = $"{nomev}:   {oldv} {(v < 0 ? '-' : '+')} {Math.Abs(v)} =   **{newv}**",
				Footer = new EmbedFooterBuilder() { Text = $"{player.Jogador}" },
				Color = GerarCorVida(player.Vida, player.VidaMax)
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

		public static int[] niveisXP = new int[]
		{
			300,900,2700,6500,14000,23000,34000,48000,64000,85000,100000,120000,140000,165000,195000,225000,265000,305000,355000
		};

		public static int GerarNivel(int xp)
		{
			var nivel = 1;

			for (int i = 0; i < niveisXP.Length; i++)
			{
				if (niveisXP[i] < xp)
				{
					nivel += 1;
				}
			}

			return nivel;
		}
	}
}
