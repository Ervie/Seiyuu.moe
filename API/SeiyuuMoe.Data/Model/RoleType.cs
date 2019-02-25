using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Data.Model
{
    public partial class RoleType
    {
        public RoleType()
        {
            Role = new HashSet<Role>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Role> Role { get; set; }
    }
}
