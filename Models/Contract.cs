using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Contract
    {
        public int Contractnumber { get; set; }
        public int Clientid { get; set; }
        public int Employeeid { get; set; }
        public DateOnly Conclusiondate { get; set; }
        public int Vacancyid { get; set; }
        public string? Status { get; set; }

        public virtual Client Client { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;
        public virtual Vacancy Vacancy { get; set; } = null!;
    }
}
