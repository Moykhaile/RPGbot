using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RPGbotAPI.Models;
using RPGbotLib;

namespace RPGbotAPI.Services
{
	public class ClasseService
	{
		private readonly IMongoCollection<Classe> _collection;

		public ClasseService(IOptions<ClasseDatabaseSettings> service)
		{
			var mongoClient = new MongoClient(service.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(service.Value.DatabaseName);

			_collection = mongoDatabase.GetCollection<Classe>
				(service.Value.CollectionName);
		}

		public ClasseService()
		{
			var mongoClient = new MongoClient(IDatabaseConfiguration.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(IDatabaseConfiguration.DatabaseName);

			_collection = mongoDatabase.GetCollection<Classe>
				("Classes");
		}

		public async Task<List<Classe>> GetAsync() =>
			await _collection.Find(x => true).ToListAsync();

		public async Task<Classe> GetAsync(string name) =>
			await _collection.Find(x => x.Id == Utilities.FormatID(name)).FirstOrDefaultAsync();

		public async Task CreateAsync(Classe colObj) =>
			await _collection.InsertOneAsync(colObj);

		public async Task UpdateAsync(string name, Classe colObj) =>
			await _collection.ReplaceOneAsync(x => x.Id == Utilities.FormatID(name), colObj);

		public async Task RemoveAsync(string name) =>
			await _collection.DeleteOneAsync(x => x.Id == Utilities.FormatID(name));
	}
}
