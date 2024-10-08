﻿using Discord;
using Discord.Interactions;
using RPGbotAPI.Controllers;
using RPGbotAPI.Models;
using RPGbotAPI.Services;

namespace RPGbot.Modules
{
	public class AddPlayerModule : InteractionModuleBase<SocketInteractionContext>
	{
		public InteractionService? Commands { get; set; }

		readonly PersonagensController personagensController = new(new PersonagemService());
		readonly ClassesController classesController = new(new ClasseService());
		readonly RacasController racasController = new(new RacaService());

		[SlashCommand("add", "Adiciona um personagem ao jogo. Só é permitido 1 personagem por jogador")]
		public async Task HandleAddCommand()
		{

			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem != null)
			{
				if (personagem.Nome != string.Empty)
				{
					await RespondAsync($"Você já tem um personagem! {personagem.Nome} já existe.", ephemeral: true); return;
				}
				else
				{
					await personagensController.Delete(personagem.Id);
					personagem = new Personagem() { Id = 0 };
				}
			}
			else
			{
				personagem = new Personagem() { Id = 0 };
			}

			var classeMenu = new SelectMenuBuilder()
			{
				CustomId = "classeMenu",
				Placeholder = "Escolha sua classe"
			};
			foreach (Classe classe in classesController.GetAll().Result)
			{
				classeMenu.AddOption(classe.Mname, classe.Mname);
			}

			var racaMenu = new SelectMenuBuilder()
			{
				CustomId = "racaMenu",
				Placeholder = "Escolha sua raça"
			};
			foreach (Raca raca in racasController.GetAll().Result)
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

			personagem.Id = Context.User.Id;
			await personagensController.Post(personagem);
			await Context.Channel.SendMessageAsync($"<@324605986683748352>, {Context.User.Username} está criando um personagem!");

			await RespondAsync($"Vamos criar o seu personagem!\n\nEscolha as informações respectivas do seu novo personagem e aperte o botão.", components: component.Build(), ephemeral: true);

			var modalButton = new ButtonBuilder()
			{
				CustomId = "modalButton",
				Label = "Enviar!",
				Style = ButtonStyle.Success
			};

			var buttonComponent = new ComponentBuilder().WithButton(modalButton);

			await FollowupAsync("Clique aqui quando terminar!", components: buttonComponent.Build(), ephemeral: true);
		}

