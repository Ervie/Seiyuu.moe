using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeiyuuMoe.Infrastructure.Warehouse.VndbEntities
{
	[Table("staff_alias")]
	public class VndbStaffAlias
	{
		[Column("id"), ForeignKey(nameof(Staff))]
		public int Id { get; set; }

		[Key]
		[Column("aid")]
		[ForeignKey("staff_aid_fkey")]
		public int StaffAliasId { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("original")]
		public string OriginalName { get; set; }

		public virtual VndbStaff Staff { get; set; }

		public ICollection<VndbVisualNovelSeiyuu> Roles { get; set; }
	}
}