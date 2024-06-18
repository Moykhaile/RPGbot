using Discord;
using Discord.Interactions;
using Microsoft.AspNetCore.Mvc.Formatters;
using RPGbot.Classes;
using RPGbotAPI.Controllers;
using RPGbotAPI.Models;
using RPGbotAPI.Services;
using static System.Net.Mime.MediaTypeNames;

namespace RPGbot.Modules
{
	public class DisplayModule : InteractionModuleBase<SocketInteractionContext>
	{
		readonly PericiasController periciasController = new(new PericiaService("Pericias"));
		readonly MagiasController magiasController = new(new MagiaService("Magias"));
		readonly ItensController itensController = new(new ItemService("Itens"));

		[SlashCommand("vermagia", "Apresenta as informações da magia, como tempo de conjuração, alcance, etc.")]
		public async Task VerMagia(string magia)
		{
			if (magiasController.Get(magia).Result == null)
			{
				ErrorModule.MagiaNotFound(Context, magia); return;
			}

			await RespondAsync($"Informações da magia:", ephemeral: true, embed: RPGbotUtilities.GerarMagia(magia));
		}

		[SlashCommand("veritem", "Apresenta as informações do item como dano, peso, preço, etc.")]
		public async Task VerItem(string nome)
		{
			Embed embed = RPGbotUtilities.GerarItem(nome);
			if (embed == null)
			{
				ErrorModule.ItemNotFound(Context, nome); return;
			}

			await RespondAsync($"Item:", embed: embed, ephemeral: true);
		}

		[SlashCommand("verpericias", "Apresenta todas as perícias disponíveis")]
		public async Task VerPericias()
		{
			List<Pericia> list = periciasController.GetAll().Result;

			await RespondAsync($"Perícias:", ephemeral: true, embed: RPGbotUtilities.GerarAllPericias(list));
		}

		#region VER TODOS OS ITENS
		private const int itensPorPagina = 25;
		[SlashCommand("veritens", "Mostra todos os itens disponíveis no jogo até então")]
		public async Task VerItens()
		{
			var buttonComponent = new ComponentBuilder().WithButton("← Anterior", $"NoButton", ButtonStyle.Secondary).WithButton("Próxima →", $"NextPage:{itensPorPagina}", ButtonStyle.Success);

			List<Item> itens = itensController.GetAll().Result.ToList();
			itens = itens.OrderBy(x => x.Name).ToList();
			itens = itens.OrderByDescending(x => x.Tipo).ToList();
			string itensTxt = "";
			for (int i = 0; i < itensPorPagina; i++)
			{
				if (itens.Count > i)
					itensTxt += $"\n* {itens[i].Name} - {itens[i].Preco}₹";
				else break;
			}
			var embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"Itens do RPGbot (1/{itens.Count / itensPorPagina + 1})" },
				Description = itensTxt,
				Footer = new EmbedFooterBuilder() { Text = "Nem todos os itens listados podem estar inclusos na aventura atual." }
			};

