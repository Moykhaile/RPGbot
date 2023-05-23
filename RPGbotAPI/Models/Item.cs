using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RPGbotAPI.Models
{
	public class Item
	{
		[BsonId, BsonRepresentation(BsonType.String)] public string? Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public float Peso { get; set; } = 0.0f;
		public string Dano { get; set; } = string.Empty;
		public int Defesa { get; set; } = 0;
		public string Tipo { get; set; } = string.Empty;
		public string ModNome { get; set; } = string.Empty;
		public string Propriedades { get; set; } = string.Empty;
		public float Preco { get; set; } = 0.0f;
	}
}