		[ComponentInteraction("classeMenu")]
		public async Task HandleClasseMenu(string selected)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			personagem.Classe = classesController.Get(selected).Result.Mname;
			await personagensController.Put(personagem.Id, personagem);
			await Context.Interaction.DeferAsync();
		}
		[ComponentInteraction("racaMenu")]
		public async Task HandleRacaMenu(string selected)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			personagem.Raca = racasController.Get(selected).Result.Mname;
			await personagensController.Put(personagem.Id, personagem);
			await Context.Interaction.DeferAsync();
		}
		[ComponentInteraction("generoMenu")]
		public async Task HandleGeneroMenu(string selected)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			personagem.Genero = selected;
			await personagensController.Put(personagem.Id, personagem);
			await Context.Interaction.DeferAsync();
		}
		[ComponentInteraction("sexualidadeMenu")]
		public async Task HandleSexualidadeMenu(string selected)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			personagem.Sexualidade = selected;
			await personagensController.Put(personagem.Id, personagem);
			await Context.Interaction.DeferAsync();
		}
		[ComponentInteraction("posicaoMenu")]
		public async Task HandlePosicaoMenu(string selected)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			personagem.Posicao = selected;
			await personagensController.Put(personagem.Id, personagem);
			await Context.Interaction.DeferAsync();
		}

		[ComponentInteraction("modalButton")]
		public async Task HandleModalButton()
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem.Nome != string.Empty)
			{
				await RespondAsync($"Você já tem um personagem! {personagem.Nome} já existe.", ephemeral: true); return;
			}
			if (personagem.Classe == null || personagem.Raca == null || personagem.Sexualidade == string.Empty || personagem.Genero == string.Empty || personagem.Posicao == string.Empty)
			{
				await RespondAsync($"Responda a todas os menus acima!", ephemeral: true); return;
			}

			await RespondWithModalAsync<AddModal>("addmodal");
		}

		[ModalInteraction("addmodal")]
		public async Task HandleAddModal(AddModal modal)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;

			personagem.Jogador = modal.Jogador;
			personagem.Nome = modal.Name;
			personagem.Idade = int.Parse(modal.Age);
			personagem.Peso = int.Parse(modal.Weight);
			personagem.Altura = int.Parse(modal.Height);

			personagem = GeneratePlayer(personagem);
			personagem = GerarDadosRaca(personagem);

			personagem.Magias = new List<string>();
			personagem.Inventario = new List<Item>();
			personagem.Pericias = new List<string>();
			personagem.Exaustão = 0;

			await personagensController.Put(personagem.Id, personagem);

			await RespondAsync("Personagem criado! Use ``/ficha`` para ver a ficha do seu personagem ✅\n\n*Os dados do seu personagem são seus e cabe a você se irá ou não compartilha-los com outros. Ninguém poderá ver sua ficha, seus itens, magias, etc. além de você.*");

			await FollowupAsync("> **Utilize o comando ``/addstats`` para gerar seus atributos como força, destreza, constituição, etc.**", ephemeral: true);

			var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Jogador");

			await (Context.User as IGuildUser)!.AddRoleAsync(role);
			await FollowupAsync($"Você agora tem o cargo {role!.Mention}!", ephemeral: true);
		}

		Personagem GeneratePlayer(Personagem personagem)
		{
			ClassesController classesController = new(new());

			Classe playerClass = classesController.Get(personagem.Classe)!.Result!;

			personagem.Vida = JogarDadosVida(playerClass.Dice);
			personagem.VidaMax = personagem.Vida;

			personagem.Saldo = JogarDadosSaldo(playerClass.SaldoDice, playerClass.SaldoDiceQntd, playerClass.SaldoDiceMod);

			return personagem;
		}

		static Personagem GerarDadosRaca(Personagem personagem)
		{
			RacasController racasController = new(new());

			List<string> habilidades = racasController.Get(personagem.Raca)!.Result!.Habilidades;

			if (habilidades[0] == "Todos")
			{
				personagem.Forca++;
				personagem.Destreza++;
				personagem.Inteligencia++;
				personagem.Constituicao++;
				personagem.Sabedoria++;
				personagem.Carisma++;
			}
			else
			{
				personagem.Forca += habilidades[0] == "Força" ? 2 : habilidades[1] == "Força" ? 1 : 0;
				personagem.Destreza += habilidades[0] == "Destreza" ? 2 : habilidades[1] == "Destreza" ? 1 : 0;
				personagem.Inteligencia += habilidades[0] == "Inteligência" ? 2 : habilidades[1] == "Inteligência" ? 1 : 0;
				personagem.Constituicao += habilidades[0] == "Constituição" ? 2 : habilidades[1] == "Constituição" ? 1 : 0;
				personagem.Sabedoria += habilidades[0] == "Sabedoria" ? 2 : habilidades[1] == "Sabedoria" ? 1 : 0;
				personagem.Carisma += habilidades[0] == "Carisma" ? 2 : habilidades[1] == "Carisma" ? 1 : 0;
			}

			return personagem;
		}

		readonly Random Rnd = new();
		int JogarDadosSaldo(int dice, int qntd, int mod)
		{
			var resultados = new List<int> { };
			for (int i = 0; i < qntd; i++)
			{
				resultados.Add(Rnd.Next(1, dice + 1));
			}

			int resultado = 0;
			foreach (var item in resultados)
				resultado += item;

			return resultado * mod;
		}
		int JogarDadosVida(int y)
		{
			int resultado = Rnd.Next(1, y + 1);

			return resultado;
		}
		/*int JogarDadosAtributos()
		{
			var resultados = new List<int> { };
			for (int i = 0; i < 4; i++)
			{
				resultados.Add(Rnd.Next(1, 7));
			}
			resultados.Remove(resultados.Min());

			int resultado = 0;
			foreach (var item in resultados)
				resultado += item;

			return resultado;
		}*/
		//4d6dl1
	}

	public class AddModal : IModal
	{
		public string Title => "Crie seu personagem!";

		[InputLabel("Nome completo do jogador")]
		[ModalTextInput("jogador", TextInputStyle.Short, "Seu nome...", maxLength: 100)]
		public string Jogador { get; set; } = string.Empty;

		[InputLabel("Nome do personagem")]
		[ModalTextInput("name", TextInputStyle.Short, "Nome do personagem...", maxLength: 40)]
		public string Name { get; set; } = string.Empty;

		[InputLabel("Idade do personagem (número inteiro)")]
		[ModalTextInput("age", TextInputStyle.Short, "Idade do personagem...", maxLength: 3)]
		public string Age { get; set; } = string.Empty;

		[InputLabel("Peso do personagem (número inteiro em kg)")]
		[ModalTextInput("weight", TextInputStyle.Short, "Peso do personagem...", maxLength: 3)]
		public string Weight { get; set; } = string.Empty;

		[InputLabel("Altura do personagem (em cm)")]
		[ModalTextInput("height", TextInputStyle.Short, "Altura do personagem...", maxLength: 3)]
		public string Height { get; set; } = string.Empty;
	}
}