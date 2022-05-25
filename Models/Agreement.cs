using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Agreement
    {
        public Agreement()
        {
            Vacancies = new HashSet<Vacancy>();
        }

        public int Agreementnumber { get; set; }
        public string Vesselnumber { get; set; } = null!;
        public int Employeeid { get; set; }
        public DateOnly Conclusiondate { get; set; }

        public virtual Employee Employee { get; set; } = null!;
        public virtual Vessel VesselnumberNavigation { get; set; } = null!;
        public virtual ICollection<Vacancy> Vacancies { get; set; }
    }
}
