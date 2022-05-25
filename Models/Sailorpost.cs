using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Sailorpost
    {
        public Sailorpost()
        {
            Vacancies = new HashSet<Vacancy>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Vacancy> Vacancies { get; set; }
    }
}
