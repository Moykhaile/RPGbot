using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace RPGbotLib
{
	public class Utilities
	{
		public enum Atributos { Força, Destreza, Constituição, Inteligência, Sabedoria, Carisma }

		public enum PetGenero { Feminino, Masculino }

		public static string FormatID(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return text;

			text = Regex.Replace(text.Normalize(NormalizationForm.FormD), @"[-/^\s]", "");
			var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
			return Regex.Replace(new string(chars).Normalize(NormalizationForm.FormC).ToLower(), @"\s*\([^\)]+\)", "");
		}
	}
}