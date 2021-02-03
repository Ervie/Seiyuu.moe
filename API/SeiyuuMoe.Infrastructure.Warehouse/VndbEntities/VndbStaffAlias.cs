using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeiyuuMoe.Infrastructure.Warehouse.VndbEntities
{
	[Table("staff_alias")]
	public class VndbStaffAlias
	{
		public VndbStaffAlias()
		{
			Roles = new HashSet<VndbVisualNovelSeiyuu>();
		}

		[Column("id"), ForeignKey(nameof(Staff))]
		public int Id { get; set; }

		[Key]
		[Column("aid")]
		public int StaffAliasId { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("original")]
		public string OriginalName { get; set; }

		public virtual VndbStaff Staff { get; set; }

		public virtual ICollection<VndbVisualNovelSeiyuu> Roles { get; set; }
	}
}