using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class Language
	{
		[Key]
		public long Id { get; set; }
		public string Description { get; set; }
	}
}