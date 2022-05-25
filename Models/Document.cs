using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Document
    {
        public int Id { get; set; }
        public string Documentnumber { get; set; } = null!;
        public string Documentname { get; set; } = null!;
        public DateOnly Expiredate { get; set; }
        public int Clientid { get; set; }

        public virtual Client Client { get; set; } = null!;
    }
}
