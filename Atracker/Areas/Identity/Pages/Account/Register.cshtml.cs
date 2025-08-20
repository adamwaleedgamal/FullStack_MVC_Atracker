using Atracker.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Atracker.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, SignInManager<ApplicationUser> signInManager, ILogger<RegisterModel> logger, IEmailSender emailSender) { _userManager = userManager; _userStore = userStore; _emailStore = GetEmailStore(); _signInManager = signInManager; _logger = logger; _emailSender = emailSender; }
        [BindProperty] public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public class InputModel { [Required][Display(Name = "Full Name")] public string FullName { get; set; } [Required][EmailAddress] public string Email { get; set; } [Required][StringLength(100, MinimumLength = 6)][DataType(DataType.Password)] public string Password { get; set; } [DataType(DataType.Password)][Compare("Password")] public string ConfirmPassword { get; set; } }
        public async Task OnGetAsync(string returnUrl = null) { ReturnUrl = returnUrl; ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(); }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.FullName = Input.FullName;
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded) { await _signInManager.SignInAsync(user, isPersistent: false); return RedirectToAction("Index", "Dashboard"); }
                foreach (var error in result.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
            }
            return Page();
        }
        private ApplicationUser CreateUser() { try { return Activator.CreateInstance<ApplicationUser>(); } catch { throw new InvalidOperationException(); } }
        private IUserEmailStore<ApplicationUser> GetEmailStore() { if (!_userManager.SupportsUserEmail) { throw new NotSupportedException(); } return (IUserEmailStore<ApplicationUser>)_userStore; }
    }
}