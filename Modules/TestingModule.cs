using Discord.Interactions;
using RPGbot.Classes;
using RPGbot.db;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RPGbot.Modules
{
	public class TestingModule : InteractionModuleBase<SocketInteractionContext>
	{
		[SlashCommand("testeadditem", "teste")]
		public async Task AddItem(string nome, [Optional, DefaultParameterValue(1), MinValue(1)] int quantidade)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem == null)
			{
				await RespondAsync($"Personagem de ID \"{Context.User.Id}\" não encontrado.", ephemeral: true); return;
			}

			if (new DBitem().Get(nome) == null)
			{
				await RespondAsync($"Item ``{nome}`` não existe!", ephemeral: true); return;
			}

			Item item = new DBitem().Get(nome);

			if (personagem.Itens == null) personagem.Itens = new List<ItemInventario>();

			if (personagem.Itens.Any(e => e.Nome == item.Nome))
				personagem.Itens.Find(e => e.Nome == item.Nome).qntd += quantidade;
			else
				personagem.Itens.Add(new ItemInventario { Nome = item.Nome, qntd = quantidade });

			new DBpersonagem().Put(personagem);

			await RespondAsync($"Novo sistema de inventário: \n\n{new DBpersonagem().Get(personagem.Id.ToString()).Itens}");
		}
	}
}
