using AppSecAssignment.Model;
using AppSecAssignment.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace AppSecAssignment.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        public bool EmailSent = false;
        private UserManager<ApplicationUser> _userManager { get; }

        [BindProperty]
        public ForgotPassword forgotpw { get; set; } = new();

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotpw.Email);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Encoding.UTF8.GetBytes(token);
                var validToken = WebEncoders.Base64UrlEncode(encodedToken);
                string localhost = "https://localhost:44373";
                string url = $"{localhost}/ResetPassword?email={forgotpw.Email}&token={validToken}";


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("limyanshuen04@gmail.com");
                    mail.To.Add(forgotpw.Email);
                    mail.Subject = "Reset Password";
                    mail.Body = $"Click <a href={url}>here</a> to reset your password";
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("limyanshuen04@gmail.com", "ejhypqrxelovkwzs");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                        EmailSent = true;
                    }
                }
            }
            return Page();
        }
    }
}
