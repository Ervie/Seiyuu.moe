using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeiyuuMoe.Infrastructure.Warehouse.VndbEntities
{
	[Table("staff")]
	public class VndbStaff
	{
		public VndbStaff()
		{
			Aliases = new HashSet<VndbStaffAlias>();
		}

		[Key]
		[Column("id")]
		public int Id { get; set; }

		[Column("aid")]
		[ForeignKey("aid")]
		public int MainAliasId { get; set; }

		[Column("lang")]
		public string Language { get; set; }

		[Column("desc")]
		public string Description { get; set; }

		public virtual VndbStaffAlias MainAlias { get; set; }

		public virtual ICollection<VndbStaffAlias> Aliases { get; set; }
	}
}