			await RespondAsync("Itens!", ephemeral: true, components: buttonComponent.Build(), embed: embed.Build());
		}

		[ComponentInteraction("PreviousPage:*", ignoreGroupNames: true)]
		public async Task PreviousPage(string CurPage)
		{
			await DeferAsync();

			int CurrentPage = int.Parse(CurPage);

			var buttonComponent = new ComponentBuilder();
			if (!(CurrentPage - itensPorPagina < 0))
				buttonComponent.WithButton("← Anterior", $"PreviousPage:{CurrentPage - itensPorPagina}", ButtonStyle.Primary);
			else
				buttonComponent.WithButton("← Anterior", $"NoButton", ButtonStyle.Secondary);
			buttonComponent.WithButton("Próxima →", $"NextPage:{CurrentPage + itensPorPagina}", ButtonStyle.Success);

			List<Item> itens = itensController.GetAll().Result.ToList();
			itens = itens.OrderBy(x => x.Name).ToList();
			itens = itens.OrderByDescending(x => x.Tipo).ToList();
			string itensTxt = "";
			for (int i = CurrentPage; i < CurrentPage + itensPorPagina; i++)
			{
				if (itens.Count > i)
					itensTxt += $"\n* {itens[i].Name} - {itens[i].Preco}₹";
				else break;
			}
			var embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"Itens do RPGbot ({CurrentPage / itensPorPagina + 1}/{itens.Count / itensPorPagina + 1})" },
				Description = itensTxt,
				Footer = new EmbedFooterBuilder() { Text = "Nem todos os itens listados podem estar inclusos na aventura atual." }
			};

			await Context.Interaction.ModifyOriginalResponseAsync(msg =>
			{
				msg.Embed = embed.Build();
				msg.Components = buttonComponent.Build();
			});
		}

		[ComponentInteraction("NextPage:*", ignoreGroupNames: true)]
		public async Task NextPage(string CurPage)
		{
			await DeferAsync();

			int CurrentPage = int.Parse(CurPage);

			List<Item> itens = itensController.GetAll().Result.ToList();
			itens = itens.OrderBy(x => x.Name).ToList();
			itens = itens.OrderByDescending(x => x.Tipo).ToList();
			string itensTxt = "";
			for (int i = CurrentPage; i < CurrentPage + itensPorPagina; i++)
			{
				if (itens.Count > i)
					itensTxt += $"\n* {itens[i].Name} - {itens[i].Preco}₹";
				else break;
			}
			var embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"Itens do RPGbot ({CurrentPage / itensPorPagina + 1}/{itens.Count / itensPorPagina + 1})" },
				Description = itensTxt,
				Footer = new EmbedFooterBuilder() { Text = "Nem todos os itens listados podem estar inclusos na aventura atual." }
			};

			var buttonComponent = new ComponentBuilder();
			buttonComponent.WithButton("← Anterior", $"PreviousPage:{(CurrentPage - itensPorPagina < 0 ? 0 : CurrentPage - itensPorPagina)}", ButtonStyle.Primary);
			if ((CurrentPage + itensPorPagina) / itensPorPagina <= itens.Count / itensPorPagina)
				buttonComponent.WithButton("Próxima →", $"NextPage:{CurrentPage + itensPorPagina}", ButtonStyle.Success);
			else
				buttonComponent.WithButton("Próxima →", $"NoButton", ButtonStyle.Secondary);

			await Context.Interaction.ModifyOriginalResponseAsync(msg =>
			{
				msg.Embed = embed.Build();
				msg.Components = buttonComponent.Build();
			});
		}

		[ComponentInteraction("NoButton")]
		public async Task NoButton()
		{
			await Context.Interaction.DeferAsync();
		}
		#endregion

		#region VER ARMADURAS
		[SlashCommand("verarmaduras", "Mostra todas as armaduras disponíveis no jogo até então")]
		public async Task VerArmaduras()
		{
			var buttonComponent = new ComponentBuilder().WithButton("← Anterior", $"NoButton", ButtonStyle.Secondary).WithButton("Próxima →", $"NextPageArmaduras:{itensPorPagina}", ButtonStyle.Success);

			List<Item> itens = itensController.GetAll().Result.ToList();
			itens = itens.Where(x => x.Defesa != 0).ToList();
			itens = itens.OrderBy(x => x.Name).ToList();
			string itensTxt = "";
			for (int i = 0; i < itensPorPagina; i++)
			{
				if (itens.Count > i)
					itensTxt += $"\n* {itens[i].Name} - {itens[i].Preco}₹";
				else break;
			}
			var embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"Itens do RPGbot (1/{itens.Count / itensPorPagina + 1})" },
				Description = itensTxt,
				Footer = new EmbedFooterBuilder() { Text = "Nem todos os itens listados podem estar inclusos na aventura atual." }
			};

			await RespondAsync("Itens!", ephemeral: true, components: buttonComponent.Build(), embed: embed.Build());
		}

		[ComponentInteraction("PreviousPageArmaduras:*", ignoreGroupNames: true)]
		public async Task PreviousPageArmaduras(string CurPage)
		{
			await DeferAsync();

			int CurrentPage = int.Parse(CurPage);

			var buttonComponent = new ComponentBuilder();
			if (!(CurrentPage - itensPorPagina < 0))
				buttonComponent.WithButton("← Anterior", $"PreviousPageArmaduras:{CurrentPage - itensPorPagina}", ButtonStyle.Primary);
			else
				buttonComponent.WithButton("← Anterior", $"NoButton", ButtonStyle.Secondary);
			buttonComponent.WithButton("Próxima →", $"NextPageArmaduras:{CurrentPage + itensPorPagina}", ButtonStyle.Success);

			List<Item> itens = itensController.GetAll().Result.ToList();
			itens = itens.Where(x => x.Defesa != 0).ToList();
			itens = itens.OrderBy(x => x.Name).ToList();
			string itensTxt = "";
			for (int i = CurrentPage; i < CurrentPage + itensPorPagina; i++)
			{
				if (itens.Count > i)
					itensTxt += $"\n* {itens[i].Name} - {itens[i].Preco}₹";
				else break;
			}
			var embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"Itens do RPGbot ({CurrentPage / itensPorPagina + 1}/{itens.Count / itensPorPagina + 1})" },
				Description = itensTxt,
				Footer = new EmbedFooterBuilder() { Text = "Nem todos os itens listados podem estar inclusos na aventura atual." }
			};

			await Context.Interaction.ModifyOriginalResponseAsync(msg =>
			{
				msg.Embed = embed.Build();
				msg.Components = buttonComponent.Build();
			});
		}

		[ComponentInteraction("NextPageArmaduras:*", ignoreGroupNames: true)]
		public async Task NextPageArmaduras(string CurPage)
		{
			await DeferAsync();

			int CurrentPage = int.Parse(CurPage);

			List<Item> itens = itensController.GetAll().Result.ToList();
			itens = itens.Where(x => x.Defesa != 0).ToList();
			itens = itens.OrderBy(x => x.Name).ToList();
			string itensTxt = "";
			for (int i = CurrentPage; i < CurrentPage + itensPorPagina; i++)
			{
				if (itens.Count > i)
					itensTxt += $"\n* {itens[i].Name} - {itens[i].Preco}₹";
				else break;
			}
			var embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"Itens do RPGbot ({CurrentPage / itensPorPagina + 1}/{itens.Count / itensPorPagina + 1})" },
				Description = itensTxt,
				Footer = new EmbedFooterBuilder() { Text = "Nem todos os itens listados podem estar inclusos na aventura atual." }
			};

			var buttonComponent = new ComponentBuilder();
			buttonComponent.WithButton("← Anterior", $"PreviousPageArmaduras:{(CurrentPage - itensPorPagina < 0 ? 0 : CurrentPage - itensPorPagina)}", ButtonStyle.Primary);
			if ((CurrentPage + itensPorPagina) / itensPorPagina <= itens.Count / itensPorPagina)
				buttonComponent.WithButton("Próxima →", $"NextPageArmaduras:{CurrentPage + itensPorPagina}", ButtonStyle.Success);
			else
				buttonComponent.WithButton("Próxima →", $"NoButton", ButtonStyle.Secondary);

			await Context.Interaction.ModifyOriginalResponseAsync(msg =>
			{
				msg.Embed = embed.Build();
				msg.Components = buttonComponent.Build();
			});
		}
		#endregion

		#region VER ARMAS
		[SlashCommand("verarmas", "Mostra todas as armas disponíveis no jogo até então")]
		public async Task VerArmas()
		{
			var buttonComponent = new ComponentBuilder().WithButton("← Anterior", $"NoButton", ButtonStyle.Secondary).WithButton("Próxima →", $"NextPageArmas:{itensPorPagina}", ButtonStyle.Success);

			List<Item> itens = itensController.GetAll().Result.ToList();
			itens = itens.Where(x => x.Dano != string.Empty).ToList();
			itens = itens.OrderBy(x => x.Name).ToList();
			string itensTxt = "";
			for (int i = 0; i < itensPorPagina; i++)
			{
				if (itens.Count > i)
					itensTxt += $"\n* {itens[i].Name} - {itens[i].Preco}₹";
				else break;
			}
			var embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"Itens do RPGbot (1/{itens.Count / itensPorPagina + 1})" },
				Description = itensTxt,
				Footer = new EmbedFooterBuilder() { Text = "Nem todos os itens listados podem estar inclusos na aventura atual." }
			};

			await RespondAsync("Itens!", ephemeral: true, components: buttonComponent.Build(), embed: embed.Build());
		}

		[ComponentInteraction("PreviousPageArmas:*", ignoreGroupNames: true)]
		public async Task PreviousPageArmas(string CurPage)
		{
			await DeferAsync();

			int CurrentPage = int.Parse(CurPage);

			var buttonComponent = new ComponentBuilder();
			if (!(CurrentPage - itensPorPagina < 0))
				buttonComponent.WithButton("← Anterior", $"PreviousPageArmas:{CurrentPage - itensPorPagina}", ButtonStyle.Primary);
			else
				buttonComponent.WithButton("← Anterior", $"NoButton", ButtonStyle.Secondary);
			buttonComponent.WithButton("Próxima →", $"NextPageArmas:{CurrentPage + itensPorPagina}", ButtonStyle.Success);

			List<Item> itens = itensController.GetAll().Result.ToList();
			itens = itens.Where(x => x.Dano != string.Empty).ToList();
			itens = itens.OrderBy(x => x.Name).ToList();
			string itensTxt = "";
			for (int i = CurrentPage; i < CurrentPage + itensPorPagina; i++)
			{
				if (itens.Count > i)
					itensTxt += $"\n* {itens[i].Name} - {itens[i].Preco}₹";
				else break;
			}
			var embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"Itens do RPGbot ({CurrentPage / itensPorPagina + 1}/{itens.Count / itensPorPagina + 1})" },
				Description = itensTxt,
				Footer = new EmbedFooterBuilder() { Text = "Nem todos os itens listados podem estar inclusos na aventura atual." }
			};

			await Context.Interaction.ModifyOriginalResponseAsync(msg =>
			{
				msg.Embed = embed.Build();
				msg.Components = buttonComponent.Build();
			});
		}

		[ComponentInteraction("NextPageArmas:*", ignoreGroupNames: true)]
		public async Task NextPageArmas(string CurPage)
		{
			await DeferAsync();

			int CurrentPage = int.Parse(CurPage);

			List<Item> itens = itensController.GetAll().Result.ToList();
			itens = itens.Where(x => x.Dano != string.Empty).ToList();
			itens = itens.OrderBy(x => x.Name).ToList();
			string itensTxt = "";
			for (int i = CurrentPage; i < CurrentPage + itensPorPagina; i++)
			{
				if (itens.Count > i)
					itensTxt += $"\n* {itens[i].Name} - {itens[i].Preco}₹";
				else break;
			}
			var embed = new EmbedBuilder()
			{
				Author = new EmbedAuthorBuilder() { Name = $"Itens do RPGbot ({CurrentPage / itensPorPagina + 1}/{itens.Count / itensPorPagina + 1})" },
				Description = itensTxt,
				Footer = new EmbedFooterBuilder() { Text = "Nem todos os itens listados podem estar inclusos na aventura atual." }
			};

			var buttonComponent = new ComponentBuilder();
			buttonComponent.WithButton("← Anterior", $"PreviousPageArmas:{(CurrentPage - itensPorPagina < 0 ? 0 : CurrentPage - itensPorPagina)}", ButtonStyle.Primary);
			if ((CurrentPage + itensPorPagina) / itensPorPagina <= itens.Count / itensPorPagina)
				buttonComponent.WithButton("Próxima →", $"NextPageArmas:{CurrentPage + itensPorPagina}", ButtonStyle.Success);
			else
				buttonComponent.WithButton("Próxima →", $"NoButton", ButtonStyle.Secondary);

			await Context.Interaction.ModifyOriginalResponseAsync(msg =>
			{
				msg.Embed = embed.Build();
				msg.Components = buttonComponent.Build();
			});
		}
		#endregion

		#region VER CLASSES E RAÇAS
		[SlashCommand("verclasse", "Mostra dados da classe selecionada")]
		public async Task VerClasse(string nome)
		{
			ClassesController classesController = new(new("Classes"));
			Classe classe = classesController.Get(RPGbotUtilities.FormatID(nome)).Result;
			if (classe == null)
			{
				await RespondAsync("Essa classe não existe.", ephemeral: true); return;
			}

			EmbedBuilder embed = new()
			{
				Author = new EmbedAuthorBuilder() { Name = classe.Mname },
				Description = $"♀️ {classe.Fname}  |  ♂️ {classe.Mname}\n\n{classe.Desc}",
				Color = 0x33cc33,
				Footer = new() { Text = $"Atributo {classe.HabilidadeP}  |  d{classe.Dice}" }

			};

			await RespondAsync($"Item:", embed: embed.Build(), ephemeral: true);
		}

		public async Task VerRaça(string nome)
		{
			RacasController racasController = new(new("Racas"));
			Raca raca = racasController.Get(RPGbotUtilities.FormatID(nome)).Result;
			if (raca == null)
			{
				await RespondAsync("Essa raça não existe.", ephemeral: true); return;
			}

			EmbedBuilder embed = new()
			{
				Author = new EmbedAuthorBuilder() { Name = raca.Mname },
				Description = $"♀️ {raca.Fname}  |  ♂️ {raca.Mname}",
				Color = 0xcc3333

			};

			await RespondAsync($"Item:", embed: embed.Build(), ephemeral: true);
		}
		#endregion
	}
}
