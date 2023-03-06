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
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RPGbot.Modules
{
	public class MasterModule : InteractionModuleBase<SocketInteractionContext>
	{
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

		[RequireRole("Mestre")]
		[SlashCommand("editplayer", "Editar informação do personagem")]
		public async Task EditPlayer(IMentionable user, Atributos atributo, [Remainder] string valor)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = new DBpersonagem().Get((user as SocketGuildUser).Id.ToString());
			if (personagem == null || personagem.Id == 0)
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
		[SlashCommand("mostrarficha", "Apresenta a ficha de outro personagem")]
		public async Task MostrarFicha(IMentionable user)
		{
			if (!(user is SocketGuildUser))
			{
				await RespondAsync($"Usuário inválido.", ephemeral: true); return;
			}

			Personagem personagem = new DBpersonagem().Get((user as SocketGuildUser).Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			await RespondAsync($"Ficha de personagem de {(user as SocketGuildUser).DisplayName}!", ephemeral: true, embed: PlayerResponse.GerarFicha(personagem));
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
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}

			await RespondAsync($"Sua ficha de personagem!", ephemeral: true, embed: PlayerResponse.GerarFicha(personagem));
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
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{(user as SocketGuildUser).Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!new DBclasse().Get(personagem.Classe).Magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}
			if (personagem.Magias == null || personagem.Magias.Count <= 0)
			{
				await RespondAsync($"{personagem.Nome} não conhece nenhuma magia.", ephemeral: true); return;
			}

			await RespondAsync($"As magias do seu personagem!", ephemeral: true, embed: PlayerResponse.GerarMagias(personagem.Magias, personagem));
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
					embed.AddField(
						$"{personagem.Nome}",
						$"```❤️ {personagem.Vida}/{personagem.VidaMax}\n🌟 {personagem.XP}/{PlayerResponse.niveisXP[PlayerResponse.GerarNivel(personagem.XP) - 1]} lvl {PlayerResponse.GerarNivel(personagem.XP)}\n💰 {personagem.Saldo}```"
					);
				}
			}

			await RespondAsync("Resumo:", embed: embed.Build(), ephemeral: true);
		}

		// -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB -- DB

		//[RequireRole("Mestre")]
		[SlashCommand("dbadditem", "Adiciona item à base de dados do RPGbot")]
		public async Task AddItem(string Name, string Tipo, float Peso, float Preco)
		{
			try
			{
				string itensstring = File.ReadAllText("../../db/g_data/itens.json");
				List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(itensstring);

				itemList.Add(new Item
				{
					Name = Name,
					Peso = Peso,
					Dano = string.Empty,
					Defesa = 0,
					Tipo = Tipo,
					ModNome = string.Empty,
					Propriedades = string.Empty,
					Preco = Preco
				});
				var itemOutput = JsonConvert.SerializeObject(itemList, Formatting.Indented);
				File.WriteAllText("../../db/g_data/itens.json", itemOutput);

				await RespondAsync($"Item '{Name}' adicionado à base de dados!");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
		[SlashCommand("dbaddarma", "Adiciona arma à base de dados do RPGbot")]
		public async Task AddArma(string Name, float Peso, string Dano, float Preco, string Tipo, string Propriedades)
		{
			try
			{
				string itensstring = File.ReadAllText("../../db/g_data/itens.json");
				List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(itensstring);

				itemList.Add(new Item
				{
					Name = Name,
					Peso = Peso,
					Dano = Dano,
					Defesa = 0,
					Tipo = Tipo,
					ModNome = string.Empty,
					Propriedades = Propriedades,
					Preco = Preco
				});
				var itemOutput = JsonConvert.SerializeObject(itemList, Formatting.Indented);
				File.WriteAllText("../../db/g_data/itens.json", itemOutput);

				await RespondAsync($"Item '{Name}' adicionado à base de dados!");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
		[SlashCommand("dbaddarmadura", "Adiciona armadura à base de dados do RPGbot")]
		public async Task AddArmadura(string Name, float Peso, int Defesa, float Preco, string Tipo, string ModNome, string Propriedades)
		{
			try
			{
				if (ModNome == ".") ModNome = "";
				string itensstring = File.ReadAllText("../../db/g_data/itens.json");
				List<Item> itemList = JsonConvert.DeserializeObject<List<Item>>(itensstring);

				itemList.Add(new Item
				{
					Name = Name,
					Peso = Peso,
					Dano = string.Empty,
					Defesa = Defesa,
					Tipo = Tipo,
					ModNome = ModNome,
					Propriedades = Propriedades,
					Preco = Preco
				});
				var itemOutput = JsonConvert.SerializeObject(itemList, Formatting.Indented);
				File.WriteAllText("../../db/g_data/itens.json", itemOutput);

				await RespondAsync($"Item '{Name}' adicionado à base de dados!");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
		[RequireRole("Mestre")]
		[SlashCommand("dbaddmagia", "Adiciona magia à base de dados do RPGbot")]
		public async Task AddMagia(string Nome, string Lançamento, string Alcance, string Componentes, string Duração, string Descrição)
		{
			Magia magia = new Magia() { Name = Nome, Casting = Lançamento, Range = Alcance, Components = Componentes, Duration = Duração, Description = Descrição };

			new DBmagia().Post(magia, magia.Name);

			await RespondAsync($"Magia ``{magia.Name}`` adicionada à base");
		}
	}
}