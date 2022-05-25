using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class LanguagesClient
    {
        public string Level { get; set; } = null!;
        public int Clientid { get; set; }
        public int Languageid { get; set; }

        public virtual Client Client { get; set; } = null!;
        public virtual Language Language { get; set; } = null!;
    }
}
