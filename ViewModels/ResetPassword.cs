using System.ComponentModel.DataAnnotations;

namespace AppSecAssignment.ViewModels
{
	public class ResetPassword
	{
        public string Email { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[/?^*]).{12,}$",
            ErrorMessage = "Password needs at least 1 lowercase character, 1 uppercase character, 1 digit and 1 special character.")]
        public string Password { get; set; } = string.Empty;
    }
}
