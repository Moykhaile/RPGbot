using Discord.Interactions;
using RPGbotAPI.Controllers;
using RPGbotAPI.Models;
using RPGbotAPI.Services;
using RPGbotLib;

namespace RPGbot.Modules
{
	public class DbModule : InteractionModuleBase<SocketInteractionContext>
	{
		readonly ItensController itensController = new(new ItemService("Itens"));
		readonly MagiasController magiasController = new(new MagiaService("Magias"));
		readonly PericiasController periciasController = new(new PericiaService("Pericias"));

		[RequireRole("Mestre")]
		[SlashCommand("dbadditem", "Adiciona item à base de dados do RPGbot")]
		public async Task AddItem(string Name, string Tipo, float Preco, float Peso)
		{
			Item item = new()
			{
				Name = Name,
				Peso = Peso,
				Dano = string.Empty,
				Defesa = 0,
				Tipo = Tipo,
				ModNome = string.Empty,
				Propriedades = string.Empty,
				Preco = Preco
			};

			await itensController.Post(item);

			await RespondAsync($"Item '{item.Name}' adicionado à base de dados!");
		}

		[RequireRole("Mestre")]
		[SlashCommand("dbaddarma", "Adiciona arma à base de dados do RPGbot")]
		public async Task AddArma(string Name, string Tipo, float Preco, string Dano, float Peso, string Propriedades)
		{
			Item item = new()
			{
				Name = Name,
				Peso = Peso,
				Dano = Dano,
				Defesa = 0,
				Tipo = Tipo,
				ModNome = string.Empty,
				Propriedades = Propriedades,
				Preco = Preco
			};

			await itensController.Post(item);

			await RespondAsync($"Item '{Name}' adicionado à base de dados!");
		}

		[RequireRole("Mestre")]
		[SlashCommand("dbaddarmadura", "Adiciona armadura à base de dados do RPGbot")]
		public async Task AddArmadura(string Name, string Tipo, float Preco, int Defesa, string ModNome, float Peso, string Propriedades)
		{
			if (ModNome == ".") ModNome = "";

			Item item = new()
			{
				Name = Name,
				Peso = Peso,
				Dano = string.Empty,
				Defesa = Defesa,
				Tipo = Tipo,
				ModNome = ModNome,
				Propriedades = Propriedades,
				Preco = Preco
			};

			await itensController.Post(item);

			await RespondAsync($"Item '{item.Name}' adicionado à base de dados!");
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
	}
}
