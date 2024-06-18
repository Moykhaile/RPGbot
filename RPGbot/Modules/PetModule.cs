using Discord;
using Discord.Interactions;
using RPGbot.Classes;
using RPGbotAPI.Controllers;
using RPGbotAPI.Models;
using RPGbotAPI.Services;
using RPGbotLib;
using System.Runtime.InteropServices;

namespace RPGbot.Modules
{
	public class PetModule : InteractionModuleBase<SocketInteractionContext>
	{
		readonly PersonagensController personagensController = new(new PersonagemService("Personagens"));
		readonly PetsController petsController = new(new PetService("Pets"));
		readonly ItensController itensController = new(new ItemService("Itens"));

		[SlashCommand("pets", "Apresenta os pets do personagem")]
		public async Task Pets()
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			List<Pet> pets = personagem.Pets!;
			if (pets == null || pets.Count == 0)
			{
				await RespondAsync($"Seu personagem não tem pets.", ephemeral: true); return;
			}

			await RespondAsync("Seus pets", embed: RPGbotUtilities.GerarPets(personagem), ephemeral: true);
		}

		[SlashCommand("pet", "Apresenta detalhes de um pet específico do personagem")]
		public async Task Pet(string NomeDoPet)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			Pet? pet = personagem.Pets!.Find(x => x.Id == Utilities.FormatID(NomeDoPet));
			if (pet == null)
			{
				await RespondAsync($"Não foi encontrado um pet de nome ``{NomeDoPet}``", ephemeral: true);
				return;
			}

			await RespondAsync($"Seu pet, ``{pet.Nome}``", embed: RPGbotUtilities.GerarPet(pet, personagem.Nome), ephemeral: true);
		}

		[SlashCommand("addpet", "Adiciona um pet ao personagem")]
		public async Task AddPet(string especie, string NomeDoPet, Utilities.PetGenero genero)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			Pet pet = petsController.Get(especie).Result;
			if (pet == null)
			{
				await RespondAsync($"Não foi encontrado um pet da especie ``{especie}``", ephemeral: true);
				return;
			}

			pet.Nome = NomeDoPet;
			pet.Id = Utilities.FormatID(NomeDoPet);
			pet.Genero = genero;

			personagem.Pets ??= new List<Pet>();

			personagem.Pets.Add(pet);

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Pet ``{pet.Nome}`` adicionado ao seu personagem!", embed: RPGbotUtilities.GerarPets(personagem), ephemeral: true);
		}

		[SlashCommand("petvida", "Adiciona ou subtrai da vida do seu pet")]
		public async Task PetVida(string PetNome, int qntd)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			Pet? pet = personagem.Pets!.Find(x => x.Id == Utilities.FormatID(PetNome));
			if (pet == null)
			{
				await RespondAsync($"Não foi encontrado um pet de nome ``{PetNome}``", ephemeral: true);
				return;
			}

			int old_vida = pet.Vida;
			pet.Vida =
				pet.Vida + qntd > pet.MaxVida ? pet.MaxVida :
				pet.Vida + qntd < 0 ? 0 :
				pet.Vida + qntd;

			personagem.Pets[personagem.Pets.FindIndex(x => x.Id == Utilities.FormatID(PetNome))] = pet;

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Vida do pet ``{pet.Nome}`` alterada de ``{old_vida}`` para ``{pet.Vida}``", embed: RPGbotUtilities.GerarPets(personagem), ephemeral: true);
		}
		[SlashCommand("petadditem", "Adiciona um item ao seu pet")]
		public async Task PetAddItem(string PetNome, string NomeDoItem, [Optional(), DefaultParameterValue(1), MinValue(1)] int Quantidade)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			Pet? pet = personagem.Pets!.Find(x => x.Id == Utilities.FormatID(PetNome));
			if (pet == null)
			{
				await RespondAsync($"Não foi encontrado um pet de nome ``{PetNome}``", ephemeral: true);
				return;
			}

			Item item = itensController.Get(NomeDoItem).Result;

			if (item == null)
			{
				await RespondAsync($"Item ``{NomeDoItem}`` não existe!", ephemeral: true); return;
			}
			pet.Inventario ??= new List<Item>();

			if (pet.TamanhoMochila < (RPGbotUtilities.GerarPesoInventario(pet) + item.Peso) * Quantidade)
			{
				await RespondAsync($"Você não tem espaço para carregar este item na sua mochila!", ephemeral: true); return;
			}

			for (int i = 0; i < Quantidade; i++)
				pet.Inventario.Add(item);

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Item ``{item.Name}`` adicionado ao inventário do pet ``{pet.Nome}``!", ephemeral: true, embed: RPGbotUtilities.GerarPets(personagem));
		}

		[SlashCommand("petremoveritem", "Remove um item do inventário do pet")]
		public async Task PetRemoverItem(string PetNome, string NomeDoItem, [Optional(), DefaultParameterValue(1), MinValue(1)] int Quantidade)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null || personagem.Id == 0)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			Pet? pet = personagem.Pets!.Find(x => x.Id == Utilities.FormatID(PetNome));
			if (pet == null)
			{
				await RespondAsync($"Não foi encontrado um pet de nome ``{PetNome}``", ephemeral: true);
				return;
			}

			pet.Inventario ??= new List<Item>();

			Item item = pet.Inventario.Find(x => x.Id == Utilities.FormatID(NomeDoItem))!;

			if (!pet.Inventario.Contains(item))
			{
				await RespondAsync($"Seu pet não tem este item.", ephemeral: true); return;
			}
			string old_item = item.Name;

			for (int i = 0; i < Quantidade; i++)
				pet.Inventario.Remove(pet.Inventario.Find(x => x.Id == Utilities.FormatID(NomeDoItem))!);

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Item ``{old_item}`` removido do inventário de {pet.Nome}!", ephemeral: true, embed: RPGbotUtilities.GerarPet(pet, personagem.Nome));
		}
	}
}