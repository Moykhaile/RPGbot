using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGbot.Classes;
using RPGbot.db;
using System.Threading.Tasks;

namespace RPGbot.Modules
{
	public class MasterModule : InteractionModuleBase<SocketInteractionContext>
	{
		public InteractionService Commands { get; set; }

		public MasterModule(DiscordSocketClient client) => _client = client;
		private readonly DiscordSocketClient _client;

		public enum Atributos
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
		public async Task EditPlayer(IMentionable user, Atributos atributo, [Remainder] string valor)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Player player = new Player().GetPlayer((user as SocketGuildUser).Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			JObject playerObj = JObject.Parse(JsonConvert.SerializeObject(player));
			if (playerObj.GetValue(atributo.ToString()) == null)
			{
				await RespondAsync($"Atributo \"{atributo}\" não encontrado.", ephemeral: true); return;
			}

			playerObj[atributo.ToString()] = valor;

			DbHandler.SavePlayer((user as SocketGuildUser).Id.ToString(), JsonConvert.DeserializeObject<Player>(playerObj.ToString()));
			await RespondAsync($"Valor do atributo {atributo} alterado!", ephemeral: true);
		}

		[RequireRole("Mestre")]
		[SlashCommand("mostrarficha", "Apresenta a ficha de outro personagem")]
		public async Task MostrarFicha(IMentionable user)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Player player = new Player().GetPlayer((user as SocketGuildUser).Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			await RespondAsync($"Ficha de personagem de {(user as SocketGuildUser).DisplayName}!", ephemeral: true, embed: PlayerResponse.GerarFicha(player));
		}

		[RequireRole("Mestre")]
		[SlashCommand("mostrarinventario", "Apresenta o inventário de outro personagem")]
		public async Task MostrarInventario(IMentionable user)
		{
			Player player = new Player().GetPlayer((user as SocketGuildUser).Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			Inventory inventory = new Inventory((user as SocketGuildUser).Id.ToString());
			if (inventory.Items.Count == 0)
			{
				await RespondAsync($"{player.Nome} não tem itens em seu inventário.", ephemeral: true); return;
			}

			await RespondAsync($"O inventário do personagem!", ephemeral: true, embed: PlayerResponse.GerarInventario(inventory.Items, player));
		}

		[RequireRole("Mestre")]
		[SlashCommand("mostrarmagias", "Apresenta as magias de outro personagem")]
		public async Task MostrarMagias(IMentionable user)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Player player = new Player().GetPlayer((user as SocketGuildUser).Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!new PlayerClass().GetClass(player.Classe).magico)
			{
				await RespondAsync($"A classe deste personagem não é mágica.", ephemeral: true); return;
			}
			if (player.Magias == null)
			{
				await RespondAsync($"Este personagem não conhece nenhuma magia.", ephemeral: true); return;
			}

			await RespondAsync($"Magias de {(user as SocketGuildUser).DisplayName}", ephemeral: true, embed: PlayerResponse.GerarMagias(player.Magias, player));
		}

		[RequireRole("Mestre")]
		[SlashCommand("additem", "Adiciona item a base de dados do RPGbot")]
		public async Task AddItem(IMentionable user)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Player player = new Player().GetPlayer((user as SocketGuildUser).Id.ToString());
			if (player.Nome == null)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!new PlayerClass().GetClass(player.Classe).magico)
			{
				await RespondAsync($"A classe deste personagem não é mágica.", ephemeral: true); return;
			}
			if (player.Magias == null)
			{
				await RespondAsync($"Este personagem não conhece nenhuma magia.", ephemeral: true); return;
			}

			await RespondAsync($"Magias de {(user as SocketGuildUser).DisplayName}", ephemeral: true, embed: PlayerResponse.GerarMagias(player.Magias, player));
		}
	}
}
