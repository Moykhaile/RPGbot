using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPGbotAPI.Models;
using RPGbotAPI.Services;
using RPGbotAPI.Controllers;

namespace RPGbot.Modules
{
	public class AddPlayerModule : InteractionModuleBase<SocketInteractionContext>
	{
		public InteractionService? Commands { get; set; }

		readonly PersonagensController personagensController = new(new PersonagemService("Personagens"));
		readonly ClassesController classesController = new(new ClasseService("Classes"));
		readonly RacasController racasController = new(new RacaService("Racas"));

		[SlashCommand("add", "Adiciona um personagem ao jogo. Só é permitido 1 personagem por jogador")]
		public async Task HandleAddCommand()
		{

			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem != null)
			{
				if (personagem.Nome != null)
				{
					await RespondAsync($"Você já tem um personagem! {personagem.Nome} já existe.", ephemeral: true); return;
				}
				else
				{
					await personagensController.Delete(personagem._Id);
					personagem = new Personagem() { _Id = 0 };
				}
			}
			else
			{
				personagem = new Personagem() { _Id = 0 };
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

			personagem._Id = Context.User.Id;

			await personagensController.Put(personagem._Id, personagem);
		}

		[ComponentInteraction("classeMenu")]
		public async Task HandleClasseMenu(string selected)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			personagem.Classe = selected;
			await personagensController.Put(personagem._Id, personagem);
			await RespondAsync($"Classe {selected} selecionada!", ephemeral: true);
		}
		[ComponentInteraction("racaMenu")]
		public async Task HandleRacaMenu(string selected)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			personagem.Raca = selected;
			await personagensController.Put(personagem._Id, personagem);
			await RespondAsync($"Raça {selected} selecionada!", ephemeral: true);
		}
		[ComponentInteraction("generoMenu")]
		public async Task HandleGeneroMenu(string selected)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			personagem.Genero = selected;
			await personagensController.Put(personagem._Id, personagem);
			await RespondAsync($"Gênero {selected} selecionado!", ephemeral: true);
		}
		[ComponentInteraction("sexualidadeMenu")]
		public async Task HandleSexualidadeMenu(string selected)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			personagem.Sexualidade = selected;
			await personagensController.Put(personagem._Id, personagem);
			await RespondAsync($"Sexualidade {selected} selecionada!", ephemeral: true);
		}
		[ComponentInteraction("posicaoMenu")]
		public async Task HandlePosicaoMenu(string selected)
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			personagem.Posicao = selected;
			await personagensController.Put(personagem._Id, personagem);
			await RespondAsync($"Posição {selected} selecionada!", ephemeral: true);
		}

		[ComponentInteraction("modalButton")]
		public async Task HandleModalButton()
		{
			Personagem personagem = personagensController.Get(Context.User.Id).Result;
			if (personagem.Nome != null)
			{
				await RespondAsync($"Você já tem um personagem! {personagem.Nome} já existe.", ephemeral: true); return;
			}
			if (personagem.Classe == null || personagem.Raca == null || personagem.Sexualidade == null || personagem.Genero == null || personagem.Posicao == null)
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
			personagem.Inventario = new List<string>();
			personagem.Pericias = new List<string>();
			personagem.Exaustão = 0;

			await personagensController.Put(personagem._Id, personagem);

			await RespondAsync("Personagem criado! Use ``/ficha`` para ver a ficha do seu personagem ✅\n\n*Os dados do seu personagem são seus e cabe a você se irá ou não compartilha-los com outros. Ninguém poderá ver sua ficha, seus itens, magias, etc. além de você.*");

			var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Jogador");

			await (Context.User as IGuildUser)!.AddRoleAsync(role);
			await FollowupAsync($"Você agora tem o cargo {role!.Mention}!", ephemeral: true);
		}

		Personagem GeneratePlayer(Personagem personagem)
		{
			personagem.Forca = JogarDadosAtributos();
			personagem.Destreza = JogarDadosAtributos();
			personagem.Inteligencia = JogarDadosAtributos();
			personagem.Constituicao = JogarDadosAtributos();
			personagem.Sabedoria = JogarDadosAtributos();
			personagem.Carisma = JogarDadosAtributos();

			Classe playerClass = classesController.Get(personagem.Classe).Result;

			personagem.Vida = JogarDadosVida(playerClass.Dice);
			personagem.VidaMax = personagem.Vida;

			personagem.Saldo = JogarDadosSaldo(playerClass.SaldoDice, playerClass.SaldoDiceNum, playerClass.SaldoDiceMod);

			return personagem;
		}

		Personagem GerarDadosRaca(Personagem personagem)
		{
			List<string> habilidades = racasController.Get(personagem.Raca).Result.Habilidades;

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
		int JogarDadosSaldo(int dice, int num, int mod)
		{
			var resultados = new List<int> { };
			for (int i = 0; i < num; i++)
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
		int JogarDadosAtributos()
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
			Console.WriteLine(resultado);
			return resultado;
		}
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