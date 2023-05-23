using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RPGbotAPI.Models;
using RPGbotLib;

namespace RPGbotAPI.Services
{
	public class MagiaService
	{
		private readonly IMongoCollection<Magia> _collection;

		public MagiaService(IOptions<MagiaDatabaseSettings> service)
		{
			var mongoClient = new MongoClient(service.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(service.Value.DatabaseName);

			_collection = mongoDatabase.GetCollection<Magia>
				(service.Value.CollectionName);
		}

		public MagiaService(string _CollectionName)
		{
			var mongoClient = new MongoClient(IDatabaseConfiguration.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(IDatabaseConfiguration.DatabaseName);

			

			_collection = mongoDatabase.GetCollection<Magia>
				(_CollectionName);
		}

		public async Task<List<Magia>> GetAsync() =>
			await _collection.Find(x => true).ToListAsync();

		public async Task<Magia> GetAsync(string name) =>
			await _collection.Find(x => x.Id == Utilities.FormatID(name)).FirstOrDefaultAsync();

		public async Task CreateAsync(Magia colObj) =>
			await _collection.InsertOneAsync(colObj);

		public async Task CreateAsync(Magia[] colObj) =>
			await _collection.InsertManyAsync(colObj);

		public async Task UpdateAsync(string name, Magia colObj) =>
			await _collection.ReplaceOneAsync(x => x.Id == Utilities.FormatID(name), colObj);

		public async Task RemoveAsync(string name) =>
			await _collection.DeleteOneAsync(x => x.Id == Utilities.FormatID(name));
	}
}
