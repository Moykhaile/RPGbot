using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGbot.Classes;
using System.Windows.Forms;
using RPGbot.Racas;
using Discord.Rest;
using Microsoft.Extensions.Options;
using RPGbot.db;

namespace RPGbot.Modules
{
	public class AddPlayerModule : InteractionModuleBase<SocketInteractionContext>
	{
		public InteractionService Commands { get; set; }

		[SlashCommand("add", "Adiciona um personagem ao jogo. Só é permitido 1 personagem por jogador")]
		public async Task HandleAddCommand()
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome != null)
			{
				await RespondAsync($"Você já tem um personagem! {player.Nome} já existe.", ephemeral: true); return;
			}

			var classeMenu = new SelectMenuBuilder()
			{
				CustomId = "classeMenu",
				Placeholder = "Escolha sua classe"
			};
			foreach (PlayerClass classe in PlayerClasses.GetClasses())
			{
				classeMenu.AddOption(classe.Mname, classe.Mname);
			}

			var racaMenu = new SelectMenuBuilder()
			{
				CustomId = "racaMenu",
				Placeholder = "Escolha sua raça"
			};
			foreach (PlayerRaca raca in PlayerRacas.GetRacas())
			{
				racaMenu.AddOption(raca.Mname, raca.Mname);
			}

			var generoMenu = new SelectMenuBuilder()
			{
				CustomId = "generoMenu",
				Placeholder = "Escolha o gênero"
			}
			.AddOption("Masculino", "Masculino")
			.AddOption("Feminino", "Feminino")
			.AddOption("Prefiro não dizer/Outro", "Outro");

			var sexualidadeMenu = new SelectMenuBuilder()
			{
				CustomId = "sexualidadeMenu",
				Placeholder = "Escolha a sexualidade"
			}
			.AddOption("Heterossexual", "Heterossexual")
			.AddOption("Homossexual", "Homossexual")
			.AddOption("Panssexual", "Panssexual")
			.AddOption("Assexual", "Assexual")
			.AddOption("Outro", "Outro");

			var posicaoMenu = new SelectMenuBuilder()
			{
				CustomId = "posicaoMenu",
				Placeholder = "Escolha a posição de batalha"
			}
			.AddOption("Dano", "Dano")
			.AddOption("Especialista", "Especialista")
			.AddOption("Suporte", "Suporte")
			.AddOption("Tanque", "Tanque");

			var component = new ComponentBuilder()
			.WithSelectMenu(classeMenu)
			.WithSelectMenu(racaMenu)
			.WithSelectMenu(generoMenu)
			.WithSelectMenu(sexualidadeMenu)
			.WithSelectMenu(posicaoMenu);

			await Context.Channel.SendMessageAsync($"{Context.User.Username} está criando um personagem!");

			await RespondAsync($"Vamos criar o seu personagem!\n\nEscolha as informações respectivas do seu novo personagem e aperte o botão.", components: component.Build(), ephemeral: true);

			var modalButton = new ButtonBuilder()
			{
				CustomId = "modalButton",
				Label = "Enviar!",
				Style = ButtonStyle.Success
			};

			var buttonComponent = new ComponentBuilder().WithButton(modalButton);

			await Context.Channel.SendMessageAsync("Clique aqui quando terminar!", components: buttonComponent.Build());

