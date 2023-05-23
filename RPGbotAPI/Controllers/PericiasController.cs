using Microsoft.AspNetCore.Mvc;
using RPGbotAPI.Models;
using RPGbotAPI.Services;

namespace RPGbotAPI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class PericiasController : ControllerBase
	{
		private readonly PericiaService _service;

		public PericiasController(PericiaService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<List<Pericia>> GetAll()
		{
			return await _service.GetAsync();
		}

		[HttpGet]
		public async Task<Pericia> Get(string name)
		{
			return await _service.GetAsync(name);
		}

		[HttpPost]
		public async Task<Pericia> Post(Pericia colObj)
		{
			await _service.CreateAsync(colObj);
			return colObj;
		}

		[HttpPost]
		public async Task<Pericia[]> PostMany(Pericia[] colObj)
		{
			await _service.CreateAsync(colObj);
			return colObj;
		}

		[HttpPut]
		public async Task<Pericia> Put(string name, Pericia colObj)
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
