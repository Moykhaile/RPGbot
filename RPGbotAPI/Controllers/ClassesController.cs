using Microsoft.AspNetCore.Mvc;
using RPGbotAPI.Models;
using RPGbotAPI.Services;

namespace RPGbotAPI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ClassesController : ControllerBase
	{
		private readonly ClasseService _service;

		public ClassesController(ClasseService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<List<Classe>> GetAll()
		{
			return await _service.GetAsync();
		}

		[HttpGet]
		public async Task<Classe> Get(string name)
		{
			return await _service.GetAsync(name);
		}

		[HttpPost]
		public async Task<Classe> Post(Classe colObj)
		{
			await _service.CreateAsync(colObj);
			return colObj;
		}

		[HttpPut]
		public async Task<Classe> Put(string name, Classe colObj)
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
