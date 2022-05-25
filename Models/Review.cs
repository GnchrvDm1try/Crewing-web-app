using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Review
    {
        public int Id { get; set; }
        public int Clientid { get; set; }
        public string Companyname { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public double Estimation { get; set; }
        public DateTime Datetime { get; set; }

        public virtual Client Client { get; set; } = null!;
        public virtual Employer CompanynameNavigation { get; set; } = null!;
    }
}
