﻿using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Data.Model
{
    public partial class Seiyuu
    {
        public Seiyuu()
        {
            Role = new HashSet<Role>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long MalId { get; set; }
        public string ImageUrl { get; set; }
        public long? Popularity { get; set; }
        public string JapaneseName { get; set; }
        public string About { get; set; }
        public string Birthday { get; set; }

        public virtual ICollection<Role> Role { get; set; }
    }
}
