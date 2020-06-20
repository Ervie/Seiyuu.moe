using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Domain.Entities
{
    public partial class Role
    {
        public long Id { get; set; }
        public long? AnimeId { get; set; }
        public long? RoleTypeId { get; set; }
        public long? CharacterId { get; set; }
        public long? SeiyuuId { get; set; }
		public long? LanguageId { get; set; }

		public virtual Anime Anime { get; set; }
        public virtual Character Character { get; set; }
        public virtual RoleType RoleType { get; set; }
        public virtual Seiyuu Seiyuu { get; set; }
		public virtual Language Language { get; set; }
	}
}
