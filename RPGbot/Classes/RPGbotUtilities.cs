﻿using Discord;
using RPGbotAPI.Controllers;
using RPGbotAPI.Models;
using RPGbotAPI.Services;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RPGbot.Classes
{
	public class RPGbotUtilities
	{
		public static Embed GerarFicha(Personagem personagem)
		{
			ClassesController classesController = new(new ClasseService("Classes"));
			RacasController racasController = new(new RacaService("Racas"));

			Classe classePlayer = classesController.Get(personagem.Classe).Result;
			Raca racaPlayer = racasController.Get(personagem.Raca).Result;

			EmbedBuilder embed = new()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   {personagem.Vida}/{personagem.VidaMax}hp   {GerarCA(personagem)} CA" },
				Description = $"{(personagem.Genero == "Feminino" ? classePlayer.Fname : classePlayer.Mname)}   -   {personagem.Posicao}       {(personagem.Genero == "Feminino" ? racaPlayer.Fname : racaPlayer.Mname)}   -   {personagem.XP}/{niveisXP[GerarNivel(personagem.XP) - 1]}xp",
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
						Value = $"{personagem.Peso}kg   {personagem.Altura}cm\n{personagem.Idade} anos de idade\n{personagem.Genero} - {personagem.Sexualidade}\n**{(personagem.Exaustão > 0 ? $"Exaustão {GerarExaustão(personagem.Exaustão)}" : "Descansado")}**",
						IsInline = true
					}
				},
				Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
			};

			return embed.Build();
		}
		public static Embed GerarPericias(Personagem personagem)
		{
			PericiasController periciasController = new(new PericiaService("Pericias"));

			string txt = "```md\n> Perícias do personagem\n";
			if (personagem.Pericias != null)
				foreach (string pericia in personagem.Pericias)
					txt += $"- {periciasController.Get(pericia).Result.Nome}\n";

			EmbedBuilder embed = new()
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
			ItensController itensController = new(new ItemService("Itens"));

			List<string> inventarioStrings = personagem.Inventario!;

			string itensTxt = "";
			string itensTxt2 = "";
			string armasTxt = "";
			string armasTxt2 = "";
			string armadurasTxt = "";
			string armadurasTxt2 = "";
			List<Item> itemList = itensController.GetAll().Result;
			for (int i = 0; i < inventarioStrings.Count; i++)
			{
				foreach (Item item in itemList)
				{
					if (FormatID(item.Name) == FormatID(inventarioStrings[i]))
					{
						if (item.Dano == string.Empty)
							if (item.Defesa == 0)
								if (itensTxt.Length <= 950)
									itensTxt += $"* {item.Name}   {(item.Propriedades != "" ? $"{item.Propriedades}   " : "")}{item.Peso}kg\n";
								else
									itensTxt2 += $"* {item.Name}   {(item.Propriedades != "" ? $"{item.Propriedades}   " : "")}{item.Peso}kg\n";

							else
							{
								if (armadurasTxt.Length <= 950)
									armadurasTxt += $"{(FormatID(item.Name) == personagem.Armadura || FormatID(item.Name) == personagem.Escudo ? ">" : "*")} {item.Name}   {item.Peso}kg   {item.Defesa} CA\n";
								else
									armadurasTxt2 += $"{(FormatID(item.Name) == personagem.Armadura || FormatID(item.Name) == personagem.Escudo ? ">" : "*")} {item.Name}   {item.Peso}kg   {item.Defesa} CA\n";
							}
						else
							if (armasTxt.Length <= 950)
							armasTxt += $"* {item.Name}   {item.Peso}kg   {item.Dano}\n";
						else
							armasTxt2 += $"* {item.Name}   {item.Peso}kg   {item.Dano}\n";
					}
				}
			}

			float pesoInv = personagem.Forca * pesoMod + GetMochila(personagem.Inventario!);

			EmbedBuilder embed = new()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   -   Inventário {GerarPesoInventario(personagem)}/{pesoInv:N2}kg" },
				Footer = new EmbedFooterBuilder() { Text = $"💰 {GerarSaldo(personagem.Saldo)}   -   {personagem.Jogador}" },
				Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
			};

			if (armasTxt != "") embed.AddField("Armas", $"```md\n{armasTxt}```");
			if (armasTxt2 != "") embed.AddField("Armas", $"```md\n{armasTxt2}```");
			if (armadurasTxt != "") embed.AddField($"Armaduras   {GerarCA(personagem)} CA", $"```md\n{armadurasTxt}```");
			if (armadurasTxt2 != "") embed.AddField($"Armaduras   {GerarCA(personagem)} CA total", $"```md\n{armadurasTxt2}```");
			if (itensTxt != "") embed.AddField("Itens", $"```md\n{itensTxt}```");
			if (itensTxt2 != "") embed.AddField("Itens", $"```md\n{itensTxt2}```");

			return embed.Build();
		}
		public static Embed GerarMagias(List<string> magias, Personagem personagem)
		{
			MagiasController magiasController = new(new MagiaService("Magias"));
			ClassesController classesController = new(new ClasseService("Classes"));

			string magiasTxt = "";
			for (int i = 0; i < magias.Count; i++)
				magiasTxt += $"• {magiasController.Get(magias[i]).Result.Name} \n";

			Classe classePlayer = classesController.Get(personagem.Classe).Result;

			EmbedBuilder embed = new()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}" },
				Title = $"Magias   {magias.Count}/{classesController.Get(personagem.Classe).Result.Magias![GerarNivel(personagem.XP) - 1]}",
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

			EmbedBuilder embed = new()
			{
				Author = new EmbedAuthorBuilder() { Name = $"Perícias" },
				Description = txt,
				Color = 0x57F287
			};

			return embed.Build();
		}
		public static Embed GerarMagia(string nome)
		{
			MagiasController magiasController = new(new MagiaService("Magias"));
			Magia magia = magiasController.Get(nome).Result;

			EmbedBuilder embed = new()
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
			ItensController itensController = new(new ItemService("Itens"));

			List<Item> itemList = itensController.GetAll().Result;
			if (itemList.Find(e => FormatID(e.Name) == nome) == null)
			{
				return null!;
			}

			Item item = itensController.Get(nome).Result;

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

			ItensController itensController = new(new ItemService("Itens"));

			List<Item> itemList = itensController.GetAll().Result;
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

		public static string GerarExaustão(int valor)
		{
			string resultado = "";
			for (int i = 0; i < valor; i++) resultado += " ● ";
			if (valor != 5)
				for (int i = valor; i < 5; i++) resultado += " ○ ";

			return resultado;
		}
		public static int GerarCA(Personagem personagem)
		{
			ItensController itensController = new(new ItemService("Itens"));

			if (personagem.Armadura == null || personagem.Armadura == "")
			{
				if (personagem.Escudo == null || personagem.Escudo == "")
					return 10 + int.Parse(Math.Floor((float.Parse(personagem.Destreza.ToString()) - 10) / 2).ToString());
				else
					return 10 + int.Parse(Math.Floor((float.Parse(personagem.Destreza.ToString()) - 10) / 2).ToString()) + itensController.Get(personagem.Escudo).Result.Defesa;
			}
			else
			{
				if (personagem.Escudo == null || personagem.Escudo == "")
					return itensController.Get(personagem.Armadura).Result.Defesa;
				else
					return itensController.Get(personagem.Armadura).Result.Defesa + itensController.Get(personagem.Escudo).Result.Defesa;
			}
		}
		public static string GerarMod(int valor)
		{
			double resultado = Math.Floor((float.Parse(valor.ToString()) - 10) / 2);
			return resultado >= 0 ? $"+{resultado}" : $"-{Math.Abs(resultado)}";
		}
		public static Color GerarCorVida(int vida, int vidamax)
		{
			Color verde = new(87, 242, 135);
			Color vermelho = new(237, 66, 69);
			Color laranja = new(230, 126, 34);
			Color branco = new(255, 255, 255);
			Color preto = new(35, 39, 42);

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

			ItensController itensController = new(new ItemService("Itens"));

			float peso = 0;
			foreach (string item in personagem.Inventario)
			{
				peso += itensController.Get(item).Result.Peso;
			}

			return peso + personagem.Saldo * 0.01f;
		}

		public static string FormatID(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return text;

			text = Regex.Replace(text.Normalize(NormalizationForm.FormD), @"[-/^\s]", "");
			var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
			return Regex.Replace(new string(chars).Normalize(NormalizationForm.FormC).ToLower(), @"\s*\([^\)]+\)", "");
		}
	}
}