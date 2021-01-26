using System.ComponentModel.DataAnnotations.Schema;

namespace SeiyuuMoe.Infrastructure.Warehouse.VndbEntities
{
	[Table("vn")]
	public class VndbVisualNovel
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("title")]
		public string Title { get; set; }

		[Column("original")]
		public string TitleOriginal { get; set; }

		[Column("alias")]
		public string Alias { get; set; }

		[Column("image")]
		public string Image { get; set; }

		[Column("desc")]
		public string Description { get; set; }

		[Column("c_votecount")]
		public int VoteCount { get; set; }
	}
}