using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Efficiency
    {
        public int? Month { get; set; }
        public long? ClientCount { get; set; }
        public long? ContractCount { get; set; }
        public long? AgreementCount { get; set; }
    }
}
