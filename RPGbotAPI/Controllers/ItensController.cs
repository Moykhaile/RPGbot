using Microsoft.AspNetCore.Mvc;
using RPGbotAPI.Models;
using RPGbotAPI.Services;

namespace RPGbotAPI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ItensController : ControllerBase
	{
		private readonly ItemService _service;

		public ItensController(ItemService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<List<Item>> GetAll()
		{
			return await _service.GetAsync();
		}

		[HttpGet]
		public async Task<Item> Get(string name)
		{
			return await _service.GetAsync(name);
		}

		[HttpPost]
		public async Task<Item> Post(Item colObj)
		{
			await _service.CreateAsync(colObj);
			return colObj;
		}

		[HttpPost]
		public async Task<Item[]> PostMany(Item[] colObj)
		{
			await _service.CreateAsync(colObj);
			return colObj;
		}

		[HttpPut]
		public async Task<Item> Put(string name, Item colObj)
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
