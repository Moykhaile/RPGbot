using Microsoft.AspNetCore.Mvc;
using RPGbotAPI.Models;
using RPGbotAPI.Services;

namespace RPGbotAPI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class MagiasController : ControllerBase
	{
		private readonly MagiaService _service;

		public MagiasController(MagiaService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<List<Magia>> GetAll()
		{
			return await _service.GetAsync();
		}

		[HttpGet]
		public async Task<Magia> Get(string name)
		{
			return await _service.GetAsync(name);
		}

		[HttpPost]
		public async Task<Magia> Post(Magia colObj)
		{
			await _service.CreateAsync(colObj);
			return colObj;
		}

		[HttpPost]
		public async Task<Magia[]> PostMany(Magia[] colObj)
		{
			await _service.CreateAsync(colObj);
			return colObj;
		}

		[HttpPut]
		public async Task<Magia> Put(string name, Magia colObj)
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
