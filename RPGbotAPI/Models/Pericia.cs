using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using RPGbotLib;

namespace RPGbotAPI.Models
{
	public class Pericia
	{
		[BsonId, BsonRepresentation(BsonType.String)] public string? Id { get; set; }
		public string Nome { get; set; } = string.Empty;
		public Utilities.Atributos Atributo { get; set; }
		public string Descricao { get; set; } = string.Empty;
	}
}
