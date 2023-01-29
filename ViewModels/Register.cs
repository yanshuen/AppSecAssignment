using System.ComponentModel.DataAnnotations;

namespace AppSecAssignment.ViewModels
{
    public class Register
    {
        [Required, DataType(DataType.Text), Display(Name = "Full Name")]
        //[RegularExpression(@"^[a-zA-Z]$", ErrorMessage = "Full Name can only contain letters.")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Full Name can only contain letters.")]
        public string Name { get; set; } = string.Empty;

        [Required, DataType(DataType.CreditCard), Display(Name = "Credit Card")]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "Credit Card can only contain digits.")]
        [MinLength(16, ErrorMessage = "Credit Card must be 16 digits long."), MaxLength(16, ErrorMessage = "Credit Card must be 16 digits long.")]
        public string CreditCardNo { get; set; } = string.Empty;

        [Required, DataType(DataType.Text)]
        public string Gender { get; set; } = string.Empty;

        [Required, DataType(DataType.PhoneNumber), Display(Name = "Mobile Number")]
        [RegularExpression("^[0-9]{8}$", ErrorMessage = "Phone number can only contain digits.")]
        [MinLength(8, ErrorMessage = "Mobile Number must be 8 digits long."), MaxLength(8, ErrorMessage = "Mobile Number must be 8 digits long.")]
        public string MobileNo { get; set; } = string.Empty;

        [Required, DataType(DataType.Text), Display(Name = "Delivery Address")]
        [RegularExpression("^[0-9a-zA-Z#\\- ]*$", ErrorMessage = "Address can only contain digits, letters, the # and the - character.")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required, Display(Name = "Email Address"), DataType(DataType.EmailAddress)]
        [RegularExpression("^[0-9a-zA-Z@.]+$", ErrorMessage = "Email must and can only contain digits, letters, the @ and the . character.")]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[/?^*]).{12,}$",
            ErrorMessage = "Password needs at least 1 lowercase character, 1 uppercase character, 1 digit and 1 special character.")]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Photo { get; set; }

        [Required]
        public IFormFile? Upload { get; set; }

        [Required, DataType(DataType.MultilineText), Display(Name = "About Me")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "About Me can only contain letters.")]
        public string AboutMe { get; set; } = string.Empty;
    }
}
