using Discord;
using Newtonsoft.Json;
using RPGbot.db;
using RPGbot.Racas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core.Tokens;

namespace RPGbot.Classes
{
	public class PlayerResponse
	{
		public static Embed GerarFicha(Personagem personagem)
		{
			Classe classePlayer = new DBclasse().Get(personagem.Classe);
			Raca racaPlayer = new DBraca().Get(personagem.Raca);

			EmbedBuilder embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   {personagem.Vida}/{personagem.VidaMax}hp   {GerarCA(personagem)} CA" },
				Description = $"{(personagem.Genero == "Feminino" ? classePlayer.Fname : classePlayer.Mname)} - {personagem.Posicao}       {(personagem.Genero == "Feminino" ? racaPlayer.Fname : racaPlayer.Mname)}   -   {personagem.XP}/{niveisXP[GerarNivel(personagem.XP) - 1]}xp",
				Footer = new EmbedFooterBuilder() { Text = $"💰 {GerarSaldo(personagem.Saldo)}   -   {personagem.Jogador}" },
				Fields = new List<EmbedFieldBuilder>()
				{
					new EmbedFieldBuilder()
					{
						Name = $"Proficiência   +{proficiencia[GerarNivel(personagem.XP) - 1]}",
						Value = $"```" +
						$"FOR {GerarMod(personagem.Forca)}   INT {GerarMod(personagem.Inteligencia)}\n" +
						$"DES {GerarMod(personagem.Destreza)}   SAB {GerarMod(personagem.Sabedoria)}\n" +
						$"CON {GerarMod(personagem.Constituicao)}   CAR {GerarMod(personagem.Carisma)}```",
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
			};

			return embed.Build();
		}
		public static Embed GerarPericias(Personagem personagem)
		{
			string txt = "```md\n> Perícias do personagem\n";
			if (personagem.Pericias != null)
				foreach (string pericia in personagem.Pericias)
					txt += $"- {new DBpericia().Get(pericia).Nome}\n";

			EmbedBuilder embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   {personagem.Vida}/{personagem.VidaMax}hp" },
				Description = $"{txt}```",
				Footer = new EmbedFooterBuilder() { Text = $"🌟 Nível {GerarNivel(personagem.XP)}   -   {personagem.Jogador}" },
				Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
			};

			return embed.Build();
		}

		public static float pesoMod = 7.5f;
		public static Embed GerarInventario(Personagem personagem)
		{
			List<string> inventarioStrings = personagem.Inventario;

			string itensTxt = "";
			string itensTxt2 = "";
			string armasTxt = "";
			string armasTxt2 = "";
			string armadurasTxt = "";
			string armadurasTxt2 = "";
			List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText("../../db/g_data/itens.json"));
			for (int i = 0; i < inventarioStrings.Count; i++)
			{
				foreach (Item item in itemList)
				{
					if (FormatID(item.Name) == FormatID(inventarioStrings[i]))
					{
						if (item.Dano == string.Empty)
							if (item.Defesa == 0)
								if (itensTxt.Length <= 950)
									itensTxt += $"• {item.Name}   {(item.Propriedades != "" ? $"{item.Propriedades}   " : "")}{item.Peso}kg\n";
								else
									itensTxt2 += $"• {item.Name}   {(item.Propriedades != "" ? $"{item.Propriedades}   " : "")}{item.Peso}kg\n";

							else
							{
								if (armadurasTxt.Length <= 950)
									armadurasTxt += $"{(FormatID(item.Name) == personagem.Armadura || FormatID(item.Name) == personagem.Escudo ? "-   »" : "•")} {item.Name}   {item.Peso}kg   {item.Defesa} CA\n";
								else
									armadurasTxt2 += $"{(FormatID(item.Name) == personagem.Armadura || FormatID(item.Name) == personagem.Escudo ? "-   »" : "•")} {item.Name}   {item.Peso}kg   {item.Defesa} CA\n";
							}
						else
							if (armasTxt.Length <= 950)
							armasTxt += $"• {item.Name}   {item.Peso}kg   {item.Dano}\n";
						else
							armasTxt2 += $"• {item.Name}   {item.Peso}kg   {item.Dano}\n";
					}
				}
			}

			float pesoInv = personagem.Forca * pesoMod + personagem.Saldo * 0.01f + GetMochila(personagem.Inventario);

