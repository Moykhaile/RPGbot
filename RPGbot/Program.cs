using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RPGbotAPI.Controllers;
using RPGbotAPI.Services;

namespace RPGbot
{
	class Program
	{
		readonly BotInfoController botInfoController = new(new BotInfoService("BotInfo"));

		public static Task Main() => new Program().MainAsync(new Program().GetHost());

		public IHost GetHost()
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddYamlFile("config.yml")
				.Build();

			return Host.CreateDefaultBuilder()
						.ConfigureServices((_, services) =>
						services
						.AddSingleton(config)
						.AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
						{
							GatewayIntents = GatewayIntents.All,
							AlwaysDownloadUsers = true
						}))
						.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
						.AddSingleton<InteractionHandler>()
						.AddSingleton(x => new CommandService())
						).Build();
		}

		public async Task MainAsync(IHost host)
		{
			await RunAsync(host, host.Services.CreateScope());
		}

		public async Task RunAsync(IHost host, IServiceScope serviceScope)
		{
			IServiceProvider provider = serviceScope.ServiceProvider;

			var _client = provider.GetRequiredService<DiscordSocketClient>();
			var sCommands = provider.GetRequiredService<InteractionService>();
			await provider.GetRequiredService<InteractionHandler>().InitializeAsync();
			var config = provider.GetRequiredService<IConfigurationRoot>();

			_client.Log += async (LogMessage msg) => Console.WriteLine(msg.Message);
			sCommands.Log += async (LogMessage msg) => Console.WriteLine(msg.Message);

			await _client.SetGameAsync(botInfoController.Get().Result.Versao);

			_client.Ready += async () =>
			{
				Console.WriteLine($"Logged in as {_client.CurrentUser.Username}!");
				await sCommands.RegisterCommandsToGuildAsync(703097325937098813);
			};

			await _client.LoginAsync(TokenType.Bot, config["token"]);
			await _client.StartAsync();

			await Task.Delay(-1);
		}
	}

}