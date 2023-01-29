using AppSecAssignment.Model;
using AppSecAssignment.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace AppSecAssignment.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor contxt;
        private readonly SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager;
        private readonly UserServices _userServices;
        IDataProtector _dataProtector;

        public string email;
        public string fullname;
        public string creditCard;
        public string gender;
        public string mobile;
        public string delivery;
        public string photo;
        public string aboutme;

        public ApplicationUser user;

        public IndexModel(IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, UserServices userServices, IDataProtectionProvider objProvider)
        {
            this.contxt = httpContextAccessor;
            this.signInManager = signInManager;
            this.userManager = userManager;
            _userServices = userServices;
            _dataProtector = objProvider.CreateProtector("assignment");
        }

        public async Task<IActionResult> OnGet()
        {
            email = contxt.HttpContext.Session.GetString("email");
            user = _userServices.GetUserDetails(email);

            if (user == null || email == null)
            {
                await signInManager.SignOutAsync();
                contxt.HttpContext.Session.Remove("email");
                return RedirectToPage("Login");
            }
            else
            {
                fullname = Encoding.UTF8.GetString(Convert.FromBase64String(user.FullName));
                creditCard = _dataProtector.Unprotect(user.CreditCardNo);
                gender = Encoding.UTF8.GetString(Convert.FromBase64String(user.Gender));
                mobile = Encoding.UTF8.GetString(Convert.FromBase64String(user.PhoneNumber));
                delivery = Encoding.UTF8.GetString(Convert.FromBase64String(user.DeliveryAddress));
                photo = Encoding.UTF8.GetString(Convert.FromBase64String(user.Photo));
                aboutme = Encoding.UTF8.GetString(Convert.FromBase64String(user.AboutMe));
            }



            return Page();
        }
    }
}