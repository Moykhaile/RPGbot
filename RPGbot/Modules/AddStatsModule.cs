using Discord;
using Discord.Interactions;
using RPGbot.Classes;
using RPGbotAPI.Controllers;
using RPGbotAPI.Models;
using RPGbotAPI.Services;
using System.ComponentModel;

namespace RPGbot.Modules
{
	public class AddStatsModule : InteractionModuleBase<SocketInteractionContext>
	{
		readonly PersonagensController personagensController = new(new PersonagemService());

		[SlashCommand("addstats", "Gera seus stats (força, destreza, carisma, etc) com base nas suas escolhas")]
		public async Task HandleAddStatsCommand(int Força, int Destreza, int Constituição, int Inteligência, int Sabedoria, int carisma)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem == null)
			{
				ErrorModule.PersonagemNotFound(Context, Context.User.Id.ToString()); return;
			}
			if (personagem.GeneratedStats)
			{
				await RespondAsync($"Você já utilizou seus pontos!", ephemeral: true);
				return;
			}

			int total = Força + Destreza + Constituição + Inteligência + Sabedoria + carisma;
			if (total > 27)
			{
				await RespondAsync($"Você só tem 27 pontos para gastar, porém tentou utilizar {total} pontos!", ephemeral: true);
				return;
			}
			if (total < 27)
			{
				await RespondAsync($"Você tem 27 pontos para gastar, porém tentou utilizar somente {total} pontos!", ephemeral: true);
				return;
			}

			personagem.Forca += Força + 8;
			personagem.Destreza += Destreza + 8;
			personagem.Constituicao += Constituição + 8;
			personagem.Inteligencia += Inteligência + 8;
			personagem.Sabedoria += Sabedoria + 8;
			personagem.Carisma += carisma + 8;

			personagem.GeneratedStats = true;

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync($"Stats gerados! Você não poderá gerar outros depois. Para ver seus modificadores, utilize o comando **/ficha**.", ephemeral: true);
		}
	}
}
