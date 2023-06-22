using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Aspnetrole
    {
        public Aspnetrole()
        {
            Aspnetroleclaims = new HashSet<Aspnetroleclaim>();
            Users = new HashSet<Aspnetuser>();
        }

        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public string? ConcurrencyStamp { get; set; }

        public virtual ICollection<Aspnetroleclaim> Aspnetroleclaims { get; set; }

        public virtual ICollection<Aspnetuser> Users { get; set; }
    }
}
