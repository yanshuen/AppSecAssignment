using AppSecAssignment.Model;
using AppSecAssignment.Services;
using AppSecAssignment.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AppSecAssignment.Pages
{
    public class ResetPasswordModel : PageModel
    {
        public string ResetToken;
        public string Email;
        public bool reset = false;

        private readonly UserServices _userServices;
        private readonly AuditLogServices _auditlogServices;
        private UserManager<ApplicationUser> _userManager { get; }

        //[BindProperty]
        //public ApplicationUser myUser { get; set; } = new();

        [BindProperty]
        public ResetPassword resetPassword { get; set; } = new();

        [BindProperty]
        public AuditLog myAudit { get; set; }

        public ResetPasswordModel(UserServices userServices, UserManager<ApplicationUser> userManager, AuditLogServices auditlogServices)
        {
            _userServices = userServices;
            _userManager = userManager;
            _auditlogServices = auditlogServices;
        }

        public void OnGet(string email, string token)
        {
            Email = email;
            ResetToken = token;
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPassword.Email);
                var decodedToken = WebEncoders.Base64UrlDecode(resetPassword.Token);
                var normalToken = Encoding.UTF8.GetString(decodedToken);
                var result = await _userManager.ResetPasswordAsync(user, normalToken, resetPassword.Password);
                System.Diagnostics.Debug.WriteLine(result);
                if (result.Succeeded)
                {
                    reset = true;

                    myAudit.auditId = Guid.NewGuid().ToString();
                    myAudit.userId = user.Id;
                    myAudit.activity = "Resetted Password";
                    myAudit.datetime = DateTime.Now;
                    _auditlogServices.AddAuditLog(myAudit);

                    return RedirectToPage("Login");
                }
                foreach (var error in result.Errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.Code, error.Description);
                }
                
            }
            return Page();
        }
    }
}
