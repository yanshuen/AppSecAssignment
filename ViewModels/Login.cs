using System.ComponentModel.DataAnnotations;

namespace AppSecAssignment.ViewModels
{
    public class Login
    {
        [Required, Display(Name = "Email Address"), DataType(DataType.EmailAddress)]
        //[RegularExpression(@"[0-9a-zA-Z@.]$", ErrorMessage = "Email must and can only contain digits, letters, the @ and the . character.")]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[/?^*]).{12,}$",  
        //ErrorMessage = "Password can only contain digits, letters and certain special characters.")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        public string Token { get; set; } = string.Empty;
    }
}
