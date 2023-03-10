using Discord;
using Discord.Interactions;
using RPGbot.Classes;
using RPGbot.db;
using RPGbot.Racas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RPGbot.Modules
{
	public class AddPlayerModule : InteractionModuleBase<SocketInteractionContext>
	{
		public InteractionService Commands { get; set; }

		[SlashCommand("add", "Adiciona um personagem ao jogo. Só é permitido 1 personagem por jogador")]
		public async Task HandleAddCommand()
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			if (personagem != null)
			{
				if (personagem.Nome != null)
				{
					await RespondAsync($"Você já tem um personagem! {personagem.Nome} já existe.", ephemeral: true); return;
				}
				else
				{
					new DBpersonagem().Delete(personagem);
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
			foreach (Classe classe in new DBclasse().GetAll())
			{
				classeMenu.AddOption(classe.Mname, classe.Mname);
			}

			var racaMenu = new SelectMenuBuilder()
			{
				CustomId = "racaMenu",
				Placeholder = "Escolha sua raça"
			};
			foreach (Raca raca in new DBraca().GetAll())
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

			personagem.Id = Context.User.Id;

			new DBpersonagem().Put(personagem);
		}

		[ComponentInteraction("classeMenu")]
		public async Task HandleClasseMenu(string selected)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			personagem.Classe = selected;
			new DBpersonagem().Put(personagem);
			await RespondAsync($"Classe {selected} selecionada!", ephemeral: true);
		}
		[ComponentInteraction("racaMenu")]
		public async Task HandleRacaMenu(string selected)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			personagem.Raca = selected;
			new DBpersonagem().Put(personagem);
			await RespondAsync($"Raça {selected} selecionada!", ephemeral: true);
		}
		[ComponentInteraction("generoMenu")]
		public async Task HandleGeneroMenu(string selected)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			personagem.Genero = selected;
			new DBpersonagem().Put(personagem);
			await RespondAsync($"Gênero {selected} selecionado!", ephemeral: true);
		}
		[ComponentInteraction("sexualidadeMenu")]
		public async Task HandleSexualidadeMenu(string selected)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			personagem.Sexualidade = selected;
			new DBpersonagem().Put(personagem);
			await RespondAsync($"Sexualidade {selected} selecionada!", ephemeral: true);
		}
		[ComponentInteraction("posicaoMenu")]
		public async Task HandlePosicaoMenu(string selected)
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
			personagem.Posicao = selected;
			new DBpersonagem().Put(personagem);
			await RespondAsync($"Posição {selected} selecionada!", ephemeral: true);
		}

		[ComponentInteraction("modalButton")]
		public async Task HandleModalButton()
		{
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());
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
			Personagem personagem = new DBpersonagem().Get(Context.User.Id.ToString());

			personagem.Jogador = modal.Jogador;
			personagem.Nome = modal.Name;
			personagem.Idade = int.Parse(modal.Age);
			personagem.Peso = int.Parse(modal.Weight);
			personagem.Altura = int.Parse(modal.Height);

			personagem = GeneratePlayer(personagem);

			personagem.Magias = new List<string>();
			personagem.Inventario = new List<string>();

			new DBpersonagem().Put(personagem);

			await RespondAsync("Personagem criado! Use ``/ficha`` para ver a ficha do seu personagem ✅\n\n*Os dados do seu personagem são seus e cabe a você se irá ou não compartilha-los com outros. Ninguém poderá ver sua ficha, seus itens, magias, etc. além de você.*");
		}

		Personagem GeneratePlayer(Personagem personagem)
		{
			personagem.Sabedoria = JogarDadosAtributos();
			personagem.Forca = JogarDadosAtributos();
			personagem.Destreza = JogarDadosAtributos();
			personagem.Constituicao = JogarDadosAtributos();
			personagem.Inteligencia = JogarDadosAtributos();
			personagem.Carisma = JogarDadosAtributos();

			Classe playerClass = new DBclasse().Get(personagem.Classe);

			personagem.Vida = JogarDadosVida(playerClass.Dice);
			personagem.VidaMax = personagem.Vida;

			personagem.Saldo = JogarDadosSaldo(playerClass.SaldoDice, playerClass.SaldoDiceNum, playerClass.SaldoDiceMod);

			return personagem;
		}

		readonly Random Rnd = new Random();
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
	public string Jogador { get; set; }

	[InputLabel("Nome do personagem")]
	[ModalTextInput("name", TextInputStyle.Short, "Nome do personagem...", maxLength: 40)]
	public string Name { get; set; }

	[InputLabel("Idade do personagem (número inteiro)")]
	[ModalTextInput("age", TextInputStyle.Short, "Idade do personagem...", maxLength: 3)]
	public string Age { get; set; }

	[InputLabel("Peso do personagem (número inteiro em kg)")]
	[ModalTextInput("weight", TextInputStyle.Short, "Peso do personagem...", maxLength: 3)]
	public string Weight { get; set; }

	[InputLabel("Altura do personagem (em cm)")]
	[ModalTextInput("height", TextInputStyle.Short, "Altura do personagem...", maxLength: 3)]
	public string Height { get; set; }
}