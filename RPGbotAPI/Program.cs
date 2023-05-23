using RPGbotAPI.Models;
using RPGbotAPI.Services;

namespace RPGbotAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			Console.WriteLine("> PERSONAGENSDATABASE.VALUE  =  " + builder.Configuration.GetSection("PersonagensDatabase"));

			builder.Services.Configure<PersonagemDatabaseSettings>(builder.Configuration.GetSection("PersonagensDatabase"))
				.AddSingleton<PersonagemService>();
			builder.Services.Configure<ClasseDatabaseSettings>(builder.Configuration.GetSection("ClassesDatabase"))
				.AddSingleton<ClasseService>();
			builder.Services.Configure<ItemDatabaseSettings>(builder.Configuration.GetSection("ItensDatabase"))
				.AddSingleton<ItemService>();
			builder.Services.Configure<MagiaDatabaseSettings>(builder.Configuration.GetSection("MagiasDatabase"))
				.AddSingleton<MagiaService>();
			builder.Services.Configure<PericiaDatabaseSettings>(builder.Configuration.GetSection("PericiasDatabase"))
				.AddSingleton<PericiaService>();
			builder.Services.Configure<RacaDatabaseSettings>(builder.Configuration.GetSection("RacasDatabase"))
				.AddSingleton<RacaService>();
			builder.Services.Configure<BotInfoDatabaseSettings>(builder.Configuration.GetSection("BotInfoDatabase"))
				.AddSingleton<BotInfoService>();

			// Add services to the container.

			builder.Services.AddControllers();
			//builder.Services.AddDbContext<PersonagensContext>(opt => opt.UseInMemoryDatabase("Personagens"));


			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}