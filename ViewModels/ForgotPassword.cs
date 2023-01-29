using System.ComponentModel.DataAnnotations;

namespace AppSecAssignment.ViewModels
{
	public class ForgotPassword
	{
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        //public bool EmailSent { get; set; } = false;
    }
}
