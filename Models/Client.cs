using System;
using System.Collections.Generic;

namespace Crewing.Models
{
    public partial class Client
    {
        public Client()
        {
            Contracts = new HashSet<Contract>();
            Documents = new HashSet<Document>();
            LanguageClients = new HashSet<LanguageClient>();
            Reviews = new HashSet<Review>();
        }

        public int Id { get; set; }
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public DateOnly Registrationdate { get; set; }
        public string Phonenumber { get; set; } = null!;
        public string? Education { get; set; }
        public string Status { get; set; } = null!;
        public string? Dependencies { get; set; }
        public string? Chronicdiseases { get; set; }
        public string? Experience { get; set; }
        public DateOnly Birthdate { get; set; }
        public string Email { get; set; } = null!;
        public bool Ismale { get; set; }
        public string? Bio { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<LanguageClient> LanguageClients { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
