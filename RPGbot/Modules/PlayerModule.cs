using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
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

		[SlashCommand("ficha", "Apresenta a ficha do personagem")]
		public async Task Ficha()
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null || personagem._Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			var inventarioBtn = new ButtonBuilder()
			{
				CustomId = "inventarioBtn",
				Label = "Ver inventário",
				Style = ButtonStyle.Primary
			};
			var periciasBtn = new ButtonBuilder()
			{
				CustomId = "periciasBtn",
				Label = "Ver perícias",
				Style = ButtonStyle.Success
			};

			var buttonComponent = new ComponentBuilder().WithButton(inventarioBtn).WithButton(periciasBtn);

			await RespondAsync($"Sua ficha de personagem!", ephemeral: true, components: buttonComponent.Build(), embed: RPGbotUtilities.GerarFicha(personagem));
		}
		[ComponentInteraction("fichaBtn")]
		public async Task FichaButton()
		{
			var inventarioBtn = new ButtonBuilder()
			{
				CustomId = "inventarioBtn",
				Label = "Ver inventário",
				Style = ButtonStyle.Primary
			};
			var periciasBtn = new ButtonBuilder()
			{
				CustomId = "periciasBtn",
				Label = "Ver perícias",
				Style = ButtonStyle.Success
			};

			var buttonComponent = new ComponentBuilder().WithButton(inventarioBtn).WithButton(periciasBtn);

			await Context.Interaction.RespondAsync("Sua ficha de personagem!", ephemeral: true, components: buttonComponent.Build(), embed: RPGbotUtilities.GerarFicha(personagensController.Get(Context.User.Id).Result));
		}
		[ComponentInteraction("periciasBtn")]
		public async Task PericiasButton()
		{
			var fichaBtn = new ButtonBuilder()
			{
				CustomId = "fichaBtn",
				Label = "Ver ficha",
				Style = ButtonStyle.Primary
			};
			var inventarioBtn = new ButtonBuilder()
			{
				CustomId = "inventarioBtn",
				Label = "Ver inventário",
				Style = ButtonStyle.Success
			};

			var buttonComponent = new ComponentBuilder().WithButton(fichaBtn).WithButton(inventarioBtn);

			await Context.Interaction.RespondAsync("Suas perícias de personagem!", ephemeral: true, components: buttonComponent.Build(), embed: RPGbotUtilities.GerarPericias(personagensController.Get(Context.User.Id).Result));
		}
		[ComponentInteraction("inventarioBtn")]
		public async Task InventarioButton()
		{
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

			await Context.Interaction.RespondAsync("Seu inventário de personagem!", ephemeral: true, components: buttonComponent.Build(), embed: RPGbotUtilities.GerarInventario(personagensController.Get(Context.User.Id).Result));
		}

		[SlashCommand("vida", "Adiciona ou remove vida do personagem")]
		public async Task Vida([MaxValue(999), MinValue(-999)] int qntd)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			int old_vida = personagem.Vida;
			personagem.Vida =
				personagem.Vida + qntd > personagem.VidaMax ? personagem.VidaMax :
				personagem.Vida + qntd < -4 ? -4 :
				personagem.Vida + qntd;

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Vida do personagem alterada! Anterior: {old_vida}", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}

		[SlashCommand("xp", "Adiciona pontos de experiência ao personagem")]
		public async Task XP([MaxValue(19999), MinValue(1)] int qntd)
		{
			qntd = Math.Abs(qntd);

			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			int old_xp = personagem.XP;
			personagem.XP += qntd;

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Pontos de experiência do personagem alterados! Anterior: {old_xp}", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}

		[SlashCommand("saldo", "Adiciona dinheiro à carteira do personagem")]
		public async Task Saldo([MaxValue(9999), MinValue(-9999)] float qntd)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			float old_saldo = personagem.Saldo;
			personagem.Saldo += qntd;
			personagem.Saldo = personagem.Saldo < 0 ? 0 : personagem.Saldo;

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Saldo do personagem alterado! Anterior: {old_saldo}", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}

		[SlashCommand("addmagia", "Adiciona uma magia aos conhecimentos do personagem")]
		public async Task AddMagia(string nome)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!personagem.Classe!.Magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}

			Magia magia = magiasController.Get(nome).Result;

			if (magia == null)
			{
				await RespondAsync($"Magia ``{magia}`` não existe!", ephemeral: true); return;
			}

			personagem.Magias ??= new List<Magia>();

			if (personagem.Magias.Contains(magia))
			{
				await RespondAsync($"Seu personagem já conhece esta magia.", ephemeral: true); return;
			}
			if (personagem.Magias.Count >= personagem.Classe!.Magias![RPGbotUtilities.GerarNivel(personagem.XP) - 1])
			{
				await RespondAsync($"Seu personagem não tem experiência suficiente para aprender tantas magias! Explore o mundo e tente estudar novamente.", ephemeral: true); return;
			}

			personagem.Magias.Add(magia);

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Magia ``{magia.Name}`` adicionada ao personagem!", ephemeral: true, embed: RPGbotUtilities.GerarMagias(personagem.Magias, personagem));
		}

		[SlashCommand("removermagia", "Remove uma magia dos conhecimentos do personagem")]
		public async Task RemoverMagia(string nome)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!personagem.Classe!.Magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}

			personagem.Magias ??= new List<Magia>();

			Magia magia = personagem.Magias.Find(x => x.Id == Utilities.FormatID(nome))!;

			if (magia == null)
			{
				await RespondAsync($"Seu personagem não conhece esta magia.", ephemeral: true); return;
			}
			personagem.Magias.Remove(magia);

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Magia ``{magia.Name}`` removida do personagem!", ephemeral: true);
		}

		[SlashCommand("magias", "Apresenta as mágias conhecidas pelo personagem")]
		public async Task Magias()
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!personagem.Classe!.Magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}
			if (personagem.Magias == null || personagem.Magias.Count == 0)
			{
				await RespondAsync($"O seu personagem não conhece nenhuma magia.", ephemeral: true); return;
			}

			await RespondAsync($"As magias do seu personagem!", ephemeral: true, embed: RPGbotUtilities.GerarMagias(personagem.Magias, personagem));
		}

		[SlashCommand("vermagia", "Apresenta as informações da magia, como tempo de conjuração, alcance, etc.")]
		public async Task VerMagia(string magia)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!personagem.Classe!.Magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}
			if (magiasController.Get(magia).Result == null)
			{
				await RespondAsync($"Magia {magia} não existe!", ephemeral: true); return;
			}

			await RespondAsync($"Informações da magia:", ephemeral: true, embed: RPGbotUtilities.GerarMagia(magia));
		}

		[SlashCommand("inventario", "Apresenta o inventário do personagem")]
		public async Task Inventario()
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			if (personagem.Inventario == null || personagem.Inventario.Count == 0)
			{
				await RespondAsync($"{personagem.Nome} não tem itens em seu inventário.", ephemeral: true); return;
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
		}

		[SlashCommand("additem", "Adiciona um item ao inventário do personagem")]
		public async Task AddItem(string nome, [Optional(), DefaultParameterValue(1), MinValue(1)] int quantidade)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			Item item = itensController.Get(nome).Result;

			if (item == null)
			{
				await RespondAsync($"Item ``{nome}`` não existe!", ephemeral: true); return;
			}
			personagem.Inventario ??= new List<Item>();

			if (personagem.Forca * RPGbotUtilities.PesoMod < (RPGbotUtilities.GerarPesoInventario(personagem) + item.Peso) * quantidade)
			{
				await RespondAsync($"Você não tem espaço para carregar este item na sua mochila!", ephemeral: true); return;
			}

			for (int i = 0; i < quantidade; i++)
				personagem.Inventario.Add(item);

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Item ``{item.Name}`` adicionado ao inventário!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
		}

		[SlashCommand("removeritem", "Remove um item do inventário do personagem")]
		public async Task RemoverItem(string nome, [Optional(), DefaultParameterValue(1), MinValue(1)] int quantidade)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			personagem.Inventario ??= new List<Item>();

			Item item = personagem.Inventario.Find(x => x.Id == Utilities.FormatID(nome))!;

			if (!personagem.Inventario.Contains(item))
			{
				await RespondAsync($"Seu personagem não tem este item.", ephemeral: true); return;
			}
			string old_item = item.Name;

			for (int i = 0; i < quantidade; i++)
				personagem.Inventario.Remove(personagem.Inventario.Find(x => x.Id == Utilities.FormatID(nome))!);

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Item ``{old_item}`` removido do inventário!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
		}

		[SlashCommand("compraritem", "Compra um item e o adiciona ao inventário do personagem")]
		public async Task ComprarItem([Required] string nome, [Optional, MinValue(1), DefaultParameterValue(0)] float preço)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			Item item = itensController.Get(nome).Result;

			if (item == null)
			{
				await RespondAsync($"Item ``{nome}`` não existe!", ephemeral: true); return;
			}
			personagem.Inventario ??= new List<Item>();

			if (personagem.Forca * RPGbotUtilities.PesoMod + RPGbotUtilities.GetMochila(personagem.Inventario) + personagem.Saldo * 0.1f < (RPGbotUtilities.GerarPesoInventario(personagem) + item.Peso))
			{
				await RespondAsync($"Você não tem espaço para carregar este item na sua mochila!", ephemeral: true); return;
			}

			if (personagem.Saldo < (preço > 0 ? preço : item.Preco))
			{
				await RespondAsync($"Você não tem dinheiro suficiente! Você tem ``{personagem.Saldo}`` de saldo, e o item custa ``{(preço > 0 ? preço : item.Preco)}``.", ephemeral: true); return;
			}

			if (preço > 0)
				personagem.Saldo -= preço;
			else
				personagem.Saldo -= item.Preco;
			personagem.Inventario.Add(item);

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Item comprado por {(preço > 0 ? preço : item.Preco)} moedas!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
		}

		[SlashCommand("veritem", "Apresenta as informações do item como dano, peso, preço, etc.")]
		public async Task VerItem(string nome)
		{
			Embed embed = RPGbotUtilities.GerarItem(nome);
			if (embed == null)
			{
				await RespondAsync($"Item ``{nome}`` não existe!", ephemeral: true); return;
			}

			await RespondAsync($"Item:", embed: embed, ephemeral: true);
		}

		[SlashCommand("addpericia", "Adiciona uma perícia ao personagem")]
		public async Task AddPericia(string nome)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			Pericia pericia = periciasController.Get(nome).Result;

			if (pericia == null)
			{
				await RespondAsync($"Perícia ``{nome}`` não existe!", ephemeral: true); return;
			}

			personagem.Pericias ??= new List<Pericia>();

			if (personagem.Pericias.Contains(pericia))
			{
				await RespondAsync($"Seu personagem já tem esta perícia.", ephemeral: true); return;
			}

			personagem.Pericias.Add(pericia);

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Perícia ``{pericia.Nome}`` adicionada ao personagem!", ephemeral: true, embed: RPGbotUtilities.GerarFicha(personagem));
		}

		[SlashCommand("removerpericia", "Remove uma perícia do personagem")]
		public async Task RemoverPericia(string nome)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			personagem.Pericias ??= new List<Pericia>();

			Pericia pericia = personagem.Pericias.Find(x => x.Id == Utilities.FormatID(nome))!;

			if (pericia == null)
			{
				await RespondAsync($"Seu personagem não tem esta perícia.", ephemeral: true); return;
			}
			personagem.Pericias.Remove(pericia);

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Perícia ``{pericia.Nome}`` removida do personagem!", ephemeral: true);
		}

		[SlashCommand("verpericias", "Apresenta todas as perícias disponíveis")]
		public async Task VerPericias()
		{
			List<Pericia> list = periciasController.GetAll().Result;

			await RespondAsync($"Perícias:", ephemeral: true, embed: RPGbotUtilities.GerarAllPericias(list));
		}

		public enum Equipaveis { Armadura, Escudo }
		[SlashCommand("equipar", "Equipar um item do inventário ao personagem")]
		public async Task Equipar(Equipaveis tipo, string nome)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			Item item = personagem.Inventario!.Find(x => x.Id == Utilities.FormatID(nome))!;

			if (!personagem.Inventario!.Contains(item))
			{
				await RespondAsync($"Você não tem o item ``{item.Name}``!", ephemeral: true); return;
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

			await personagensController.Put(personagem._Id, personagem);

			if (old_item == "")
				await RespondAsync($"Item ``{item.Name}`` equipado!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
			else
				await RespondAsync($"Item ``{item.Name}`` equipado, e item ``{old_item}`` desequipado!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
		}

		[SlashCommand("desequipar", "Desequipar um item do personagem ao inventário")]
		public async Task Desequipar(Equipaveis tipo, string nome)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
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

			await personagensController.Put(personagem._Id, personagem);

				await RespondAsync($"Item desequipado!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
		}

		[SlashCommand("veritens", "Mostra todos os itens disponíveis no jogo até então")]
		public async Task VerItens()
		{
			List<Item> itens = itensController.GetAll().Result.ToList();

			itens = itens.OrderByDescending(x => x.Tipo).ToList();

			int maxlenght = 1536;

			string itensTxt1 = "";
			string itensTxt2 = "";
			string itensTxt3 = "";
			foreach (Item item in itens)
			{
				if (itensTxt1.Length < maxlenght)
					itensTxt1 += $"{item.Name}\n";
				else if (itensTxt2.Length < maxlenght)
					itensTxt2 += $"{item.Name}\n";
				else
					itensTxt3 += $"{item.Name}\n";
			}

			await RespondAsync(itensTxt1, ephemeral: true);
			if (itensTxt2 != "") await FollowupAsync(itensTxt2, ephemeral: true);
			if (itensTxt3 != "") await FollowupAsync(itensTxt3, ephemeral: true);
		}
	}
}