using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class Language
	{
		[Key]
		public LanguageId Id { get; set; }
		public string Description { get; set; }
	}

	public enum LanguageId: int
	{
		Japanese = 1,
		Korean = 2
	}
}