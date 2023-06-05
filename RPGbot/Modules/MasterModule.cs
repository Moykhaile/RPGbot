using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using RPGbot.Classes;
using RPGbotAPI.Controllers;
using RPGbotAPI.Models;
using RPGbotAPI.Services;
using System.Reflection;

namespace RPGbot.Modules
{
	public class MasterModule : InteractionModuleBase<SocketInteractionContext>
	{
		public enum Dados
		{
			Nome,
			Jogador,
			Genero,
			Raca,
			Classe,
			Sexualidade,
			Posicao,
			Idade,
			Peso,
			Altura,
			Forca,
			Destreza,
			Inteligencia,
			Constituicao,
			Sabedoria,
			Carisma,
			VidaMax,
			Vida,
			Saldo,
			XP
		}

		readonly PersonagensController personagensController = new(new PersonagemService("Personagens"));

		[SlashCommand("editplayer", "Editar informação do personagem")]
		public async Task EditPlayer(IMentionable user, Dados atributo, string valor)
		{
			if (user is not SocketGuildUser)
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = personagensController.Get(((SocketGuildUser)user).Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID ``{((SocketGuildUser)user).Id}`` não encontrado.", ephemeral: true); return;
			}

			PropertyInfo propertyInfo = personagem.GetType().GetProperty(atributo.ToString())!;
			if (propertyInfo == null) { await RespondAsync($"Propriedade ``{atributo}`` não encontrada no personagem {personagem.Nome}!", ephemeral: true); return; }

			propertyInfo.SetValue(personagem, Convert.ChangeType(valor, propertyInfo.PropertyType), null);

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Propriedade ``{atributo}`` do personagem ``{personagem.Nome}`` alterada.", ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("mestrevida", "Altera a vida de outro personagem")]
		public async Task MestreVida(IMentionable user, [MaxValue(999), MinValue(-999)] int qntd)
		{
			if (user is not SocketGuildUser)
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = personagensController.Get(((SocketGuildUser)user).Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID ``{((SocketGuildUser)user).Id}`` não encontrado.", ephemeral: true); return;
			}

			int old_vida = personagem.Vida;
			personagem.Vida =
				personagem.Vida + qntd > personagem.VidaMax ? personagem.VidaMax :
				personagem.Vida + qntd < -4 ? -4 :
				personagem.Vida + qntd;

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Vida do personagem alterada! Anterior: {old_vida}", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("mestrexp", "Altera o XP de outro personagem")]
		public async Task MestreXP(IMentionable user, [MaxValue(19999), MinValue(-999)] int qntd)
		{
			if (user is not SocketGuildUser)
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			qntd = Math.Abs(qntd);

			Personagem personagem = personagensController.Get(((SocketGuildUser)user).Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID ``{((SocketGuildUser)user).Id}`` não encontrado.", ephemeral: true); return;
			}

			int old_xp = personagem.XP;
			personagem.XP += qntd;

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Pontos de experiência do personagem alterados! Anterior: {old_xp}", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("mestresaldo", "Altera o saldo de outro personagem")]
		public async Task MestreSaldo(IMentionable user, [MaxValue(9999), MinValue(-9999)] float qntd)
		{
			if (user is not SocketGuildUser)
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = personagensController.Get(((SocketGuildUser)user).Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID ``{((SocketGuildUser)user).Id}`` não encontrado.", ephemeral: true); return;
			}

			float old_saldo = personagem.Saldo;
			personagem.Saldo += qntd;
			personagem.Saldo = personagem.Saldo < 0 ? 0 : personagem.Saldo;

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Saldo do personagem alterado! Anterior: {old_saldo}", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("mostrarficha", "Apresenta a ficha de outro personagem")]
		public async Task MostrarFicha(IMentionable user)
		{
			if (user is not SocketGuildUser)
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = personagensController.Get(((SocketGuildUser)user).Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID ``{((SocketGuildUser)user).Id}`` não encontrado.", ephemeral: true); return;
			}

			await RespondAsync($"Ficha de personagem de {((SocketGuildUser)user).DisplayName}!", ephemeral: true, embed: RPGbotUtilities.GerarFicha(personagem));
		}
		[RequireRole("Mestre")]
		[SlashCommand("mostrarpericias", "Apresenta as perícias de outro personagem")]
		public async Task MostrarPericias(IMentionable user)
		{
			if (user is not SocketGuildUser)
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = personagensController.Get(((SocketGuildUser)user).Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID ``{((SocketGuildUser)user).Id}`` não encontrado.", ephemeral: true); return;
			}

			await RespondAsync($"Ficha de personagem de {((SocketGuildUser)user).DisplayName}!", ephemeral: true, embed: RPGbotUtilities.GerarPericias(personagem));
		}

		[RequireRole("Mestre")]
		[SlashCommand("mostrarinventario", "Apresenta o inventário de outro personagem")]
		public async Task MostrarInventario(IMentionable user)
		{
			if (user is not SocketGuildUser)
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = personagensController.Get(((SocketGuildUser)user).Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID ``{((SocketGuildUser)user).Id}`` não encontrado.", ephemeral: true); return;
			}

			if (personagem.Inventario == null)
			{
				await RespondAsync($"{personagem.Nome} não tem itens em seu inventário.", ephemeral: true); return;
			}

			await RespondAsync($"Inventário de {((SocketGuildUser)user).DisplayName}!", ephemeral: true, embed: RPGbotUtilities.GerarInventario(personagem));
		}

		[RequireRole("Mestre")]
		[SlashCommand("mostrarmagias", "Apresenta as magias de outro personagem")]
		public async Task MostrarMagias(IMentionable user)
		{
			if (user is not SocketGuildUser)
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = personagensController.Get(((SocketGuildUser)user).Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID ``{((SocketGuildUser)user).Id}`` não encontrado.", ephemeral: true); return;
			}
			if (!personagem.Classe!.Magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}
			if (personagem.Magias == null)
			{
				await RespondAsync($"{personagem.Nome} não conhece nenhuma magia.", ephemeral: true); return;
			}

			await RespondAsync($"As magias de {((SocketGuildUser)user).DisplayName}!", ephemeral: true, embed: RPGbotUtilities.GerarMagias(personagem.Magias, personagem));
		}

		[RequireRole("Mestre")]
		[SlashCommand("resumo", "Apresenta um resumo dos personagens")]
		public async Task Resumo()
		{
			List<Personagem> personagens = personagensController.GetAll().Result;

			var embed = new EmbedBuilder()
			{
				Footer = new EmbedFooterBuilder() { Text = $"{personagens.Count - 1} personagens!" },
				Color = 0xffffff
			};
			foreach (var personagem in personagens)
			{
				if (personagem._Id != 0)
				{
					if (personagem._Id != Context.User.Id)
						embed.AddField(
							$"{personagem.Nome}",
							$"```❤️ {personagem.Vida}/{personagem.VidaMax}\n🌟 {personagem.XP}/{RPGbotUtilities.NiveisXP[RPGbotUtilities.GerarNivel(personagem.XP) - 1]} lvl {RPGbotUtilities.GerarNivel(personagem.XP)}\n💰 {personagem.Saldo}```",
							inline: true
						);
				}
			}

			await RespondAsync("Resumo:", embed: embed.Build(), ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("exaustar", "Adiciona ou remove exaustão do personagem")]
		public async Task Exaustar(IMentionable user, [MinValue(-5), MaxValue(5)] int qntd)
		{
			if (user is not SocketGuildUser)
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = personagensController.Get(((SocketGuildUser)user).Id).Result;
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID ``{((SocketGuildUser)user).Id}`` não encontrado.", ephemeral: true); return;
			}

			personagem.Exaustão += qntd;
			if (personagem.Exaustão > 5) personagem.Exaustão = 5;
			if (personagem.Exaustão < 0) personagem.Exaustão = 0;

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync($"Exaustão alterada para {personagem.Nome}!", embed: RPGbotUtilities.GerarFicha(personagem), ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("mostrarpets", "Apresenta os pets de um personagem")]
		public async Task MostrarPets(IMentionable user)
		{
			if (user is not SocketGuildUser)
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = personagensController.Get(((SocketGuildUser)user).Id).Result;
			if (personagem == null || personagem._Id == 0)
			{
				await RespondAsync($"Personagem de ID ``{((SocketGuildUser)user).Id}`` não encontrado.", ephemeral: true); return;
			}

			List<Pet> pets = personagem.Pets!;
			if (pets == null || pets.Count == 0)
			{
				await RespondAsync($"O personagem ``{personagem.Nome}`` não tem pets.", ephemeral: true); return;
			}

			await RespondAsync($"Os pets de {personagem.Nome}", embed: RPGbotUtilities.GerarPets(personagem, true), ephemeral: true);
		}
	}
}