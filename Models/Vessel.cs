using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Vessel
    {
        public Vessel()
        {
            Agreements = new HashSet<Agreement>();
        }

        public string Vesselname { get; set; } = null!;
        public string? Location { get; set; }
        public short? Workersamount { get; set; }
        public string Companyname { get; set; } = null!;
        public string Internationalnumber { get; set; } = null!;
        public string? Status { get; set; }
        public int? Vesseltypeid { get; set; }

        public virtual Employer CompanynameNavigation { get; set; } = null!;
        public virtual Vesseltype? Vesseltype { get; set; }
        public virtual ICollection<Agreement> Agreements { get; set; }
    }
}
