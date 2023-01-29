using AppSecAssignment.Model;
using AppSecAssignment.Services;
using AppSecAssignment.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Net;

namespace AppSecAssignment.Pages
{
    public class OTPModel : PageModel
    {
        public int otp;
        private readonly UserServices _userServices;
        public string Email { get; set; }
        public bool EmailSent = false;
        public DateTime otpsaveTime;
        public DateTime inputTime;

        private RoleManager<IdentityRole> roleManager { get; }
        private UserManager<ApplicationUser> userManager { get; }

        private readonly SignInManager<ApplicationUser> signInManager;
        private AuditLogServices _auditLogServices { get; set; }

        private readonly IHttpContextAccessor contxt;

        [BindProperty]
        public ApplicationUser myUser { get; set; } = new();

        [BindProperty]
        public AuditLog myAudit { get; set; }

        public OTPModel(UserServices userServices, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
            AuditLogServices auditLogServices, IHttpContextAccessor contxt, SignInManager<ApplicationUser> signInManager)
        {
            _userServices = userServices;
            this.roleManager = roleManager;
            this.userManager = userManager;
            _auditLogServices = auditLogServices;
            this.contxt = contxt;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> OnGet(string email)
        {
            var user = _userServices.GetUserDetails(email);

            if (user.Count == 1)
            {
                Email = email;
                Random random = new Random();
                otp = random.Next(100000, 999999);
                var userOTP = _userServices.GetUserDetails(email);
                myUser = userOTP;
                userOTP.OTP = otp;
                System.Diagnostics.Debug.WriteLine("otp " + userOTP.Count);
                _userServices.UpdateUser(userOTP);
                otpsaveTime = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("otp save time" + otpsaveTime);


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("limyanshuen04@gmail.com");
                    mail.To.Add(email);
                    mail.Subject = "OTP";
                    mail.Body = "Your OTP is " + otp;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("limyanshuen04@gmail.com", "ovdsvyobkikxoddu");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                        EmailSent = true;
                    }
                }
                return Page();
            }

            else
            {
                user.Count = 0;
                _userServices.UpdateUser(user);
                await signInManager.SignOutAsync();
                contxt.HttpContext.Session.Remove("email");
                TempData["MultipleLogin"] = "There is already a device logged in";
                return RedirectToPage("Login");
            }


        }

        public async Task<IActionResult> OnPost(string email)
        {
            inputTime = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("otp input time" + inputTime);
            //TimeSpan diff = inputTime.Subtract(inputTime);
            TimeSpan diff = inputTime.TimeOfDay - otpsaveTime.TimeOfDay;
            System.Diagnostics.Debug.WriteLine("diff check " + diff.TotalSeconds);

            //creating role
            //IdentityRole role = await roleManager.FindByIdAsync("User");
            //if (role == null)
            //{
            //    IdentityResult assign = await roleManager.CreateAsync(new IdentityRole("AuthenticatedUser"));
            //    if (!assign.Succeeded)
            //    {
            //        ModelState.AddModelError("role", "Create role AuthenticatedUser failed");
            //    }
            //}

            if (_userServices.GetUserDetails(myUser.Email).OTP == myUser.OTP)
            {
                //System.Diagnostics.Debug.WriteLine("otp diff" + diff);
                //if (diff < 60)
                //{
                var user = _userServices.GetUserDetails(myUser.Email);
                await userManager.AddToRoleAsync(user, "AuthenticatedUser");

                myAudit.auditId = Guid.NewGuid().ToString();
                myAudit.userId = user.Id;
                myAudit.activity = "Logged In";
                myAudit.datetime = DateTime.Now;
                _auditLogServices.AddAuditLog(myAudit);

                return Redirect("/Index");
                //}

                //else
                //{
                //    System.Diagnostics.Debug.WriteLine("otp expired");
                //    ModelState.AddModelError("otp expire", "OTP expired. Please try again");
                //    return Page();
                //}

            }
            else
            {
                TempData["OTPWrong"] = "Invalid OTP. Please try again.";
                //ModelState.AddModelError("wrong otp", "Invalid OTP. Please try again.");
                System.Diagnostics.Debug.WriteLine("Invalid OTP");
                return Page();
            }

        }
    }
}
