using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeiyuuMoe.Infrastructure.Warehouse.VndbEntities
{
	[Table("vn_seiyuu")]
	public class VndbVisualNovelSeiyuu
	{
		[Column("id")]
		[ForeignKey(nameof(VisualNovel))]
		public int VisualNovelId { get; set; }

		[Column("aid")]
		[ForeignKey(nameof(StaffAlias))]
		public int StaffAliasId { get; set; }

		[Column("cid")]
		[ForeignKey(nameof(Character))]
		public int CharacterId { get; set; }

		[Column("note")]
		public string Note { get; set; }

		public virtual VndbVisualNovel VisualNovel { get; set; }
		public virtual VndbCharacter Character { get; set; }
		public virtual VndbStaffAlias StaffAlias { get; set; }
	}
}