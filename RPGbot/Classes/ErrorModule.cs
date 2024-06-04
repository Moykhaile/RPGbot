using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGbot.Classes
{
	public class ErrorModule
	{
		public static async void PersonagemNotFound(SocketInteractionContext Context, string Id)
		{
			await Context.Interaction.RespondAsync($"Personagem de ID *{Id}* não encontrado!", ephemeral: true);
		}

		public static async void ClasseNaoMagica(SocketInteractionContext Context)
		{
			await Context.Interaction.RespondAsync($"A classe do seu personagem não é mágica!", ephemeral: true);
		}

		public static async void MagiaNotFound(SocketInteractionContext Context, string Nome)
		{
			await Context.Interaction.RespondAsync($"Magia *{Nome}* não encontrada!", ephemeral: true);
		}

		public static async void ItemNotFound(SocketInteractionContext Context, string Nome)
		{
			await Context.Interaction.RespondAsync($"Item *{Nome}* não encontrado!", ephemeral: true);
		}

		public static async void PericiaNotFound(SocketInteractionContext Context, string Nome)
		{
			await Context.Interaction.RespondAsync($"Perícia *{Nome}* não encontrada!", ephemeral: true);
		}

		public static async void NotEnoughXP(SocketInteractionContext Context)
		{
			await Context.Interaction.RespondAsync($"Seu personagem não tem experiência suficiente!", ephemeral: true);
		}

		public static async void NotEnoughInvSpace(SocketInteractionContext Context)
		{
			await Context.Interaction.RespondAsync($"Seu personagem não tem espaço suficiente no seu inventário!", ephemeral: true);
		}

		public static async void NotEnoughMoney(SocketInteractionContext Context)
		{
			await Context.Interaction.RespondAsync($"Seu personagem não tem saldo suficiente!", ephemeral: true);
		}

		public enum T { Item, Magia, Pericia }
		public static async void HasOrHasnt(SocketInteractionContext Context, bool Has, T Tipo)
		{
			if (Tipo == T.Magia)
			{
				if (Has)
					await Context.Interaction.RespondAsync($"Seu personagem já conhece essa magia!", ephemeral: true);
				else
					await Context.Interaction.RespondAsync($"Seu personagem não conhece essa magia!", ephemeral: true);
				return;
			}
			if (Tipo == T.Item)
			{
				if (Has)
					await Context.Interaction.RespondAsync($"Seu personagem já tem esse item em seu inventário!", ephemeral: true);
				else
					await Context.Interaction.RespondAsync($"Seu personagem não tem esse item em seu inventário!", ephemeral: true);
				return;
			}
			if (Tipo == T.Pericia)
			{
				if (Has)
					await Context.Interaction.RespondAsync($"Seu personagem já tem essa perícia!", ephemeral: true);
				else
					await Context.Interaction.RespondAsync($"Seu personagem não tem essa perícia!", ephemeral: true);
				return;
			}
		}

		public static async void DoesntHaveInventory(SocketInteractionContext Context, T Tipo)
		{
			if (Tipo == T.Item)
				await Context.Interaction.RespondAsync($"Seu personagem não tem inventário!", ephemeral: true);
			if (Tipo == T.Magia)
				await Context.Interaction.RespondAsync($"Seu personagem não tem magias!", ephemeral: true);
			if (Tipo == T.Pericia)
				await Context.Interaction.RespondAsync($"Seu personagem não tem perícias!", ephemeral: true);
		}
	}
}
