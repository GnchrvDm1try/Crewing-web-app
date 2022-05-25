using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Post
    {
        public Post()
        {
            EmployeePosts = new HashSet<EmployeePost>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<EmployeePost> EmployeePosts { get; set; }
    }
}
