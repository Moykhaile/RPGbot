using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RPGbotAPI.Models;
using RPGbotLib;

namespace RPGbotAPI.Services
{
	public class RacaService
	{
		private readonly IMongoCollection<Raca> _collection;

		public RacaService(IOptions<RacaDatabaseSettings> service)
		{
			var mongoClient = new MongoClient(service.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(service.Value.DatabaseName);

			_collection = mongoDatabase.GetCollection<Raca>
				(service.Value.CollectionName);
		}
		public RacaService(string _CollectionName)
		{
			var mongoClient = new MongoClient(IDatabaseConfiguration.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(IDatabaseConfiguration.DatabaseName);

			

			_collection = mongoDatabase.GetCollection<Raca>
				(_CollectionName);
		}

		public async Task<List<Raca>> GetAsync() =>
			await _collection.Find(x => true).ToListAsync();

		public async Task<Raca> GetAsync(string name) =>
			await _collection.Find(x => x.Id == Utilities.FormatID(name)).FirstOrDefaultAsync();

		public async Task CreateAsync(Raca colObj) =>
			await _collection.InsertOneAsync(colObj);

		public async Task UpdateAsync(string name, Raca colObj) =>
			await _collection.ReplaceOneAsync(x => x.Id == Utilities.FormatID(name), colObj);

		public async Task RemoveAsync(string name) =>
			await _collection.DeleteOneAsync(x => x.Id == Utilities.FormatID(name));
	}
}
