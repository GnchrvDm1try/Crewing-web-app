using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Language
    {
        public Language()
        {
            LanguageClients = new HashSet<LanguageClient>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<LanguageClient> LanguageClients { get; set; }
    }
}
