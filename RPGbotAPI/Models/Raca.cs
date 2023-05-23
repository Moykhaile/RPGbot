using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RPGbotAPI.Models
{
	public class Raca
	{
		[BsonId, BsonRepresentation(BsonType.String)] public string? Id { get; set; }

		public string Mname { get; set; } = string.Empty;
		public string Fname { get; set; } = string.Empty;
		public List<string> Habilidades { get; set; } = new List<string>();
	}
}