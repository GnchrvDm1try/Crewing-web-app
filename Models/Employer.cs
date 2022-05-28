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
        public string Email { get; set; } = null!;
        public string Phonenumber { get; set; } = null!;

        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Vessel> Vessels { get; set; }
    }
}
