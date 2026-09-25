#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FootBallShop.Areas.Identity.Pages.Account
{
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginWith2faModel> _logger;
        private readonly IEmailSender _emailSender;

        public LoginWith2faModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginWith2faModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(6, ErrorMessage = "The code must be 6 characters.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "One-time code")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Don't ask again on this device")]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.LogWarning("Unable to load 2FA user.");
                return RedirectToPage("./Login");
            }

            // Generate a 6-digit OTP and send it by email
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

            await _emailSender.SendEmailAsync(
                user.Email,
                "Your FootBallShop login code",
                $@"<div style='font-family:Arial,sans-serif;max-width:400px;margin:auto;padding:32px;'>
                    <h2 style='color:#E8001D;font-weight:900;text-transform:uppercase;letter-spacing:0.05em;'>
                        FootBallShop
                    </h2>
                    <p style='color:#555;'>Your one-time login code is:</p>
                    <div style='font-size:2.5rem;font-weight:900;letter-spacing:0.4em;color:#0D0D0D;
                                background:#F4F4F4;padding:16px 24px;display:inline-block;margin:16px 0;
                                border-left:4px solid #E8001D;'>
                        {token}
                    </div>
                    <p style='color:#9E9E9E;font-size:0.85rem;margin-top:16px;'>
                        This code expires in 5 minutes.<br/>Do not share it with anyone.
                    </p>
                </div>");

            _logger.LogInformation("2FA email OTP sent to {Email}.", user.Email);

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;
            Input = new InputModel();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return Page();

            returnUrl ??= Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                throw new InvalidOperationException("Unable to load two-factor authentication user.");

            var code = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorSignInAsync(
                TokenOptions.DefaultEmailProvider,
                code,
                rememberMe,
                Input.RememberMachine);

            var userId = await _userManager.GetUserIdAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User '{UserId}' signed in with email 2FA.", userId);
                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User '{UserId}' account locked out.", userId);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid 2FA code for user '{UserId}'.", userId);
                ModelState.AddModelError(string.Empty, "Invalid code. Please try again.");
                return Page();
            }
        }
    }
}