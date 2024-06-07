using Discord;
using Discord.Interactions;
using RPGbot.Classes;
using RPGbotAPI.Controllers;
using RPGbotAPI.Models;
using RPGbotAPI.Services;
using RPGbotLib;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace RPGbot.Modules
{
	public class PlayerModule : InteractionModuleBase<SocketInteractionContext>
	{
		readonly PersonagensController personagensController = new(new PersonagemService("Personagens"));
		readonly PericiasController periciasController = new(new PericiaService("Pericias"));
		readonly MagiasController magiasController = new(new MagiaService("Magias"));
		readonly ItensController itensController = new(new ItemService("Itens"));

		#region Ficha
		[SlashCommand("ficha", "Apresenta a ficha do personagem")]
		public async Task Ficha()
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null || personagem.Id == 0)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			var inventarioBtn = new ButtonBuilder()
			{
				CustomId = "inventarioBtn",
				Label = "Ver inventário",
				Style = ButtonStyle.Danger
			};
			var periciasBtn = new ButtonBuilder()
			{
				CustomId = "periciasBtn",
				Label = "Ver perícias",
				Style = ButtonStyle.Success
			};
			var magiasBtn = new ButtonBuilder()
			{
				CustomId = "magiasBtn",
				Label = "Ver magias",
				Style = ButtonStyle.Primary
			};

			var buttonComponent = new ComponentBuilder().WithButton(inventarioBtn).WithButton(periciasBtn).WithButton(magiasBtn);

			await RespondAsync($"Sua ficha de personagem!", ephemeral: true, components: buttonComponent.Build(), embed: RPGbotUtilities.GerarFicha(personagem!));
		}
		[ComponentInteraction("fichaBtn")]
		public async Task FichaButton()
		{
			await DeferAsync();

			var inventarioBtn = new ButtonBuilder()
			{
				CustomId = "inventarioBtn",
				Label = "Ver inventário",
				Style = ButtonStyle.Danger
			};
			var periciasBtn = new ButtonBuilder()
			{
				CustomId = "periciasBtn",
				Label = "Ver perícias",
				Style = ButtonStyle.Success
			};
			var magiasBtn = new ButtonBuilder()
			{
				CustomId = "magiasBtn",
				Label = "Ver magias",
				Style = ButtonStyle.Primary
			};

			var buttonComponent = new ComponentBuilder().WithButton(inventarioBtn).WithButton(periciasBtn).WithButton(magiasBtn);

			await Context.Interaction.ModifyOriginalResponseAsync(msg =>
			{
				msg.Content = "Sua ficha de personagem!";
				msg.Components = buttonComponent.Build();
				msg.Embed = RPGbotUtilities.GerarFicha(personagensController.Get(Context.User.Id).Result);
			});
		}
		[ComponentInteraction("periciasBtn")]
		public async Task PericiasButton()
		{
			await DeferAsync();

			var inventarioBtn = new ButtonBuilder()
			{
				CustomId = "inventarioBtn",
				Label = "Ver inventário",
				Style = ButtonStyle.Danger
			};
			var fichaBtn = new ButtonBuilder()
			{
				CustomId = "fichaBtn",
				Label = "Ver ficha",
				Style = ButtonStyle.Success
			};
			var magiasBtn = new ButtonBuilder()
			{
				CustomId = "magiasBtn",
				Label = "Ver magias",
				Style = ButtonStyle.Primary
			};

			var buttonComponent = new ComponentBuilder()
				.WithButton(inventarioBtn)
				.WithButton(fichaBtn)
				.WithButton(magiasBtn);

			await Context.Interaction.ModifyOriginalResponseAsync(msg =>
			{
				msg.Content = "Suas perícias de personagem!";
				msg.Components = buttonComponent.Build();
				msg.Embed = RPGbotUtilities.GerarPericias(personagensController.Get(Context.User.Id).Result);
			});
		}
		[ComponentInteraction("inventarioBtn")]
		public async Task InventarioButton()
		{
			await DeferAsync();

			var fichaBtn = new ButtonBuilder()
			{
				CustomId = "fichaBtn",
				Label = "Ver ficha",
				Style = ButtonStyle.Danger
			};
			var periciasBtn = new ButtonBuilder()
			{
				CustomId = "periciasBtn",
				Label = "Ver perícias",
				Style = ButtonStyle.Success
			};
			var magiasBtn = new ButtonBuilder()
			{
				CustomId = "magiasBtn",
				Label = "Ver magias",
				Style = ButtonStyle.Primary
			};

			var buttonComponent = new ComponentBuilder()
				.WithButton(fichaBtn)
				.WithButton(periciasBtn)
				.WithButton(magiasBtn);

			await Context.Interaction.ModifyOriginalResponseAsync(msg =>
			{
				msg.Content = "Seu inventário de personagem!";
				msg.Components = buttonComponent.Build();
				msg.Embed = RPGbotUtilities.GerarInventario(personagensController.Get(Context.User.Id).Result);
			});
		}
		[ComponentInteraction("magiasBtn")]
		public async Task MagiasButton()
		{
			await DeferAsync();

			var inventarioBtn = new ButtonBuilder()
			{
				CustomId = "inventarioBtn",
				Label = "Ver inventário",
				Style = ButtonStyle.Danger
			};
			var periciasBtn = new ButtonBuilder()
			{
				CustomId = "periciasBtn",
				Label = "Ver perícias",
				Style = ButtonStyle.Success
			};
			var fichaBtn = new ButtonBuilder()
			{
				CustomId = "fichaBtn",
				Label = "Ver ficha",
				Style = ButtonStyle.Primary
			};

			var buttonComponent = new ComponentBuilder()
				.WithButton(inventarioBtn)
				.WithButton(periciasBtn)
				.WithButton(fichaBtn);

			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			await Context.Interaction.ModifyOriginalResponseAsync(msg =>
			{
				msg.Content = "Suas magias de personagem!";
				msg.Components = buttonComponent.Build();
				msg.Embed = RPGbotUtilities.GerarMagias(personagem.Magias!, personagem);
			});
		}
		#endregion

		#region Básicos
		[SlashCommand("vida", "Adiciona ou remove vida do personagem")]
		public async Task Vida([MaxValue(999), MinValue(-999)] int qntd)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			int old_vida = personagem.Vida;
			personagem.Vida =
				personagem.Vida + qntd > personagem.VidaMax ? personagem.VidaMax :
				personagem.Vida + qntd < -4 ? -4 :
				personagem.Vida + qntd;

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Vida do personagem alterada! Anterior: {old_vida}", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}

		[SlashCommand("xp", "Adiciona pontos de experiência ao personagem")]
		public async Task XP([MaxValue(19999), MinValue(1)] int qntd)
		{
			qntd = Math.Abs(qntd);

			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			int old_xp = personagem.XP;
			personagem.XP += qntd;

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Pontos de experiência do personagem alterados! Anterior: {old_xp}", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}

		[SlashCommand("saldo", "Adiciona dinheiro à carteira do personagem")]
		public async Task Saldo([MaxValue(9999), MinValue(-9999)] float qntd)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			float old_saldo = personagem.Saldo;
			personagem.Saldo += qntd;
			personagem.Saldo = personagem.Saldo < 0 ? 0 : personagem.Saldo;

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Saldo do personagem alterado! Anterior: {old_saldo}", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}

		[SlashCommand("exaustao", "Adiciona e remove exaustão ao personagem")]
		public async Task Exaustao([MaxValue(5), MinValue(-5)] int qntd)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			float old_exaustao = personagem.Exaustão;
			personagem.Exaustão += qntd;
			personagem.Exaustão = personagem.Exaustão < 0 ? 0 : personagem.Exaustão > 5 ? 5 : personagem.Exaustão;

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Saldo do personagem alterado! Anterior: {old_exaustao}", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}

		[SlashCommand("levelup", "Pula de nível e gera os novos atributos do personagem")]
		public async Task LevelUp(Utilities.Atributos atributo, [Optional, DefaultParameterValue(Utilities.Atributos.Nenhum)] Utilities.Atributos atributo2)
		{
			ClassesController classesController = new(new("Classes"));
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}
			if (personagem.XP < RPGbotUtilities.NiveisXP[personagem.Nivel - 1])
			{
				await RespondAsync("Seu personagem não tem XP suficiente!", ephemeral: true); return;
			}
			if (atributo == Utilities.Atributos.Nenhum)
			{
				await RespondAsync("O primeiro atributo não pode ser vazio!", ephemeral: true); return;
			}

			personagem.Nivel += 1;
			int dadoVida = JogarDadosVida(classesController.Get(personagem.Classe).Result.Dice)
				+ RPGbotUtilities.CalcularMod(personagem.Constituicao);
			personagem.VidaMax += dadoVida;
			personagem.Vida += dadoVida;

			if (atributo2 != Utilities.Atributos.Nenhum)
			{
				if (atributo == Utilities.Atributos.Força) personagem.Forca += 1;
				if (atributo == Utilities.Atributos.Destreza) personagem.Destreza += 1;
				if (atributo == Utilities.Atributos.Constituição) personagem.Constituicao += 1;
				if (atributo == Utilities.Atributos.Inteligência) personagem.Inteligencia += 1;
				if (atributo == Utilities.Atributos.Sabedoria) personagem.Sabedoria += 1;
				if (atributo == Utilities.Atributos.Carisma) personagem.Carisma += 1;

				if (atributo2 == Utilities.Atributos.Força) personagem.Forca += 1;
				if (atributo2 == Utilities.Atributos.Destreza) personagem.Destreza += 1;
				if (atributo2 == Utilities.Atributos.Constituição) personagem.Constituicao += 1;
				if (atributo2 == Utilities.Atributos.Inteligência) personagem.Inteligencia += 1;
				if (atributo2 == Utilities.Atributos.Sabedoria) personagem.Sabedoria += 1;
				if (atributo2 == Utilities.Atributos.Carisma) personagem.Carisma += 1;
			}
			else
			{
				if (atributo == Utilities.Atributos.Força) personagem.Forca += 2;
				if (atributo == Utilities.Atributos.Destreza) personagem.Destreza += 2;
				if (atributo == Utilities.Atributos.Constituição) personagem.Constituicao += 2;
				if (atributo == Utilities.Atributos.Inteligência) personagem.Inteligencia += 2;
				if (atributo == Utilities.Atributos.Sabedoria) personagem.Sabedoria += 2;
				if (atributo == Utilities.Atributos.Carisma) personagem.Carisma += 2;
			}

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Você agora está no **nível {personagem.Nivel}**!", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}
		readonly Random Rnd = new();
		int JogarDadosVida(int y)
		{
			int resultado = Rnd.Next(1, y + 1);

			return resultado;
		}
		#endregion

		#region Magias
		[SlashCommand("addmagia", "Adiciona uma magia aos conhecimentos do personagem")]
		public async Task AddMagia(string nome)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			ClassesController classesController = new(new("Classes"));

			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}
			if (classesController.Get(personagem.Classe)!.Result!.Magico)
			{
				ErrorModule.ClasseNaoMagica(Context); return;
			}

			string magia = magiasController.Get(nome).Result.Name;

			if (magia == null)
			{
				ErrorModule.MagiaNotFound(Context, nome); return;
			}

			personagem.Magias ??= new List<string>();

			if (personagem.Magias.Contains(magia))
			{
				ErrorModule.HasOrHasnt(Context, true, ErrorModule.T.Magia); return;
			}
			if (personagem.Magias.Count >= classesController.Get(personagem.Classe)!.Result!.Magias![personagem.Nivel - 1])
			{
				ErrorModule.NotEnoughXP(Context); return;
			}

			personagem.Magias.Add(magia);

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Magia ``{magia}`` adicionada ao personagem!", ephemeral: true, embed: RPGbotUtilities.GerarMagias(personagem.Magias, personagem));
		}

		[SlashCommand("removermagia", "Remove uma magia dos conhecimentos do personagem")]
		public async Task RemoverMagia(string nome)
		{
			ClassesController classesController = new(new("Classes"));
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}
			if (classesController.Get(personagem.Classe)!.Result!.Magico)
			{
				ErrorModule.ClasseNaoMagica(Context); return;
			}

			personagem.Magias ??= new List<string>();

			string magia = personagem.Magias.Find(x => Utilities.FormatID(x) == Utilities.FormatID(nome))!;

			if (magia == null)
			{
				ErrorModule.HasOrHasnt(Context, false, ErrorModule.T.Magia); return;
			}
			personagem.Magias.Remove(magia);

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Magia ``{magia}`` removida do personagem!", ephemeral: true);
		}

		/*[SlashCommand("magias", "Apresenta as mágias conhecidas pelo personagem")]
		public async Task Magias()
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}
			if (!personagem.Classe!.Magico)
			{
				ErrorModule.ClasseNaoMagica(Context); return;
			}
			if (personagem.Magias == null || personagem.Magias.Count == 0)
			{
				ErrorModule.DoesntHaveInventory(Context, ErrorModule.T.Magia); return;
			}

			await RespondAsync($"As magias do seu personagem!", ephemeral: true, embed: RPGbotUtilities.GerarMagias(personagem.Magias, personagem));
		}*/
		#endregion

		#region Itens e Inventário
		/*[SlashCommand("inventario", "Apresenta o inventário do personagem")]
		public async Task Inventario()
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			if (personagem.Inventario == null || personagem.Inventario.Count == 0)
			{
				ErrorModule.DoesntHaveInventory(Context, ErrorModule.T.Item); return;
			}

			var fichaBtn = new ButtonBuilder()
			{
				CustomId = "fichaBtn",
				Label = "Ver ficha",
				Style = ButtonStyle.Primary
			};
			var periciasBtn = new ButtonBuilder()
			{
				CustomId = "periciasBtn",
				Label = "Ver perícias",
				Style = ButtonStyle.Success
			};

			var buttonComponent = new ComponentBuilder().WithButton(fichaBtn).WithButton(periciasBtn);

			await RespondAsync($"Seu inventário de personagem!", ephemeral: true, components: buttonComponent.Build(), embed: RPGbotUtilities.GerarInventario(personagem));
		}*/

		[SlashCommand("additem", "Adiciona um item ao inventário do personagem")]
		public async Task AddItem(string nome, [Optional(), DefaultParameterValue(1), MinValue(1)] int quantidade)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			Item item = itensController.Get(nome).Result;

			if (item == null)
			{
				ErrorModule.ItemNotFound(Context, nome); return;
			}
			personagem.Inventario ??= new List<Item>();

			if (personagem.Forca * RPGbotUtilities.PesoMod < (RPGbotUtilities.GerarPesoInventario(personagem) + item.Peso) * quantidade)
			{
				ErrorModule.NotEnoughInvSpace(Context); return;
			}

			for (int i = 0; i < quantidade; i++)
				personagem.Inventario.Add(item);

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Item ``{item.Name}`` adicionado ao inventário!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
		}

		[SlashCommand("removeritem", "Remove um item do inventário do personagem")]
		public async Task RemoverItem(string nome, [Optional(), DefaultParameterValue(1), MinValue(1)] int quantidade)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			personagem.Inventario ??= new List<Item>();

			Item item = personagem.Inventario.Find(x => x.Id == Utilities.FormatID(nome))!;

			if (!personagem.Inventario.Contains(item))
			{
				ErrorModule.HasOrHasnt(Context, false, ErrorModule.T.Item); return;
			}
			string old_item = item.Name;

			for (int i = 0; i < quantidade; i++)
				personagem.Inventario.Remove(personagem.Inventario.Find(x => x.Id == Utilities.FormatID(nome))!);

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Item ``{old_item}`` removido do inventário!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
		}

		[SlashCommand("compraritem", "Compra um item e o adiciona ao inventário do personagem")]
		public async Task ComprarItem([Required] string nome, [Optional, MinValue(1), DefaultParameterValue(0)] float preço)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			Item item = itensController.Get(nome).Result;

			if (item == null)
			{
				ErrorModule.ItemNotFound(Context, nome); return;
			}
			personagem.Inventario ??= new List<Item>();

			if (personagem.Forca * RPGbotUtilities.PesoMod + RPGbotUtilities.GetMochila(personagem.Inventario) + personagem.Saldo * 0.1f < (RPGbotUtilities.GerarPesoInventario(personagem) + item.Peso))
			{
				ErrorModule.NotEnoughInvSpace(Context); return;
			}

			if (personagem.Saldo < (preço > 0 ? preço : item.Preco))
			{
				ErrorModule.NotEnoughMoney(Context); return;
			}

			if (preço > 0)
				personagem.Saldo -= preço;
			else
				personagem.Saldo -= item.Preco;
			personagem.Inventario.Add(item);

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Item comprado por {(preço > 0 ? preço : item.Preco)} moedas!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
		}

		public enum Equipaveis { Armadura, Escudo }
		[SlashCommand("equipar", "Equipar um item do inventário ao personagem")]
		public async Task Equipar(Equipaveis tipo, string nome)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			Item item = personagem.Inventario!.Find(x => x.Id == Utilities.FormatID(nome))!;

			if (!personagem.Inventario!.Contains(item))
			{
				ErrorModule.HasOrHasnt(Context, false, ErrorModule.T.Item); return;
			}

			string old_item = "";

			if (tipo == Equipaveis.Armadura)
			{
				if (item.Tipo == "Escudo" || item.Defesa == 0)
				{
					await RespondAsync($"O item ``{item.Name}`` não é uma armadura!", ephemeral: true); return;
				}

				if (personagem.Armadura != null)
					old_item = personagem.Armadura!.Name;
				personagem.Armadura = item;
			}
			if (tipo == Equipaveis.Escudo)
			{
				if (item.Tipo != "Escudo")
				{
					await RespondAsync($"O item ``{item.Name}`` não é um escudo!", ephemeral: true); return;
				}

				if (personagem.Escudo != null)
					old_item = personagem.Escudo!.Name;
				personagem.Escudo = item;
			}

			await personagensController.Put(personagem.Id, personagem);

			if (old_item == "")
				await RespondAsync($"Item ``{item.Name}`` equipado!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
			else
				await RespondAsync($"Item ``{item.Name}`` equipado, e item ``{old_item}`` desequipado!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
		}

		[SlashCommand("desequipar", "Desequipar um item do personagem ao inventário")]
		public async Task Desequipar(Equipaveis tipo)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			if (tipo == Equipaveis.Armadura)
			{
				if (personagem.Armadura == null)
				{
					await RespondAsync($"Você não tem uma armadura equipada!", ephemeral: true); return;
				}

				personagem.Armadura = null;
			}
			if (tipo == Equipaveis.Escudo)
			{
				if (personagem.Escudo == null)
				{
					await RespondAsync($"Você não tem um escudo equipado!", ephemeral: true); return;
				}

				personagem.Escudo = null;
			}

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Item desequipado!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
		}
		#endregion

		#region Perícias
		[SlashCommand("addpericia", "Adiciona uma perícia ao personagem")]
		public async Task AddPericia(string nome)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			string pericia = periciasController.Get(nome).Result.Nome;

			if (pericia == null)
			{
				ErrorModule.PericiaNotFound(Context, nome); return;
			}

			personagem.Pericias ??= new List<string>();

			if (personagem.Pericias.Contains(pericia))
			{
				ErrorModule.HasOrHasnt(Context, true, ErrorModule.T.Pericia); return;
			}

			personagem.Pericias.Add(pericia);

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Perícia ``{pericia}`` adicionada ao personagem!", ephemeral: true, embed: RPGbotUtilities.GerarFicha(personagem));
		}

		[SlashCommand("removerpericia", "Remove uma perícia do personagem")]
		public async Task RemoverPericia(string nome)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}

			personagem.Pericias ??= new List<string>();

			string pericia = personagem.Pericias.Find(x => Utilities.FormatID(x) == Utilities.FormatID(nome))!;

			if (pericia == null)
			{
				ErrorModule.HasOrHasnt(Context, false, ErrorModule.T.Pericia); return;
			}
			personagem.Pericias.Remove(pericia);

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Perícia ``{pericia}`` removida do personagem!", ephemeral: true);
		}
		#endregion
	}
}