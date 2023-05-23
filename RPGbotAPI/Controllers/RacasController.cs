using Microsoft.AspNetCore.Mvc;
using RPGbotAPI.Models;
using RPGbotAPI.Services;

namespace RPGbotAPI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class RacasController : ControllerBase
	{
		private readonly RacaService _service;

		public RacasController(RacaService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<List<Raca>> GetAll()
		{
			return await _service.GetAsync();
		}

		[HttpGet]
		public async Task<Raca> Get(string name)
		{
			return await _service.GetAsync(name);
		}

		[HttpPost]
		public async Task<Raca> Post(Raca colObj)
		{
			await _service.CreateAsync(colObj);
			return colObj;
		}

		[HttpPut]
		public async Task<Raca> Put(string name, Raca colObj)
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
