﻿using System.ComponentModel.DataAnnotations.Schema;

namespace SeiyuuMoe.Infrastructure.Warehouse.VndbEntities
{
	[Table("chars_vns")]
	public class VndbCharacterVisualNovel
	{
		[Column("id")]
		public int CharacterId { get; set; }

		[Column("vid")]
		public int VisualNovelId { get; set; }

		[Column("rid")]
		public int? ReleaseId { get; set; }

		[Column("spoil")]
		public short IsSpoiler { get; set; }

		[Column("role")]
		public string RoleType { get; set; }
	}
}