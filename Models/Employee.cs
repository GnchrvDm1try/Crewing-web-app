using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Agreements = new HashSet<Agreement>();
            Contracts = new HashSet<Contract>();
            EmployeePosts = new HashSet<EmployeePost>();
        }

        public int Id { get; set; }
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Phonenumber { get; set; } = null!;
        public DateOnly Birthdate { get; set; }
        public string Email { get; set; } = null!;

        public virtual ICollection<Agreement> Agreements { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<EmployeePost> EmployeePosts { get; set; }
    }
}
