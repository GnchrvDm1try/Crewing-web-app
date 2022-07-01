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
        public int Employeeid { get; set; } = 0;
        public DateOnly Conclusiondate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public virtual Employee? Employee { get; set; }
        public virtual Vessel? VesselnumberNavigation { get; set; }
        public virtual ICollection<Vacancy> Vacancies { get; set; }
    }
}
