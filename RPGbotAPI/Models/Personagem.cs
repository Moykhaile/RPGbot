using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RPGbotAPI.Models
{
	public class Personagem
	{
		[BsonId, BsonRepresentation(BsonType.Decimal128, AllowTruncation = true)] public ulong Id { get; set; }

		[BsonElement] public string Nome { get; set; } = string.Empty;
		[BsonElement] public string Jogador { get; set; } = string.Empty;
		[BsonElement] public string Genero { get; set; } = string.Empty;
		[BsonElement] public string Raca { get; set; } = string.Empty;
		[BsonElement] public string Classe { get; set; } = string.Empty;
		[BsonElement] public string Sexualidade { get; set; } = string.Empty;
		[BsonElement] public string Posicao { get; set; } = string.Empty;

		[BsonElement] public int Idade { get; set; }
		[BsonElement] public int Peso { get; set; }
		[BsonElement] public int Altura { get; set; }

		[BsonElement] public bool GeneratedStats { get; set; } = false;
		[BsonElement] public int Forca { get; set; }
		[BsonElement] public int Destreza { get; set; }
		[BsonElement] public int Inteligencia { get; set; }
		[BsonElement] public int Constituicao { get; set; }
		[BsonElement] public int Sabedoria { get; set; }
		[BsonElement] public int Carisma { get; set; }

		[BsonElement] public int VidaMax { get; set; }
		[BsonElement] public int Vida { get; set; }
		[BsonElement] public float Saldo { get; set; }
		[BsonElement] public int XP { get; set; }
		[BsonElement] public int Nivel { get; set; }

		[BsonElement] public Item? Armadura { get; set; }
		[BsonElement] public Item? Escudo { get; set; }

		[BsonElement] public List<Item>? Inventario { get; set; }
		[BsonElement] public List<string> Magias { get; set; } = new List<string>();
		[BsonElement] public List<string>? Pericias { get; set; } = new List<string>();

		[BsonElement] public int Exaustão { get; set; }
		[BsonElement] public List<Pet>? Pets { get; set; }
	}
}
