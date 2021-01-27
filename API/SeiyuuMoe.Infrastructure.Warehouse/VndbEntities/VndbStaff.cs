using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeiyuuMoe.Infrastructure.Warehouse.VndbEntities
{
	[Table("staff")]
	public  class VndbStaff
	{
		public VndbStaff()
		{
			Aliases = new HashSet<VndbStaffAlias>();
			VisualNovelSeiyuus = new HashSet<VndbVisualNovelSeiyuu>();
		}

		[Key]
		[Column("id")]
		public int Id { get; set; }

		[Column("aid")]
		public int StaffAliasId { get; set; }

		[Column("language")]
		public string Language { get; set; }

		[Column("image")]
		public string Image { get; set; }

		[Column("desc")]
		public string Description { get; set; }

		public virtual ICollection<VndbStaffAlias> Aliases { get; set; }
		public virtual ICollection<VndbVisualNovelSeiyuu> VisualNovelSeiyuus { get; set; }
	}
}