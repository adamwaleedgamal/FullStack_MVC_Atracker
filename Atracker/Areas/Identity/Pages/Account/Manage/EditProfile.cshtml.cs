// In Areas/Identity/Pages/Account/Manage/EditProfile.cshtml.cs

using Atracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Atracker.Areas.Identity.Pages.Account.Manage
{
    public class EditProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EditProfileModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

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

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            Input = new InputModel
            {
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                PermanentAddress = user.PermanentAddress,
                PresentAddress = user.PresentAddress,
                ReferralSource = user.ReferralSource,
                Gender = user.Gender
            };
            return Page();
        }

        // THIS IS THE CORRECTED POST METHOD THAT SAVES THE DATA
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Manually update each property on the user object
            user.FullName = Input.FullName;
            user.PhoneNumber = Input.PhoneNumber;
            user.DateOfBirth = Input.DateOfBirth;
            user.PermanentAddress = Input.PermanentAddress;
            user.PresentAddress = Input.PresentAddress;
            user.ReferralSource = Input.ReferralSource;
            user.Gender = Input.Gender;

            // Save the updated user object to the database
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                // Handle errors if the update fails
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                return Page();
            }

            // Refresh the user's session to reflect the changes
            await _signInManager.RefreshSignInAsync(user);

            // Redirect back to the main profile display page with a success message
            TempData["StatusMessage"] = "Your profile has been updated";
            return RedirectToPage("./Index");
        }
    }
}