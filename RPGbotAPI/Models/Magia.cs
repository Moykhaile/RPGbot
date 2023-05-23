using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RPGbotAPI.Models
{
	public class Magia
	{
		[BsonId, BsonRepresentation(BsonType.String)] public string? Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Casting { get; set; } = string.Empty;
		public string Range { get; set; } = string.Empty;
		public string Components { get; set; } = string.Empty;
		public string Duration { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
	}
}