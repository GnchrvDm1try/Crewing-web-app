using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class EmployeePost
    {
        public int Employeeid { get; set; }
        public int Postid { get; set; }
        public DateOnly Hiringdate { get; set; }
        public decimal Salary { get; set; }

        public virtual Employee Employee { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}
