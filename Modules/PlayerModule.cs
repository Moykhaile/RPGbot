using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using RPGbot.Classes;
using RPGbot.db;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RPGbot.Modules
{
	public class PlayerModule : InteractionModuleBase<SocketInteractionContext>
	{
		[SlashCommand("ficha", "Apresenta a ficha do personagem")]
		public async Task Ficha()
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			await RespondAsync($"Sua ficha de personagem!", ephemeral: true, embed: PlayerResponse.GerarFicha(personagem));
		}

		[SlashCommand("vida", "Adiciona ou remove vida do personagem")]
		public async Task Vida([MaxValue(999), MinValue(-999)] int qntd)
		{
			if (qntd > 999 || qntd < -999 || qntd == 0)
			{
				await RespondAsync($"Valor inválido. Tente novamente com um número entre -999 e 999.", ephemeral: true);
				return;
			}

			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			int old_vida = personagem.Vida;
			personagem.Vida =
				personagem.Vida + qntd > personagem.VidaMax ? personagem.VidaMax :
				personagem.Vida + qntd < -4 ? -4 :
				personagem.Vida + qntd;

			new DBpersonagem().Put(personagem);

			await RespondAsync($"Vida do personagem alterada! Anterior: {old_vida}", embed: PlayerResponse.GerarFicha(personagem), ephemeral: true);
		}

		[SlashCommand("xp", "Adiciona pontos de experiência ao personagem")]
		public async Task XP([MaxValue(999), MinValue(1)] int qntd)
		{
			qntd = Math.Abs(qntd);
			if (qntd > 9999 || qntd == 0)
			{
				await RespondAsync($"Valor inválido. Tente novamente com um número entre 0 e 9999.", ephemeral: true); return;
			}

			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			int old_xp = personagem.XP;
			personagem.XP += qntd;

			new DBpersonagem().Put(personagem);

			await RespondAsync($"Pontos de experiência do personagem alterados! Anterior: {old_xp}", embed: PlayerResponse.GerarFicha(personagem), ephemeral: true);
		}

		[SlashCommand("saldo", "Adiciona dinheiro à carteira do personagem")]
		public async Task Saldo([MaxValue(9999), MinValue(-9999)] float qntd)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			float old_saldo = personagem.Saldo;
			personagem.Saldo += qntd;
			personagem.Saldo = personagem.Saldo < 0 ? 0 : personagem.Saldo;

			new DBpersonagem().Put(personagem);

			await RespondAsync($"Saldo do personagem alterado! Anterior: {old_saldo}", embed: PlayerResponse.GerarFicha(personagem), ephemeral: true);
		}

		[SlashCommand("addmagia", "Adiciona uma magia aos conhecimentos do personagem")]
		public async Task AddMagia(string magia)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!new DBclasse().Get(personagem.Classe).Magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}

			if (new DBmagia().Get(magia) == null)
			{
				await RespondAsync($"Magia ``{magia}`` não existe!", ephemeral: true); return;
			}

			if (personagem.Magias == null) personagem.Magias = new List<string>();

			if (personagem.Magias.Contains(PlayerResponse.FormatID(magia)))
			{
				await RespondAsync($"Seu personagem já conhece esta magia.", ephemeral: true); return;
			}
			if (personagem.Magias.Count >= PlayerResponse.niveisMagias[PlayerResponse.GerarNivel(personagem.XP) - 1])
			{
				await RespondAsync($"Seu personagem não tem experiência suficiente para aprender tantas magias! Explore o mundo e tente estudar novamente.", ephemeral: true); return;
			}

			personagem.Magias.Add(PlayerResponse.FormatID(magia));

			new DBpersonagem().Put(personagem);

			await RespondAsync($"Magia ``{new DBmagia().Get(magia).Name}`` adicionada ao personagem!", ephemeral: true, embed: PlayerResponse.GerarMagias(personagem.Magias, personagem));
		}

		[SlashCommand("removermagia", "Remove uma magia dos conhecimentos do personagem")]
		public async Task RemoverMagia(string magia)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!new DBclasse().Get(personagem.Classe).Magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}

			if (new DBmagia().Get(magia) == null)
			{
				await RespondAsync($"Magia ``{magia}`` não existe!", ephemeral: true); return;
			}

			if (personagem.Magias == null) personagem.Magias = new List<string>();

			if (!personagem.Magias.Contains(PlayerResponse.FormatID(magia)))
			{
				await RespondAsync($"Seu personagem não conhece esta magia.", ephemeral: true); return;
			}
			personagem.Magias.Remove(magia);

			new DBpersonagem().Put(personagem);

			await RespondAsync($"Magia ``{new DBmagia().Get(magia).Name}`` removida do personagem!", ephemeral: true);
		}

		[SlashCommand("magias", "Apresenta as mágias conhecidas pelo personagem")]
		public async Task Magias()
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!new DBclasse().Get(personagem.Classe).Magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}
			if (personagem.Magias == null || personagem.Magias.Count <= 0)
			{
				await RespondAsync($"O seu personagem não conhece nenhuma magia.", ephemeral: true); return;
			}

			await RespondAsync($"As magias do seu personagem!", ephemeral: true, embed: PlayerResponse.GerarMagias(personagem.Magias, personagem));
		}

		[SlashCommand("vermagia", "Apresenta as informações da magia, como tempo de conjuração, alcance, etc.")]
		public async Task VerMagia(string magia)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}
			if (!new DBclasse().Get(personagem.Classe).Magico)
			{
				await RespondAsync($"A classe do seu personagem não é mágica.", ephemeral: true); return;
			}
			if (new DBmagia().Get(magia) == null)
			{
				await RespondAsync($"Magia {magia} não existe!", ephemeral: true); return;
			}

			await RespondAsync($"Informações da magia:", ephemeral: true, embed: PlayerResponse.GerarMagia(magia));
		}

		[SlashCommand("inventario", "Apresenta o inventário do personagem")]
		public async Task Inventario()
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			if (personagem.Inventario == null || personagem.Inventario.Count <= 0)
			{
				await RespondAsync($"{personagem.Nome} não tem itens em seu inventário.", ephemeral: true); return;
			}

			await RespondAsync($"Seu inventário de personagem!", ephemeral: true, embed: PlayerResponse.GerarInventario(personagem));
		}

		[SlashCommand("additem", "Adiciona um item ao inventário do personagem")]
		public async Task AddItem(string nome, [Optional(), DefaultParameterValue(1), MinValue(1)] int quantidade)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			if (new DBitem().Get(nome) == null)
			{
				await RespondAsync($"Item ``{nome}`` não existe!", ephemeral: true); return;
			}
			if (personagem.Inventario == null) personagem.Inventario = new List<string>();

			if (personagem.Forca * PlayerResponse.pesoMod < (PlayerResponse.GerarPesoInventario(personagem) + new DBitem().Get(nome).Peso) * quantidade)
			{
				await RespondAsync($"Você não tem espaço para carregar este item na sua mochila!", ephemeral: true); return;
			}

			for (int i = 0; i < quantidade; i++)
				personagem.Inventario.Add(PlayerResponse.FormatID(nome));

			new DBpersonagem().Put(personagem);

			await RespondAsync($"Item adicionado ao inventário!", ephemeral: true, embed: PlayerResponse.GerarInventario(personagem));
		}

		[SlashCommand("removeritem", "Remove um item do inventário do personagem")]
		public async Task RemoverItem(string nome)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			if (new DBitem().Get(nome) == null)
			{
				await RespondAsync($"Item ``{nome}`` não existe!", ephemeral: true); return;
			}

			if (personagem.Inventario == null) personagem.Inventario = new List<string>();

			if (!personagem.Inventario.Contains(PlayerResponse.FormatID(nome)))
			{
				await RespondAsync($"Seu personagem não tem este item.", ephemeral: true); return;
			}
			string old_item = new DBitem().Get(nome).Name;
			personagem.Inventario.Remove(PlayerResponse.FormatID(nome));

			new DBpersonagem().Put(personagem);

			await RespondAsync($"Item ``{old_item}`` removido do inventário!", ephemeral: true, embed: PlayerResponse.GerarInventario(personagem));
		}

		[SlashCommand("compraritem", "Compra um item e o adiciona ao inventário do personagem")]
		public async Task ComprarItem([Required] string nome, [Optional, MinValue(1), DefaultParameterValue(0)] float preço)
		{
			try
			{
				Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
				if (personagem == null || personagem.Id == 0)
				{
					await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
				}

				if (new DBitem().Get(nome) == null)
				{
					await RespondAsync($"Item ``{nome}`` não existe!", ephemeral: true); return;
				}
				if (personagem.Inventario == null) personagem.Inventario = new List<string>();

				if (personagem.Forca * PlayerResponse.pesoMod < (PlayerResponse.GerarPesoInventario(personagem) + new DBitem().Get(nome).Peso))
				{
					await RespondAsync($"Você não tem espaço para carregar este item na sua mochila!", ephemeral: true); return;
				}
				Item item = new DBitem().Get(nome);
				if (personagem.Saldo < (preço > 0 ? preço : item.Preco))
				{
					await RespondAsync($"Você não tem dinheiro suficiente! Você tem ``{personagem.Saldo}`` de saldo, e o item custa ``{(preço > 0 ? preço : item.Preco)}``.", ephemeral: true); return;
				}

				if (preço > 0)
					personagem.Saldo -= preço;
				else
					personagem.Saldo -= item.Preco;
				personagem.Inventario.Add(nome);

				new DBpersonagem().Put(personagem);

				await RespondAsync($"Item comprado por {(preço > 0 ? preço : item.Preco)} moedas!", ephemeral: true, embed: PlayerResponse.GerarInventario(personagem));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		[SlashCommand("veritem", "Apresenta as informações do item como dano, peso, preço, etc.")]
		public async Task VerItem(string nome)
		{
			nome = PlayerResponse.FormatID(nome);

			Embed embed = PlayerResponse.GerarItem(nome);
			if (embed == null)
			{
				await RespondAsync($"Item {nome} não existe!", ephemeral: true); return;
			}

			await RespondAsync($"Item:", embed: embed, ephemeral: true);
		}
	}
}