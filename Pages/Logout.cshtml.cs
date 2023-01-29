using AppSecAssignment.Model;
using AppSecAssignment.Services;
using AppSecAssignment.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppSecAssignment.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly UserServices _userServices;
        private UserManager<ApplicationUser> userManager { get; }

        private AuditLogServices _auditLogServices { get; set; }

        private readonly IHttpContextAccessor contxt;

        [BindProperty]
        public AuditLog myAudit { get; set; }
        public LogoutModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager, UserServices userServices, AuditLogServices auditLogServices)
        {
            this.signInManager = signInManager;
            this.contxt = httpContextAccessor;
            this.userManager = userManager;
            _userServices = userServices;
            _auditLogServices = auditLogServices;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            var user = _userServices.GetUserDetails(contxt.HttpContext.Session.GetString("email"));
            if (user != null)
            {
                user.Count = 0;
                _userServices.UpdateUser(user);
                System.Diagnostics.Debug.WriteLine("logout" + user.Count);

                await userManager.RemoveFromRoleAsync(user, "AuthenticatedUser");

                myAudit.auditId = Guid.NewGuid().ToString();
                myAudit.userId = user.Id;
                myAudit.activity = "Logged Out";
                myAudit.datetime = DateTime.Now;
                _auditLogServices.AddAuditLog(myAudit);

                await signInManager.SignOutAsync();
                contxt.HttpContext.Session.Remove("email");
            }

            else
            {
                await signInManager.SignOutAsync();
                contxt.HttpContext.Session.Remove("email");
            }

            return RedirectToPage("Login");

        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("Index");
        }
    }
}
