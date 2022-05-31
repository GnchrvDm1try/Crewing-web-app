using System.ComponentModel.DataAnnotations;

namespace Crewing.Models
{
    public abstract class UserRegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [MaxLength(50)]
        [MinLength(5)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(13)]
        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [MaxLength(50)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirmedPassword { get; set; }
    }

    public class ClientRegisterModel : UserRegisterModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string? FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string? LastName { get; set; }

        [Required]
        public bool IsMale { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }
    }

    public class EmployerRegisterModel : UserRegisterModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string? CompanyName { get; set; }
    }

    public class UserLoginModel
    {
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string? EmailOrPhone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
