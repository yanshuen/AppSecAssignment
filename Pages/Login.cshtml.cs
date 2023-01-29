using AppSecAssignment.Model;
using AppSecAssignment.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppSecAssignment.Services;

namespace AppSecAssignment.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly CaptchaService _captchaService;

        private readonly UserServices _userService;

        private readonly IHttpContextAccessor contxt;
        private RoleManager<IdentityRole> roleManager { get; }
        private UserManager<ApplicationUser> userManager { get; }

        [BindProperty]
        public Login LModel { get; set; }


        public LoginModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, CaptchaService captchaService,
            UserServices userService, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.contxt = httpContextAccessor;
            _captchaService = captchaService;
            _userService = userService;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                //IdentityRole role = await roleManager.FindByIdAsync("AuthenticatedUser");
                //if (role == null)
                //{
                //    IdentityResult assign = await roleManager.CreateAsync(new IdentityRole("AuthenticatedUser"));
                //    if (!assign.Succeeded)
                //    {
                //        ModelState.AddModelError("role", "Create role AuthenticatedUser failed");
                //    }
                //}

                var captchaResult = await _captchaService.VerifyToken(LModel.Token);
                if (!captchaResult)
                {
                    TempData["Robot"] = "You are not a human.";
                    return Page();
                }

                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, true, true);
                if (identityResult.Succeeded)
                {
                    System.Diagnostics.Debug.WriteLine("correct");

                    contxt.HttpContext?.Session.SetString("email", LModel.Email);

                    var user = _userService.GetUserDetails(contxt.HttpContext.Session.GetString("email"));
                    user.Count += 1;
                    _userService.UpdateUser(user);
                    System.Diagnostics.Debug.WriteLine("login " + user.Count);

                    return Redirect("/OTP?email=" + LModel.Email);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("credentials wrong");
                    TempData["CredentialsWrong"] = "Incorrect username or password.";
                    //ModelState.AddModelError("incorrect_credentials", "Incorrect username or password.");
                }

                if (identityResult.IsLockedOut)
                {
                    System.Diagnostics.Debug.WriteLine("lockout");
                    TempData["Lockout"] = "You have been locked out of your account. Please wait 10 minutes before trying again.";
                    //ModelState.AddModelError("lockout", "You have been locked out of your account. Please wait 10 minutes before trying again.");
                }
            }

            if (contxt.HttpContext.Session.GetString("email") == null)
            {
                return RedirectToPage("/Login");
            }


            return Page();
        }
    }
}