			DbHandler.SavePlayer(Context.User.Id.ToString(), player);
		}

		[ComponentInteraction("classeMenu")]
		public async Task HandleClasseMenu(string selected)
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			player.Classe = selected;
			DbHandler.SavePlayer(Context.User.Id.ToString(), player);
			await RespondAsync($"Classe {selected} selecionada!", ephemeral: true);
		}
		[ComponentInteraction("racaMenu")]
		public async Task HandleRacaMenu(string selected)
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			player.Raca = selected;
			DbHandler.SavePlayer(Context.User.Id.ToString(), player);
			await RespondAsync($"Raça {selected} selecionada!", ephemeral: true);
		}
		[ComponentInteraction("generoMenu")]
		public async Task HandleGeneroMenu(string selected)
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			player.Genero = selected;
			DbHandler.SavePlayer(Context.User.Id.ToString(), player);
			await RespondAsync($"Gênero {selected} selecionado!", ephemeral: true);
		}
		[ComponentInteraction("sexualidadeMenu")]
		public async Task HandleSexualidadeMenu(string selected)
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			player.Sexualidade = selected;
			DbHandler.SavePlayer(Context.User.Id.ToString(), player);
			await RespondAsync($"Sexualidade {selected} selecionada!", ephemeral: true);
		}
		[ComponentInteraction("posicaoMenu")]
		public async Task HandlePosicaoMenu(string selected)
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			player.Posicao = selected;
			DbHandler.SavePlayer(Context.User.Id.ToString(), player);
			await RespondAsync($"Posição {selected} selecionada!", ephemeral: true);
		}

		[ComponentInteraction("modalButton")]
		public async Task HandleModalButton()
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());
			if (player.Nome != null)
			{
				await RespondAsync($"Você já tem um personagem! {player.Nome} já existe.", ephemeral: true); return;
			}
			if (player.Classe == null || player.Raca == null || player.Sexualidade == null || player.Genero == null || player.Posicao == null)
			{
				await RespondAsync($"Responda a todas os menus acima!", ephemeral: true); return;
			}

			await RespondWithModalAsync<AddModal>("addmodal");
		}

		[ModalInteraction("addmodal")]
		public async Task HandleAddModal(AddModal modal)
		{
			Player player = new Player().GetPlayer(Context.User.Id.ToString());

			player.Jogador = modal.jogador;
			player.Nome = modal.name;
			player.Idade = int.Parse(modal.age);
			player.Peso = int.Parse(modal.weight);
			player.Altura = int.Parse(modal.height);

			player = generatePlayer(player);

			DbHandler.SavePlayer(Context.User.Id.ToString(), player);

			await RespondAsync("Personagem criado! Use ``/ficha`` para ver a ficha do seu personagem ✅");
		}

		Player generatePlayer(Player player)
		{
			player.Sabedoria = jogarDadosAtributos();
			player.Forca = jogarDadosAtributos();
			player.Destreza = jogarDadosAtributos();
			player.Constituicao = jogarDadosAtributos();
			player.Inteligencia = jogarDadosAtributos();
			player.Carisma = jogarDadosAtributos();

			PlayerClass playerClass = new PlayerClass().GetClass(player.Classe);

			player.Vida = jogarDadosVida(playerClass.Dice);
			player.VidaMax = player.Vida;

			player.Saldo = jogarDadosSaldo(playerClass.saldoDice, playerClass.saldoDiceNum, playerClass.saldoDiceMod);

			return player;
		}

		Random rnd = new Random();
		int jogarDadosSaldo(int dice, int num, int mod)
		{
			var resultados = new List<int> { };
			for (int i = 0; i < num; i++)
			{
				resultados.Add(rnd.Next(1, dice + 1));
			}

			int resultado = 0;
			foreach (var item in resultados)
				resultado += item;

			return resultado * mod;
		}
		int jogarDadosVida(int y)
		{
			int resultado = rnd.Next(1, y + 1);

			return resultado;
		}
		int jogarDadosAtributos()
		{
			var resultados = new List<int> { };
			for (int i = 0; i < 4; i++)
			{
				resultados.Add(rnd.Next(1, 7));
			}
			resultados.Remove(resultados.Min());

			int resultado = 0;
			foreach (var item in resultados)
				resultado += item;

			return resultado;
		}
		//4d6dl1
	}
}

public class AddModal : IModal
{
	public string Title => "Crie seu personagem!";

	[InputLabel("Nome completo do jogador")]
	[ModalTextInput("jogador", TextInputStyle.Short, "Seu nome...", maxLength: 100)]
	public string jogador { get; set; }

	[InputLabel("Nome do personagem")]
	[ModalTextInput("name", TextInputStyle.Short, "Nome do personagem...", maxLength: 40)]
	public string name { get; set; }

	[InputLabel("Idade do personagem (número inteiro)")]
	[ModalTextInput("age", TextInputStyle.Short, "Idade do personagem...", maxLength: 3)]
	public string age { get; set; }

	[InputLabel("Peso do personagem (número inteiro em kg)")]
	[ModalTextInput("weight", TextInputStyle.Short, "Peso do personagem...", maxLength: 3)]
	public string weight { get; set; }

	[InputLabel("Altura do personagem (em cm)")]
	[ModalTextInput("height", TextInputStyle.Short, "Altura do personagem...", maxLength: 3)]
	public string height { get; set; }
}