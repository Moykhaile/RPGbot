using Microsoft.AspNetCore.Mvc;
using RPGbotAPI.Models;
using RPGbotAPI.Services;

namespace RPGbotAPI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class PersonagensController : ControllerBase
	{
		private readonly PersonagemService _personagemService;

		public PersonagensController(PersonagemService personagemService)
		{
			_personagemService = personagemService;
		}

		[HttpGet] 
		public async Task<List<Personagem>> GetAll()
		{
			return await _personagemService.GetAsync();
		}

		[HttpGet]
		public async Task<Personagem> Get(ulong id)
		{
			return await _personagemService.GetAsync(id);
		}

		[HttpPost]
		public async Task<Personagem> Post(Personagem personagem)
		{
			await _personagemService.CreateAsync(personagem);
			return personagem;
		}

		[HttpPut]
		public async Task<Personagem> Put(ulong id, Personagem personagem)
		{
			await _personagemService.UpdateAsync(id, personagem);
			return personagem;
		}

		[HttpDelete]
		public async Task Delete(ulong id)
		{
			await _personagemService.RemoveAsync(id);
		}
	}
}
