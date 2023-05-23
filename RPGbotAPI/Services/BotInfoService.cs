using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using RPGbotAPI.Models;
using RPGbotLib;

namespace RPGbotAPI.Services
{
	public class BotInfoService
	{
		private readonly IMongoCollection<BotInfo> _collection;

		public BotInfoService(IOptions<BotInfoDatabaseSettings> service)
		{
			var mongoClient = new MongoClient(service.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(service.Value.DatabaseName);

			_collection = mongoDatabase.GetCollection<BotInfo>
				(service.Value.CollectionName);
		}

		public BotInfoService(string _CollectionName)
		{
			var mongoClient = new MongoClient(IDatabaseConfiguration.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(IDatabaseConfiguration.DatabaseName);

			

			_collection = mongoDatabase.GetCollection<BotInfo>
				(_CollectionName);
		}

		public async Task<BotInfo> GetAsync() =>
			await _collection.Find(x => x.Id == "646bcfb12fcc8e374abac65d").FirstOrDefaultAsync();

		public async Task UpdateAsync(BotInfo colObj) =>
			await _collection.ReplaceOneAsync(x => x.Id == "646bcfb12fcc8e374abac65d", colObj);
	}
}
