using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Requirement
    {
        public int Id { get; set; }
        public int Vacancyid { get; set; }
        public string Name { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string? Description { get; set; }

        public virtual Vacancy Vacancy { get; set; } = null!;
    }
}