			EmbedBuilder embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   -   Inventário {GerarPesoInventario(personagem)}/{pesoInv}kg" },
				Footer = new EmbedFooterBuilder() { Text = $"💰 {GerarSaldo(personagem.Saldo)}   -   {personagem.Jogador}" },
				Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
			};

			if (armasTxt != "") embed.AddField("Armas", armasTxt);
			if (armasTxt2 != "") embed.AddField("Armas", armasTxt2);
			if (armadurasTxt != "") embed.AddField($"Armaduras   {GerarCA(personagem)} CA", armadurasTxt);
			if (armadurasTxt2 != "") embed.AddField($"Armaduras   {GerarCA(personagem)} CA total", armadurasTxt2);
			if (itensTxt != "") embed.AddField("Itens", itensTxt);
			if (itensTxt2 != "") embed.AddField("Itens", itensTxt2);

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
				Title = $"Magias   {magias.Count}/{new DBclasse().Get(personagem.Classe).Magias[GerarNivel(personagem.XP) - 1]}",
				Description = magiasTxt,
				Footer = new EmbedFooterBuilder() { Text = $"🧙 {(personagem.Genero == "Feminino" ? classePlayer.Fname : classePlayer.Mname)}   -   {personagem.Jogador}" },
				Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
			};

			return embed.Build();
		}
		public static Embed GerarAllPericias(List<Pericia> list)
		{
			list = list.OrderByDescending(x => (int)(x.Atributo)).ToList();

			string txt = "";
			foreach (Pericia obj in list)
				txt += $"```md\n" +
					$"> {obj.Nome}   ({obj.Atributo})\n" +
					//$"# {obj.Atributo}\n" +
					$"{obj.Descricao}```";

			EmbedBuilder embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"Perícias" },
				Description = txt,
				Color = 0x57F287
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
				.WithDescription($"{item.Peso}kg   |   {item.Preco} moedas {modificador}")
				.WithFooter(new EmbedFooterBuilder() { Text = $"{item.Tipo}" });
			embed.Color = item.Dano != string.Empty ? new Color(0xED4245) : item.Defesa != 0 ? new Color(0x57F287) : new Color(0x3498DB);

			if (danodefesa != string.Empty)
				embed.AddField(danodefesa, item.Propriedades);

			return embed.Build();
		}

		public static float GetMochila(List<string> inventario)
		{
			float mochila = 0f;

			List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText("../../db/g_data/itens.json"));
			for (int i = 0; i < inventario.Count; i++)
			{
				foreach (Item item in itemList)
				{
					if (FormatID(item.Name) == FormatID(inventario[i]))
					{
						if (item.Tipo == "Mochila")
						{
							string txt = Regex.Replace(item.Propriedades, @"[\+kg]*", "");
							mochila = float.Parse(txt);
							break;
						}
					}
				}
			}
			return mochila;
		}

		public static int GerarCA(Personagem personagem)
		{
			if (personagem.Armadura == null || personagem.Armadura == "")
			{
				if (personagem.Escudo == null || personagem.Escudo == "")
					return 10 + int.Parse(Math.Floor((float.Parse(personagem.Destreza.ToString()) - 10) / 2).ToString());
				else
					return 10 + int.Parse(Math.Floor((float.Parse(personagem.Destreza.ToString()) - 10) / 2).ToString()) + new DBitem().Get(personagem.Escudo).Defesa;
			}
			else
			{
				if (personagem.Escudo == null || personagem.Escudo == "")
					return new DBitem().Get(personagem.Armadura).Defesa;
				else
					return new DBitem().Get(personagem.Armadura).Defesa + new DBitem().Get(personagem.Escudo).Defesa;
			}
		}
		public static string GerarMod(int valor)
		{
			double resultado = Math.Floor((float.Parse(valor.ToString()) - 10) / 2);
			return resultado >= 0 ? $"+{resultado}" : $"-{Math.Abs(resultado)}";
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
		public static int[] proficiencia = new int[]
		{
			2,2,2,2,3,3,3,3,4,4,4,4,5,5,5,5,6,6,6,6
		};

		public enum Atributos { Força, Destreza, Constituição, Inteligência, Sabedoria, Carisma }

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

			text = Regex.Replace(text.Normalize(NormalizationForm.FormD), @"[-/^\s]", "");
			var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
			return Regex.Replace(new string(chars).Normalize(NormalizationForm.FormC).ToLower(), @"\s*\((^\)]+)\)", "");
		}
	}
}
