using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RPGbotAPI.Models
{
	public class BotInfo
	{
		[BsonId, BsonRepresentation(BsonType.ObjectId)] public string? Id { get; set; }
		public string? Versao { get; set; }
	}
}