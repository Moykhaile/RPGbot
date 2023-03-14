using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGbot.Classes;
using RPGbot.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

		//[RequireRole("Mestre")]
		[SlashCommand("editplayer", "Editar informação do personagem")]
		public async Task EditPlayer(IMentionable user, Dados atributo, string valor)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = new DBpersonagem().Get((user as SocketGuildUser).Id.ToString());
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			JObject playerObj = JObject.Parse(JsonConvert.SerializeObject(personagem));
			if (playerObj.GetValue(atributo.ToString()) == null)
			{
				await RespondAsync($"Atributo \"{atributo}\" não encontrado.", ephemeral: true); return;
			}

			playerObj[atributo.ToString()] = valor;

			new DBpersonagem().Put(JsonConvert.DeserializeObject<Personagem>(playerObj.ToString()));
			await RespondAsync($"Valor do atributo {atributo} alterado!", ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("mestrevida", "Altera a vida de outro personagem")]
		public async Task MestreVida(IMentionable user, [MaxValue(999), MinValue(-999)] int qntd)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = new DBpersonagem().Get((user as SocketGuildUser).Id.ToString());
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			int old_vida = personagem.Vida;
			personagem.Vida =
				personagem.Vida + qntd > personagem.VidaMax ? personagem.VidaMax :
				personagem.Vida + qntd < -4 ? -4 :
				personagem.Vida + qntd;

			new DBpersonagem().Put(personagem);

			await RespondAsync($"Vida do personagem alterada! Anterior: {old_vida}", embed: PlayerResponse.GerarFicha(personagem), ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("mestrexp", "Altera o XP de outro personagem")]
		public async Task MestreXP(IMentionable user, [MaxValue(19999), MinValue(-999)] int qntd)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			qntd = Math.Abs(qntd);

			Personagem personagem = new DBpersonagem().Get((user as SocketGuildUser).Id.ToString());
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			int old_xp = personagem.XP;
			personagem.XP += qntd;

			new DBpersonagem().Put(personagem);

			await RespondAsync($"Pontos de experiência do personagem alterados! Anterior: {old_xp}", embed: PlayerResponse.GerarFicha(personagem), ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("mestresaldo", "Altera o saldo de outro personagem")]
		public async Task MestreSaldo(IMentionable user, [MaxValue(9999), MinValue(-9999)] float qntd)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = new DBpersonagem().Get((user as SocketGuildUser).Id.ToString());
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			float old_saldo = personagem.Saldo;
			personagem.Saldo += qntd;
			personagem.Saldo = personagem.Saldo < 0 ? 0 : personagem.Saldo;

			new DBpersonagem().Put(personagem);

			await RespondAsync($"Saldo do personagem alterado! Anterior: {old_saldo}", embed: PlayerResponse.GerarFicha(personagem), ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("mostrarficha", "Apresenta a ficha de outro personagem")]
		public async Task MostrarFicha(IMentionable user)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = new DBpersonagem().Get((user as SocketGuildUser).Id.ToString());
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			await RespondAsync($"Ficha de personagem de {(user as SocketGuildUser).DisplayName}!", ephemeral: true, embed: PlayerResponse.GerarFicha(personagem));
		}
		[RequireRole("Mestre")]
		[SlashCommand("mostrarpericias", "Apresenta as perícias de outro personagem")]
		public async Task MostrarPericias(IMentionable user)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = new DBpersonagem().Get((user as SocketGuildUser).Id.ToString());
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			await RespondAsync($"Ficha de personagem de {(user as SocketGuildUser).DisplayName}!", ephemeral: true, embed: PlayerResponse.GerarPericias(personagem));
		}

		[RequireRole("Mestre")]
		[SlashCommand("mostrarinventario", "Apresenta o inventário de outro personagem")]
		public async Task MostrarInventario(IMentionable user)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = new DBpersonagem().Get((user as SocketGuildUser).Id.ToString());
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			if (personagem.Inventario == null)
			{
				await RespondAsync($"{personagem.Nome} não tem itens em seu inventário.", ephemeral: true); return;
			}

			await RespondAsync($"Inventário de {(user as SocketGuildUser).DisplayName}!", ephemeral: true, embed: PlayerResponse.GerarInventario(personagem));
		}

		[RequireRole("Mestre")]
		[SlashCommand("mostrarmagias", "Apresenta as magias de outro personagem")]
		public async Task MostrarMagias(IMentionable user)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = new DBpersonagem().Get((user as SocketGuildUser).Id.ToString());
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!new DBclasse().Get(personagem.Classe).Magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}
			if (personagem.Magias == null)
			{
				await RespondAsync($"{personagem.Nome} não conhece nenhuma magia.", ephemeral: true); return;
			}

			await RespondAsync($"As magias de {(user as SocketGuildUser).DisplayName}!", ephemeral: true, embed: PlayerResponse.GerarMagias(personagem.Magias, personagem));
		}

		[RequireRole("Mestre")]
		[SlashCommand("resumo", "Apresenta um resumo dos personagens")]
		public async Task Resumo()
		{
			List<Personagem> personagens = new DBpersonagem().GetAll();

			var embed = new EmbedBuilder()
			{
				Footer = new EmbedFooterBuilder() { Text = $"{personagens.Count - 1} personagens!" },
				Color = 0xffffff
			};
			foreach (var personagem in personagens)
			{
				if (personagem.Id != 0)
				{
					if (personagem.Id != Context.User.Id)
						embed.AddField(
							$"{personagem.Nome}",
							$"```❤️ {personagem.Vida}/{personagem.VidaMax}\n🌟 {personagem.XP}/{PlayerResponse.niveisXP[PlayerResponse.GerarNivel(personagem.XP) - 1]} lvl {PlayerResponse.GerarNivel(personagem.XP)}\n💰 {personagem.Saldo}```",
							inline: true
						);
				}
			}

			await RespondAsync("Resumo:", embed: embed.Build(), ephemeral: true);
		}
	}
}