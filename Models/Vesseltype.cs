using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Vesseltype
    {
        public Vesseltype()
        {
            Vessels = new HashSet<Vessel>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Vessel> Vessels { get; set; }
    }
}
