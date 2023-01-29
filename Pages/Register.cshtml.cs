using AppSecAssignment.Model;
using AppSecAssignment.Services;
using AppSecAssignment.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace AppSecAssignment.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private RoleManager<IdentityRole> roleManager { get; }

        private AuditLogServices _auditLogServices { get; set; }

        private IWebHostEnvironment _environment;

        IDataProtector _dataProtector;

        private readonly IHttpContextAccessor contxt;

        [BindProperty]
        public Register RModel { get; set; }

        [BindProperty]
        public AuditLog myAudit { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, IDataProtectionProvider objProvider, AuditLogServices auditLogServices)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            _environment = environment;
            contxt = httpContextAccessor;
            _dataProtector = objProvider.CreateProtector("assignment");
            _auditLogServices = auditLogServices;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (RModel.Upload != null)
                {
                    if (RModel.Upload.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Upload", "File size cannot exceed 2MB.");
                        return Page();
                    }
                    var imageFile = Guid.NewGuid() + Path.GetExtension(RModel.Upload.FileName);
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot\\uploads", imageFile);
                    using var fileStream = new FileStream(file, FileMode.Create);
                    await RModel.Upload.CopyToAsync(fileStream);
                    RModel.Photo = "/uploads/" + imageFile;
                }

                string encyptedStringCreditCard = _dataProtector.Protect(RModel.CreditCardNo);

                var user = new ApplicationUser()
                {
                    UserName = RModel.Email,
                    FullName = Convert.ToBase64String(Encoding.UTF8.GetBytes(RModel.Name)),
                    CreditCardNo = encyptedStringCreditCard,
                    Gender = Convert.ToBase64String(Encoding.UTF8.GetBytes(RModel.Gender)),
                    PhoneNumber = Convert.ToBase64String(Encoding.UTF8.GetBytes(RModel.MobileNo)),
                    DeliveryAddress = Convert.ToBase64String(Encoding.UTF8.GetBytes(RModel.DeliveryAddress)),
                    Email = RModel.Email,
                    Photo = Convert.ToBase64String(Encoding.UTF8.GetBytes(RModel.Photo)),
                    AboutMe = Convert.ToBase64String(Encoding.UTF8.GetBytes(RModel.AboutMe)),
                    Count = 1
                };
                System.Diagnostics.Debug.WriteLine("register " + user.Count);

                //create role
                IdentityRole role = await roleManager.FindByIdAsync("AuthenticatedUser");
                if (role == null)
                {
                    IdentityResult assign = await roleManager.CreateAsync(new IdentityRole("AuthenticatedUser"));
                    if (!assign.Succeeded)
                    {
                        ModelState.AddModelError("role", "Create role AuthenticatedUser failed");
                    }
                }

                var result = await userManager.CreateAsync(user, RModel.Password);

                foreach (var error in result.Errors)
                {
                    if (error.Description.StartsWith("Username"))
                    {
                        TempData["DuplicateEmail"] = "This email has already been taken";

                        //ModelState.AddModelError("duplicate_email", "Email is already taken");
                    }
                    else
                    {
                        ModelState.AddModelError("error", error.Description);
                    }
                }

                if (result.Succeeded)
                {
                    //adding roles
                    await userManager.AddToRoleAsync(user, "AuthenticatedUser");

                    await signInManager.SignInAsync(user, false);

                    contxt.HttpContext?.Session.SetString("email", RModel.Email);

                    myAudit.auditId = Guid.NewGuid().ToString();
                    myAudit.userId = user.Id;
                    myAudit.activity = "Registered";
                    myAudit.datetime = DateTime.Now;
                    _auditLogServices.AddAuditLog(myAudit);

                    return RedirectToPage("Index");
                }
            }
            return Page();
        }
    }
}
