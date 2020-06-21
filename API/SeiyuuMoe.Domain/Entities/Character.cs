using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
    public partial class Character
    {
        public Character()
        {
            Role = new HashSet<Role>();
        }

        public string Name { get; set; }
		[Key]
		public long MalId { get; set; }
        public string ImageUrl { get; set; }
        public long? Popularity { get; set; }
        public string NameKanji { get; set; }
        public string About { get; set; }
        public string Nicknames { get; set; }

        public virtual ICollection<Role> Role { get; set; }
	}
}
