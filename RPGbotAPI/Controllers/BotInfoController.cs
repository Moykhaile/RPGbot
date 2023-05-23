using Microsoft.AspNetCore.Mvc;
using RPGbotAPI.Models;
using RPGbotAPI.Services;

namespace RPGbotAPI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class BotInfoController : ControllerBase
	{
		private readonly BotInfoService _service;

		public BotInfoController(BotInfoService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<BotInfo> Get()
		{
			return await _service.GetAsync();
		}

		[HttpPut]
		public async Task<BotInfo> Put(BotInfo colObj)
		{
			await _service.UpdateAsync(colObj);
			return colObj;
		}
	}
}
