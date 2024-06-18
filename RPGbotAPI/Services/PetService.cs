using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RPGbotAPI.Models;
using RPGbotLib;

namespace RPGbotAPI.Services
{
	public class PetService
	{
		private readonly IMongoCollection<Pet> _collection;

		public PetService(IOptions<PetDatabaseSettings> service)
		{
			var mongoClient = new MongoClient(service.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(service.Value.DatabaseName);

			_collection = mongoDatabase.GetCollection<Pet>
				(service.Value.CollectionName);
		}

		public PetService()
		{
			var mongoClient = new MongoClient(IDatabaseConfiguration.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(IDatabaseConfiguration.DatabaseName);



			_collection = mongoDatabase.GetCollection<Pet>
				("Pets");
		}

		public async Task<List<Pet>> GetAsync() =>
			await _collection.Find(x => true).ToListAsync();

		public async Task<Pet> GetAsync(string name) =>
			await _collection.Find(x => x.Id == Utilities.FormatID(name)).FirstOrDefaultAsync();

		public async Task CreateAsync(Pet colObj) =>
			await _collection.InsertOneAsync(colObj);

		public async Task UpdateAsync(string name, Pet colObj) =>
			await _collection.ReplaceOneAsync(x => x.Id == Utilities.FormatID(name), colObj);

		public async Task RemoveAsync(string name) =>
			await _collection.DeleteOneAsync(x => x.Id == Utilities.FormatID(name));
	}
}
