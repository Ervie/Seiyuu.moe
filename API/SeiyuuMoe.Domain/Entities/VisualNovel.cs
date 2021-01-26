using System;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public class VisualNovel
	{
		[Key]
		public Guid Id { get; set; }
		public int VndbId { get; set; }
		public string Title { get; set; }
		public string TitleOriginal { get; set; }
		public string Alias { get; set; }
		public string ImageUrl { get; set; }
		public string About { get; set; }
		public int Popularity { get; set; }
		public DateTime ModificationDate { get; set; }
	}
}