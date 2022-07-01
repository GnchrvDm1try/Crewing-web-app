using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Vacancy
    {
        public Vacancy()
        {
            Contracts = new HashSet<Contract>();
            Requirements = new HashSet<Requirement>();
        }

        public int Id { get; set; }
        public int Agreementnumber { get; set; }
        public int Sailorpostid { get; set; }
        public short Workersamount { get; set; }
        public decimal Salary { get; set; }
        public string? Description { get; set; }
        public string Term { get; set; } = null!;

        public virtual Agreement? AgreementnumberNavigation { get; set; }
        public virtual Sailorpost? Sailorpost { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Requirement> Requirements { get; set; }
    }
}
