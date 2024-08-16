using Discord;
using RPGbotAPI.Controllers;
using RPGbotAPI.Models;
using RPGbotAPI.Services;
using RPGbotLib;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RPGbot.Classes
{
	public class RPGbotUtilities
	{
		public static Embed GerarFicha(Personagem personagem)
		{
			ClassesController classesController = new(new());
			RacasController racasController = new(new());
			string classe = personagem.Genero == "Feminino" ? classesController.Get(personagem.Classe).Result.Fname : classesController.Get(personagem.Classe).Result.Mname;
			string raca = personagem.Genero == "Feminino" ? racasController.Get(personagem.Raca).Result.Fname : racasController.Get(personagem.Raca).Result.Mname;

			EmbedBuilder embed = new()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   {personagem.Vida}/{personagem.VidaMax}hp   {GerarCA(personagem)} CA" },
				Description = $"{classe}   -   {personagem.Posicao}       {raca}   -   {personagem.XP}/{NiveisXP[personagem.Nivel - 1]}xp",
				Footer = new EmbedFooterBuilder() { Text = $"💰 {GerarSaldo(personagem.Saldo)}   -   {personagem.Jogador}" },

				Fields = new List<EmbedFieldBuilder>()
				{
					new EmbedFieldBuilder()
					{
						Name = $"Proficiência   +{Proficiencia[personagem.Nivel -1 ]}",
						Value = $"```" +
						$"FOR {GerarMod(personagem.Forca)}   INT {GerarMod(personagem.Inteligencia)}\n" +
						$"DES {GerarMod(personagem.Destreza)}   SAB {GerarMod(personagem.Sabedoria)}\n" +
						$"CON {GerarMod(personagem.Constituicao)}   CAR {GerarMod(personagem.Carisma)}```",
						IsInline = true
					},
					new EmbedFieldBuilder()
					{
						Name = $"Nível {personagem.Nivel}",
						Value = $"{personagem.Peso}kg   {personagem.Altura}cm\n{personagem.Idade} anos de idade\n{personagem.Genero} - {personagem.Sexualidade}\n**{(personagem.Exaustão > 0 ? $"Exaustão {GerarExaustão(personagem.Exaustão)}" : "Descansado")}**",
						IsInline = true
					}
				},
				Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
			};

			if (personagem.XP >= NiveisXP[personagem.Nivel - 1])
				embed.Description += "\n\n**Passe de nível com ``/levelup``!**\n";

			return embed.Build();
		}
		public static Embed GerarPericias(Personagem personagem)
		{
			string txt = "```md\n> Perícias do personagem\n";
			if (personagem.Pericias != null)
				foreach (string pericia in personagem.Pericias)
					txt += $"- {pericia}\n";

			EmbedBuilder embed = new()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   {personagem.Vida}/{personagem.VidaMax}hp" },
				Description = $"{txt}```",
				Footer = new EmbedFooterBuilder() { Text = $"🌟 Nível {personagem.Nivel}   -   {personagem.Jogador}" },
				Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
			};

			return embed.Build();
		}

		public static float PesoMod { get; } = 7.5f;
		public static Embed GerarInventario(Personagem personagem)
		{
			ItensController itensController = new(new ItemService());

			List<Item> inventarioStrings = personagem.Inventario!;

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
					if (FormatID(item.Name) == FormatID(inventarioStrings[i].Id!))
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
									armadurasTxt += $"{(item == personagem.Armadura || item == personagem.Escudo ? ">" : " * ")} {item.Name}   {item.Peso}kg   {item.Defesa} CA\n";
								else
									armadurasTxt2 += $"{(item == personagem.Armadura || item == personagem.Escudo ? ">" : " * ")} {item.Name}   {item.Peso}kg   {item.Defesa} CA\n";
							}
						else
							if (armasTxt.Length <= 950)
							armasTxt += $"* {item.Name}   {item.Peso}kg   {item.Dano}\n";
						else
							armasTxt2 += $"* {item.Name}   {item.Peso}kg   {item.Dano}\n";
					}
				}
			}

			float pesoInv = personagem.Forca * PesoMod + GetMochila(personagem.Inventario!);

			EmbedBuilder embed = new()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   -   Inventário {GerarPesoInventario(personagem):N2}/{pesoInv:N2}kg" },
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
			ClassesController classesController = new(new());
			string classe = personagem.Genero == "Feminino" ? classesController.Get(personagem.Classe)!.Result!.Fname : classesController.Get(personagem.Classe)!.Result!.Mname;

			string magiasTxt = "";
			for (int i = 0; i < magias.Count; i++)
				magiasTxt += $"• {magias[i]} \n";

			if (personagem.Magias.Count == 0)
			{
				EmbedBuilder _embed = new()
				{
					Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}" },
					Title = $"Seu personagem não tem magias!",
					Footer = new EmbedFooterBuilder() { Text = $"🧙 {classe}   -   {personagem.Jogador}" },
					Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
				};
				return _embed.Build();
			}

			EmbedBuilder embed = new()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}" },
				Title = $"Magias   {magias.Count}/{classesController.Get(personagem.Classe)!.Result!.Magias![personagem.Nivel - 1]}",
				Description = magiasTxt,
				Footer = new EmbedFooterBuilder() { Text = $"🧙 {classe}   -   {personagem.Jogador}" },
				Color = GerarCorVida(personagem.Vida, personagem.VidaMax)
			};

			return embed.Build();
		}
		public static Embed GerarAllPericias(List<Pericia> list)
		{
			list = list.OrderByDescending(x => (int)x.Atributo).ToList();

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
			MagiasController magiasController = new(new MagiaService());
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
			ItensController itensController = new(new ItemService());

			List<Item> itemList = itensController.GetAll().Result;
			Item item = itemList.Find(x => x.Id == FormatID(nome))!;
			if (item == null)
			{
				return null!;
			}

			string modificador = item.ModNome != string.Empty ? $"\n*Modificador: {item.ModNome}*" : "";
			string danodefesa = item.Dano != string.Empty ? $"*{item.Dano}*" : item.Defesa != 0 ? $"*{item.Defesa} CA*" : "";

			EmbedBuilder embed = new EmbedBuilder()
				.WithTitle(item.Name)
				.WithDescription($"{item.Peso}kg   |   {item.Preco}₹ {modificador}")
				.WithFooter(new EmbedFooterBuilder() { Text = $"{item.Tipo}" });
			embed.Color = item.Dano != string.Empty ? new Color(0xED4245) : item.Defesa != 0 ? new Color(0x57F287) : new Color(0x3498DB);

			if (danodefesa != string.Empty)
				embed.AddField(danodefesa, item.Propriedades);

			return embed.Build();
		}

		public static Embed GerarPets(Personagem personagem)
		{
			List<Pet> pets = personagem.Pets!;

			var embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   {personagem.Vida}/{personagem.VidaMax}hp" },
			};

			foreach (Pet pet in pets)
			{
				string ataques = "";
				if (pet.Ataques != null && pet.Ataques.Count != 0)
				{
					ataques += "\n";
					foreach (PetAtaque ataque in pet.Ataques!)
					{
						ataques += $"**{ataque.Nome}** → {ataque.Dano}\n";
					}
					ataques += "\n";
				}
				string magias = "";
				if (pet.Magias != null && pet.Magias!.Count != 0)
				{
					magias += "\nMagias:";
					foreach (string magia in pet.Magias!)
					{
						magias += $"**{magia}**   →   ";
					}
					magias += "\n";
				}
				string inventario = "";
				if (pet.Inventario != null && pet.Inventario.Count != 0)
				{
					inventario += $"\n**{pet.Inventario.Count} itens**\n";
				}

				embed.AddField($"{pet.Nome}   (*{pet.Especie}*)   {(pet.Genero == Utilities.PetGenero.Feminino ? "♀️" : "♂️")}", $"{pet.Vida}/{pet.MaxVida}hp   -   {pet.Defesa}CA\n{GerarPesoInventario(pet):N2}/{pet.TamanhoMochila}kg\n{ataques}{magias}{inventario}", true);
			}

			return embed.Build();
		}

		public static Embed GerarPets(Personagem personagem, bool mestre)
		{
			if (mestre == false) return null!;

			List<Pet> pets = personagem.Pets!;

			var embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"{personagem.Nome}   {personagem.Vida}/{personagem.VidaMax}hp" },
			};

			foreach (Pet pet in pets)
			{
				string ataques = "";
				if (pet.Ataques != null && pet.Ataques.Count != 0)
				{
					ataques += "\n";
					foreach (PetAtaque ataque in pet.Ataques!)
					{
						ataques += $"**{ataque.Nome}** → {ataque.Dano}\n";
					}
					ataques += "\n";
				}
				string magias = "";
				if (pet.Magias != null && pet.Magias!.Count != 0)
				{
					magias += "\nMagias:";
					foreach (string magia in pet.Magias!)
					{
						magias += $"**{magia}**   →   ";
					}
					magias += "\n";
				}
				string inventario = "";
				if (pet.Inventario != null && pet.Inventario!.Count != 0)
				{
					List<Item> itens = pet.Inventario.OrderByDescending(x => x.Tipo).ToList();
					inventario += "\n";
					foreach (Item item in pet.Inventario)
					{
						inventario += $"{item.Name}   {item.Peso}kg   {(item.Dano != "" ? item.Dano : item.Defesa > 0 ? item.Defesa + " CA" : "")}\n";
					}
					inventario += "\n";
				}

				embed.AddField($"{pet.Nome}   (*{pet.Especie}*)   {(pet.Genero == Utilities.PetGenero.Feminino ? "♀️" : "♂️")}", $"{pet.Vida}/{pet.MaxVida}hp   -   {pet.Defesa}CA\n{GerarPesoInventario(pet):N2}/{pet.TamanhoMochila}kg\n{ataques}{magias}{inventario}", true);
			}

			return embed.Build();
		}

		public static Embed GerarPet(Pet pet, string nomePersonagem)
		{
			string inventario = "";
			pet.Inventario ??= new List<Item>();
			List<Item> itens = pet.Inventario.OrderByDescending(x => x.Tipo).ToList();
			foreach (Item item in pet.Inventario)
			{
				inventario += $"{item.Name}   {item.Peso}kg   {(item.Dano != "" ? item.Dano : item.Defesa > 0 ? item.Defesa + " CA" : "")}\n";
			}

			string ataquesTxt = "";
			pet.Ataques ??= new List<PetAtaque>();
			List<PetAtaque> ataques = pet.Ataques.OrderByDescending(x => x.Nome).ToList();
			foreach (PetAtaque ataque in pet.Ataques)
			{
				ataquesTxt += $"{ataque.Nome}   →   {ataque.Dano}\n";
			}

			string magiasTxt = "";
			pet.Magias ??= new List<string>();
			List<string> magias = pet.Magias.OrderByDescending(x => x).ToList();
			foreach (string magia in pet.Magias)
			{
				magiasTxt += $"{magia}\n";
			}

			var embed = new EmbedBuilder()
			{
				Title = pet.Nome,
				Description = $"{(pet.Genero == Utilities.PetGenero.Feminino ? "♀️" : "♂️")}   *{pet.Especie}*   {pet.Vida}/{pet.MaxVida}hp",
				Footer = new EmbedFooterBuilder() { Text = nomePersonagem }
			};

			if (inventario != "")
				embed.AddField($"Inventário   ({GerarPesoInventario(pet)}/{pet.TamanhoMochila}kg)", inventario, true);
			if (ataquesTxt != "")
				embed.AddField($"Ataques", ataquesTxt, true);
			if (magiasTxt != "")
				embed.AddField($"Magias", magiasTxt, true);

			return embed.Build();
		}

		public static float GetMochila(List<Item> inventario)
		{
			float mochila = 0f;

			ItensController itensController = new(new ItemService());

			List<Item> itemList = itensController.GetAll().Result;
			for (int i = 0; i < inventario.Count; i++)
			{
				foreach (Item item in itemList)
				{
					if (FormatID(item.Name) == FormatID(inventario[i].Id!))
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
			if (personagem.Armadura == null || personagem.Armadura == null)
			{
				if (personagem.Escudo == null || personagem.Escudo == null)
					return 10 + int.Parse(Math.Floor((float.Parse(personagem.Destreza.ToString()) - 10) / 2).ToString());
				else
					return 10 + int.Parse(Math.Floor((float.Parse(personagem.Destreza.ToString()) - 10) / 2).ToString()) + personagem.Escudo.Defesa;
			}
			else
			{
				if (personagem.Escudo == null || personagem.Escudo == null)
					return personagem.Armadura.Defesa;
				else
					return personagem.Armadura.Defesa + personagem.Escudo.Defesa;
			}
		}
		public static int CalcularMod(int valor)
		{
			return (int)Math.Floor((float)valor - 10) / 2;
		}
		public static string GerarMod(int valor)
		{
			double resultado = CalcularMod(valor);
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

		public static int[] NiveisXP { get; } = new int[]
		{
			300,900,2700,6500,14000,23000,34000,48000,64000,85000,100000,120000,140000,165000,195000,225000,265000,305000,355000
		};
		public static int[] Proficiencia { get; } = new int[]
		{
			2,2,2,2,3,3,3,3,4,4,4,4,5,5,5,5,6,6,6,6
		};

		public enum Atributos { Força, Destreza, Constituição, Inteligência, Sabedoria, Carisma }

		/*public static int GerarNivel(int xp)
		{
			var nivel = 1;

			for (int i = 0; i < NiveisXP.Length; i++)
			{
				if (NiveisXP[i] < xp)
				{
					nivel += 1;
				}
			}

			return nivel;
		}*/

		public static string GerarSaldo(float saldo)
		{
			return string.Format("{0:F2}", saldo);
		}

		public static float GerarPesoInventario(Personagem personagem)
		{
			float peso = 0;
			if (personagem.Inventario != null && personagem.Inventario!.Count != 0)
				foreach (Item item in personagem.Inventario)
				{
					peso += item.Peso;
				}

			return peso + personagem.Saldo * 0.01f;
		}

		public static float GerarPesoInventario(Pet pet)
		{
			float peso = 0;
			if (pet.Inventario != null && pet.Inventario!.Count != 0)
				foreach (Item item in pet.Inventario)
				{
					peso += item.Peso;
				}

			return peso;
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
