// In Areas/Identity/Pages/Account/Manage/Index.cshtml.cs

using Atracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Atracker.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone][Display(Name = "Phone number")] public string PhoneNumber { get; set; }
            [Display(Name = "Full Name")] public string FullName { get; set; }
            [DataType(DataType.Date)][Display(Name = "Date of Birth")] public DateTime? DateOfBirth { get; set; }
            [Display(Name = "Permanent Address")] public string PermanentAddress { get; set; }
            [Display(Name = "Present Address")] public string PresentAddress { get; set; }
            [Display(Name = "Referral Source")] public string ReferralSource { get; set; }
            public string Gender { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Username = userName;
            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                PermanentAddress = user.PermanentAddress,
                PresentAddress = user.PresentAddress,
                ReferralSource = user.ReferralSource,
                Gender = user.Gender
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.FullName = Input.FullName;
            user.DateOfBirth = Input.DateOfBirth;
            user.PermanentAddress = Input.PermanentAddress;
            user.PresentAddress = Input.PresentAddress;
            user.ReferralSource = Input.ReferralSource;
            user.Gender = Input.Gender;
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}