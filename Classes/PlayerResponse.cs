using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGbot.db;
using RPGbot.Racas;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RPGbot.Classes
{
	public class PlayerResponse
	{
		public static Embed GerarFicha(Personagem personagem)
		{
			Classe classePlayer = new DBclasse().Get(personagem.Classe);
			Raca racaPlayer = new DBraca().Get(personagem.Raca);

			Embed embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   {personagem.Vida}/{personagem.VidaMax}hp" },
				Description = $"{(personagem.Genero == "Feminino" ? racaPlayer.Fname : racaPlayer.Mname)} - {(personagem.Genero == "Feminino" ? classePlayer.Fname : classePlayer.Mname)}       {personagem.Posicao}   -   {personagem.XP}/{niveisXP[GerarNivel(personagem.XP) - 1]}xp",
				Footer = new EmbedFooterBuilder() { Text = $"💰 {GerarSaldo(personagem.Saldo)}   -   {personagem.Jogador}" },
				Fields = new List<EmbedFieldBuilder>()
				{
					new EmbedFieldBuilder()
					{
						Name = "Atributos",
						Value = $"```FOR {GerarMod(personagem.Forca)}   DES {GerarMod(personagem.Destreza)}\nINT {GerarMod(personagem.Inteligencia)}   CON {GerarMod(personagem.Constituicao)}\nSAB {GerarMod(personagem.Sabedoria)}   CAR {GerarMod(personagem.Carisma)}```",
						IsInline = true
					},
					new EmbedFieldBuilder()
					{
						Name = $"Nível {GerarNivel(personagem.XP)}",
						Value = $"{personagem.Peso}kg   {personagem.Altura}cm\n{personagem.Idade} anos de idade\n{personagem.Genero} - {personagem.Sexualidade}",
						IsInline = true
					}
				},
				Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
			}.Build();

			return embed;
		}
		public static float pesoMod = 7.5f;
		public static Embed GerarInventario(Personagem personagem)
		{
			List<string> inventarioStrings = personagem.Inventario;

			string itensTxt = "";
			string armasTxt = "";
			string armadurasTxt = "";
			int CAsomado = 0;
			List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText("../../db/g_data/itens.json"));
			for (int i = 0; i < inventarioStrings.Count; i++)
			{
				foreach (Item item in itemList)
				{
					if (FormatID(item.Name) == FormatID(inventarioStrings[i]))
					{
						if (item.Dano == string.Empty)
							if (item.Defesa == 0)
								itensTxt += $"• {item.Name}   {item.Peso}kg\n";
							else
							{
								armadurasTxt += $"• {item.Name}   {item.Peso}kg   {item.Defesa} CA\n";
								CAsomado += item.Defesa;
							}
						else
							armasTxt += $"• {item.Name}   {item.Peso}kg   {item.Dano}\n";
					}
				}
			}

			EmbedBuilder embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   -   Inventário {GerarPesoInventario(personagem)}/{personagem.Forca * pesoMod}kg" },
				Footer = new EmbedFooterBuilder() { Text = $"💰 {GerarSaldo(personagem.Saldo)}   -   {personagem.Jogador}" },
				Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
			};

			if (armasTxt != "") embed.AddField("Armas", armasTxt);
			if (armadurasTxt != "") embed.AddField($"Armaduras   {CAsomado} CA total", armadurasTxt);
			if (itensTxt != "") embed.AddField("Itens", itensTxt);

			return embed.Build();
		}
		public static Embed GerarMagias(List<string> magias, Personagem personagem)
		{
			string magiasTxt = "";
			for (int i = 0; i < magias.Count; i++)
				magiasTxt += $"• {new DBmagia().Get(magias[i]).Name} \n";

			Classe classePlayer = new DBclasse().Get(personagem.Classe);

			EmbedBuilder embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}" },
				Title = $"Magias   {magias.Count}/{niveisMagias[GerarNivel(personagem.XP) - 1]}",
				Description = magiasTxt,
				Footer = new EmbedFooterBuilder() { Text = $"🧙 {(personagem.Genero == "Feminino" ? classePlayer.Fname : classePlayer.Mname)}   -   {personagem.Jogador}" },
				Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
			};

			return embed.Build();
		}

		public static Embed GerarMagia(string nome)
		{
			Magia magia = new DBmagia().Get(nome);

			EmbedBuilder embed = new EmbedBuilder()
			{
				Title = magia.Name,
				Description = magia.Description,
				Footer = new EmbedFooterBuilder() { Text = "" },
				Fields =
				{
					new EmbedFieldBuilder()
					{
						Name = "Lançamento",
						Value = magia.Casting
					},
					new EmbedFieldBuilder()
					{
						Name = "Alcance",
						Value = magia.Range
					},
					new EmbedFieldBuilder()
					{
						Name = "Componentes",
						Value = magia.Components
					},
					new EmbedFieldBuilder()
					{
						Name = "Duração",
						Value = magia.Duration
					}
				}
			};

			return embed.Build();
		}
		public static Embed GerarItem(string nome)
		{
			string itensstring = File.ReadAllText("../../db/g_data/itens.json");
			List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(itensstring);
			if (itemList.Find(e => FormatID(e.Name) == nome) == null)
			{
				return null;
			}

			Item item = itemList.Find(e => FormatID(e.Name) == nome);

			string modificador = item.ModNome != string.Empty ? $"\n*Modificador: {item.ModNome}*" : "";
			string danodefesa = item.Dano != string.Empty ? $"*{item.Dano}*" : item.Defesa != 0 ? $"*{item.Defesa} CA*" : "";

			EmbedBuilder embed = new EmbedBuilder()
				.WithTitle(item.Name)
				.WithDescription($"{item.Peso}kg   |   {item.Preco}po {modificador}")
				.WithFooter(new EmbedFooterBuilder() { Text = $"{item.Tipo}" });
			embed.Color = item.Dano != string.Empty ? new Color(0xED4245) : item.Defesa != 0 ? new Color(0x57F287) : new Color(0x3498DB);

			if (danodefesa != string.Empty)
				embed.AddField(danodefesa, item.Propriedades);

			return embed.Build();
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
		public static int[] niveisMagias = new int[]
		{
			4,5,6,7,8,9,10,11,12,14,15,15,16,18,19,19,20,22,22,22
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

		public static string GerarSaldo(float saldo)
		{
			return string.Format("{0:F2}", saldo);
		}

		public static float GerarPesoInventario(Personagem personagem)
		{
			if (personagem.Inventario == null || personagem.Inventario.Count <= 0)
				return 0;

			float peso = 0;
			foreach (string item in personagem.Inventario)
			{
				peso += new DBitem().Get(item).Peso;
			}

			return peso;
		}

		public static string FormatID(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return text;

			text = text.Normalize(NormalizationForm.FormD);
			var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
			return Regex.Replace(new string(chars).Normalize(NormalizationForm.FormC).ToLower(), @"\s*\(([^\)]+)\)", "");
		}
	}
}
