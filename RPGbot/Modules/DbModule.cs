﻿using Discord;
using Discord.Interactions;
using RPGbotAPI.Controllers;
using RPGbotAPI.Models;
using RPGbotAPI.Services;
using RPGbotLib;

namespace RPGbot.Modules
{
	public class DbModule : InteractionModuleBase<SocketInteractionContext>
	{
		readonly ItensController itensController = new(new ItemService());
		readonly MagiasController magiasController = new(new MagiaService());
		readonly PericiasController periciasController = new(new PericiaService());

		[RequireRole("Mestre")]
		[SlashCommand("dbadditem", "Adiciona item à base de dados do RPGbot")]
		public async Task AddItem(string Nome, float Preço, float Peso)
		{
			var tipoMenu = new SelectMenuBuilder()
			{
				CustomId = $"tipoMenu:{Nome},{Preço},{Peso}",
				Placeholder = "Escolha o tipo de item"
			};

			TiposDeItem[] tiposDeItens = Enum.GetValues<TiposDeItem>();
			foreach (var tipoDeItem in tiposDeItens)
			{
				tipoMenu.AddOption(tipoDeItem.ToString(), tipoDeItem.ToString());
			}

			var component = new ComponentBuilder().WithSelectMenu(tipoMenu);

			Item item = new()
			{
				Id = Utilities.FormatID(Nome),
				Name = Nome,
				Preco = Preço,
				Peso = Peso
			};

			await RespondAsync($"Adicionando um item à base de dados!", components: component.Build(), ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("dbaddmagia", "Adiciona magia à base de dados do RPGbot")]
		public async Task AddMagia(string Nome, string Lançamento, string Alcance, string Componentes, string Duração, string Descrição)
		{
			Magia magia = new() { Name = Nome, Casting = Lançamento, Range = Alcance, Components = Componentes, Duration = Duração, Description = Descrição };

			await magiasController.Post(magia);

			await RespondAsync($"Magia ``{magia.Name}`` adicionada à base");
		}

		[RequireRole("Mestre")]
		[SlashCommand("dbaddpericia", "Adiciona perícia à base de dados do RPGbot")]
		public async Task AddPericia(string nome, Utilities.Atributos atributo, string descricao)
		{
			Pericia pericia = new() { Nome = nome, Atributo = atributo, Descricao = descricao };

			await periciasController.Post(pericia);

			await RespondAsync($"Perícia ``{pericia.Nome}`` adicionada à base");
		}

		[ComponentInteraction("tipoMenu:*,*,*")]
		public async Task HandleTipoMenu(string Nome, float Preço, float Peso, string selected)
		{
			Console.WriteLine(selected);
			switch (selected)
			{
				case "Item":
					await RespondWithModalAsync<ItemModal>($"itemModal:{Nome},{Preço},{Peso}");
					break;
				case "Arma":
					await RespondWithModalAsync<ArmaModal>($"armaModal:{Nome},{Preço},{Peso}");
					break;
				case "Armadura":
					await RespondWithModalAsync<ArmaduraModal>($"armaduraModal:{Nome},{Preço},{Peso}");
					break;
			}
		}

		[ModalInteraction("itemModal:*,*,*")]
		public async Task HandleItemModal(string Nome, float Preço, float Peso, ItemModal modal)
		{
			Item item = new()
			{
				Name = Nome,
				Id = Utilities.FormatID(Nome),
				Tipo = modal.Tipo,
				Preco = Preço,
				Peso = Peso
			};

			await itensController.Post(item);

			await RespondAsync($"Item ``{item.Name}`` adicionado à base de dados!", ephemeral: true);
		}
		[ModalInteraction("armaModal:*,*,*")]
		public async Task HandleArmaModal(string Nome, float Preço, float Peso, ArmaModal modal)
		{
			Item item = new()
			{
				Name = Nome,
				Id = Utilities.FormatID(Nome),
				Tipo = modal.Tipo,
				Propriedades = modal.Propriedades,
				Dano = modal.Atributo,
				Preco = Preço,
				Peso = Peso
			};

			await itensController.Post(item);

			await RespondAsync($"Arma ``{item.Name}`` adicionado à base de dados!", ephemeral: true);
		}
		[ModalInteraction("armaduraModal:*,*,*")]
		public async Task HandleArmaduraModal(string Nome, float Preço, float Peso, ArmaduraModal modal)
		{
			Item item = new()
			{
				Name = Nome,
				Id = Utilities.FormatID(Nome),
				Tipo = modal.Tipo,
				Propriedades = modal.Propriedades,
				Defesa = int.Parse(modal.Atributo),
				Preco = Preço,
				Peso = Peso
			};

			await itensController.Post(item);

			await RespondAsync($"Armadura ``{item.Name}`` adicionado à base de dados!", ephemeral: true);
		}

		enum TiposDeItem { Item, Arma, Armadura }
		public class ItemModal : IModal
		{
			public string Title => "Adicione um item!";

			[InputLabel("Categoria do item")]
			[ModalTextInput("tipo", TextInputStyle.Short, "Arma Simples Corpo-a-Corpo...", maxLength: 30)]
			public string Tipo { get; set; } = string.Empty;
		}
		public class ArmaModal : IModal
		{
			public string Title => "Adicione uma arma!";

			[InputLabel("Categoria do item")]
			[ModalTextInput("tipo", TextInputStyle.Short, "Arma Simples Corpo-a-Corpo...", maxLength: 30)]
			public string Tipo { get; set; } = string.Empty;

			[InputLabel("Propriedades ou descrição do item")]
			[ModalTextInput("propriedades", TextInputStyle.Paragraph, "Acuidade, leve, arremesso (distância 6/18)...", maxLength: 150)]
			public string Propriedades { get; set; } = string.Empty;

			[InputLabel("Atributo do item (dano, defesa)")]
			[ModalTextInput("atributo", TextInputStyle.Short, "1d4 perfurante...", maxLength: 30)]
			public string Atributo { get; set; } = string.Empty;
		}
		public class ArmaduraModal : IModal
		{
			public string Title => "Adicione uma armadura!";

			[InputLabel("Categoria do item")]
			[ModalTextInput("tipo", TextInputStyle.Short, "Arma Simples Corpo-a-Corpo...", maxLength: 30)]
			public string Tipo { get; set; } = string.Empty;

			[InputLabel("Propriedades ou descrição do item")]
			[ModalTextInput("propriedades", TextInputStyle.Paragraph, "Acuidade, leve, arremesso (distância 6/18)...", maxLength: 150)]
			public string Propriedades { get; set; } = string.Empty;

			[InputLabel("Atributo do item (dano, defesa)")]
			[ModalTextInput("atributo", TextInputStyle.Short, "1d4 perfurante...", maxLength: 30)]
			public string Atributo { get; set; } = string.Empty;

			[InputLabel("Nome do atributo modificador do item (se houver)")]
			[ModalTextInput("modNome", TextInputStyle.Short, "Destreza...", maxLength: 15)]
			public string ModNome { get; set; } = string.Empty;
		}
	}
}