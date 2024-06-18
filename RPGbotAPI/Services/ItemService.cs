using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RPGbotAPI.Models;
using RPGbotLib;

namespace RPGbotAPI.Services
{
	public class ItemService
	{
		private readonly IMongoCollection<Item> _collection;

		public ItemService(IOptions<ItemDatabaseSettings> service)
		{
			var mongoClient = new MongoClient(service.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(service.Value.DatabaseName);

			_collection = mongoDatabase.GetCollection<Item>
				(service.Value.CollectionName);
		}

		public ItemService()
		{
			var mongoClient = new MongoClient(IDatabaseConfiguration.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(IDatabaseConfiguration.DatabaseName);

			

			_collection = mongoDatabase.GetCollection<Item>
				("Itens");
		}

		public async Task<List<Item>> GetAsync() =>
			await _collection.Find(x => true).ToListAsync();

		public async Task<Item> GetAsync(string name) =>
			await _collection.Find(x => x.Id == Utilities.FormatID(name)).FirstOrDefaultAsync();

		public async Task CreateAsync(Item colObj) =>
			await _collection.InsertOneAsync(colObj);

		public async Task CreateAsync(Item[] colObj) =>
			await _collection.InsertManyAsync(colObj);

		public async Task UpdateAsync(string name, Item colObj) =>
			await _collection.ReplaceOneAsync(x => x.Id == Utilities.FormatID(name), colObj);

		public async Task RemoveAsync(string name) =>
			await _collection.DeleteOneAsync(x => x.Id == Utilities.FormatID(name));
	}
}
