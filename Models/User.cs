namespace Crewing.Models
{
    public abstract class User
    {
        public string Phonenumber { get; set; } = null!;
        
        public string Email { get; set; } = null!;

        public DateOnly Registrationdate { get; set; }
    }
}
