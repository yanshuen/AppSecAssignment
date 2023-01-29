using Microsoft.AspNetCore.Identity;

namespace AppSecAssignment.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public string CreditCardNo { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string DeliveryAddress { get; set; } = string.Empty;

        public string? Photo { get; set; } = string.Empty;

        public string AboutMe { get; set; } = string.Empty;

        public int OTP { get; set; }

        public int Count { get; set; } = 0;
    }
}
