using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RPGbotAPI.Models;
using RPGbotLib;

namespace RPGbotAPI.Services
{
	public class PericiaService
	{
		private readonly IMongoCollection<Pericia> _collection;

		public PericiaService(IOptions<PericiaDatabaseSettings> service)
		{
			var mongoClient = new MongoClient(service.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(service.Value.DatabaseName);

			_collection = mongoDatabase.GetCollection<Pericia>
				(service.Value.CollectionName);
		}

		public PericiaService()
		{
			var mongoClient = new MongoClient(IDatabaseConfiguration.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(IDatabaseConfiguration.DatabaseName);



			_collection = mongoDatabase.GetCollection<Pericia>
				("Pericias");
		}

		public async Task<List<Pericia>> GetAsync() =>
			await _collection.Find(x => true).ToListAsync();

		public async Task<Pericia> GetAsync(string name) =>
			await _collection.Find(x => x.Id == Utilities.FormatID(name)).FirstOrDefaultAsync();

		public async Task CreateAsync(Pericia colObj) =>
			await _collection.InsertOneAsync(colObj);

		public async Task CreateAsync(Pericia[] colObj) =>
			await _collection.InsertManyAsync(colObj);

		public async Task UpdateAsync(string name, Pericia colObj) =>
			await _collection.ReplaceOneAsync(x => x.Id == Utilities.FormatID(name), colObj);

		public async Task RemoveAsync(string name) =>
			await _collection.DeleteOneAsync(x => x.Id == Utilities.FormatID(name));
	}
}
