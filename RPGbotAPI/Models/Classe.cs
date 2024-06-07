using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RPGbotAPI.Models
{
	public class Classe
	{
		[BsonId, BsonRepresentation(BsonType.String)] public string? Id { get; set; }
		public string Fname { get; set; } = string.Empty;
		public string Mname { get; set; } = string.Empty;
		public int Dice { get; set; }
		public int SaldoDice { get; set; }
		public int SaldoDiceQntd { get; set; }
		public int SaldoDiceMod { get; set; }
		public bool Magico { get; set; }
		public List<int>? Magias { get; set; }
		public string Desc { get; set; } = string.Empty;
		public string HabilidadeP { get; set; } = string.Empty;
	}
}