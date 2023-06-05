using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RPGbotLib;

namespace RPGbotAPI.Models
{
	public class Pet
	{
		[BsonId, BsonRepresentation(BsonType.String)] public string? Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public float TamanhoMochila { get; set; } = float.MinValue;
		public List<Item>? Inventario { get; set; }
		public List<PetAtaque>? Ataques { get; set; }
		public float Preco { get; set; } = float.MinValue;
		public int Defesa { get; set; } = int.MinValue;
		public string Especie { get; set; } = string.Empty;
		public List<Magia>? Magias { get; set; }
		public int MaxVida { get; set; } = int.MinValue;
		public int Vida { get; set; } = int.MinValue;
		public Utilities.PetGenero Genero { get; set; }
	}


	public class PetAtaque
	{
		public string Nome { get; set; } = string.Empty;
		public string Dano { get; set; } = string.Empty;
	}
}