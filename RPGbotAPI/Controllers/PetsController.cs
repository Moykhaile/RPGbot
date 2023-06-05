using Microsoft.AspNetCore.Mvc;
using RPGbotAPI.Models;
using RPGbotAPI.Services;

namespace RPGbotAPI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class PetsController
	{
		private readonly PetService _service;

		public PetsController(PetService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<List<Pet>> GetAll()
		{
			return await _service.GetAsync();
		}

		[HttpGet]
		public async Task<Pet> Get(string name)
		{
			return await _service.GetAsync(name);
		}

		[HttpPost]
		public async Task<Pet> Post(Pet colObj)
		{
			await _service.CreateAsync(colObj);
			return colObj;
		}

		[HttpPut]
		public async Task<Pet> Put(string name, Pet colObj)
		{
			await _service.UpdateAsync(name, colObj);
			return colObj;
		}

		[HttpDelete]
		public async Task Delete(string name)
		{
			await _service.RemoveAsync(name);
		}
	}
}
