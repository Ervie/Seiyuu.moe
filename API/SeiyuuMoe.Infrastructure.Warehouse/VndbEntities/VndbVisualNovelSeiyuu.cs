using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeiyuuMoe.Infrastructure.Warehouse.VndbEntities
{
	[Table("vn_seiyuu")]
	public class VndbVisualNovelSeiyuu
	{
		[Column("id")]
		public int VisualNovelId { get; set; }

		[Column("aid")]
		public int StaffId { get; set; }

		[Column("cid")]
		public int CharacterId { get; set; }

		[Column("note")]
		public string Note { get; set; }


		public virtual VndbVisualNovel VisualNovel { get; set; }
		public virtual VndbCharacter Character { get; set; }
		public virtual VndbStaff Seiyuu { get; set; }
	}
}