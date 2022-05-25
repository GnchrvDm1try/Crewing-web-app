using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Employer
    {
        public Employer()
        {
            Reviews = new HashSet<Review>();
            Vessels = new HashSet<Vessel>();
        }

        public string Companyname { get; set; } = null!;
        public double? Rating { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Vessel> Vessels { get; set; }
    }
}
