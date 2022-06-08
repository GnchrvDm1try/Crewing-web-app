using System.ComponentModel.DataAnnotations;

namespace Crewing.Models
{
    public abstract class EntityRegisterModel
    {
        [Required(ErrorMessage = "You must provide an email")]
        [DataType(DataType.EmailAddress)]
        [MinLength(5, ErrorMessage = "The length of the email has to be 5 or more characters")]
        [MaxLength(50, ErrorMessage = "The length of the email has to be 50 or fewer characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "You must provide a mobile phone number")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "The mobile phone number has to be 13 characters long")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "You must provide a password")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The length of the password has to be 8 or more characters")]
        [MaxLength(50, ErrorMessage = "The length of the password has to be 50 or fewer characters")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "You must confirm a password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmedPassword { get; set; }
    }

    public class ClientRegisterModel : EntityRegisterModel
    {
        [Required(ErrorMessage = "You must provide your first name")]
        [MinLength(2, ErrorMessage = "The first name has to be 2 characters or more")]
        [MaxLength(20, ErrorMessage = "The first name has to be no more than 20 characters long")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "You must provide your last name")]
        [MinLength(2, ErrorMessage = "The last name has to be 2 characters or more")]
        [MaxLength(30, ErrorMessage = "The last name has to be no more than 30 characters long")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "You must provide your sex")]
        public bool IsMale { get; set; }

        [Required(ErrorMessage = "You must provide your date of birth")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }

    public class EmployerRegisterModel : EntityRegisterModel
    {
        [Required(ErrorMessage = "You must provide the name of the company")]
        [MinLength(2, ErrorMessage = "The company name has to be 2 characters or more")]
        [MaxLength(30, ErrorMessage = "The company name has to be no more than 30 characters long")]
        public string? CompanyName { get; set; }
    }

    public class UserLoginModel
    {
        [Required(ErrorMessage = "You must provide an email or mobile phone number")]
        [MinLength(5, ErrorMessage = "The email or phone number has to be 5 or more characters long")]
        [MaxLength(50, ErrorMessage = "The email or phone number has to be 50 or fewer characters long")]
        public string? EmailOrPhone { get; set; }

        [Required(ErrorMessage = "You must provide a password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
