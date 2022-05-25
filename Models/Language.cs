using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Language
    {
        public Language()
        {
            LanguagesClients = new HashSet<LanguagesClient>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<LanguagesClient> LanguagesClients { get; set; }
    }
}
