using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RPGbotAPI.Models;

namespace RPGbotAPI.Services
{
	public class PersonagemService
	{
		private readonly IMongoCollection<Personagem> _collection;

		public PersonagemService(IOptions<PersonagemDatabaseSettings> service)
		{
			var mongoClient = new MongoClient(service.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(service.Value.DatabaseName);

			

			_collection = mongoDatabase.GetCollection<Personagem>
				(service.Value.CollectionName);
		}

		public PersonagemService(string _CollectionName)
		{
			var mongoClient = new MongoClient(IDatabaseConfiguration.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(IDatabaseConfiguration.DatabaseName);

			

			_collection = mongoDatabase.GetCollection<Personagem>
				(_CollectionName);
		}

		public async Task<List<Personagem>> GetAsync() =>
			await _collection.Find(x => true).ToListAsync();

		public async Task<Personagem> GetAsync(ulong id) =>
			await _collection.Find(x => x._Id == id).FirstOrDefaultAsync();

		public async Task CreateAsync(Personagem colObj) =>
			await _collection.InsertOneAsync(colObj);

		public async Task UpdateAsync(ulong id, Personagem colObj) =>
			await _collection.ReplaceOneAsync(x => x._Id == id, colObj);

		public async Task RemoveAsync(ulong id) =>
			await _collection.DeleteOneAsync(x => x._Id == id);
	}
}
