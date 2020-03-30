using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PersonalStockTrader.Data.Models;

namespace PersonalStockTrader.Web.Areas.Identity.Pages.Account.Manage
{
    using Data.Common.Repositories;
    using Microsoft.EntityFrameworkCore;

    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<DeletePersonalDataModel> logger;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;

        public DeletePersonalDataModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            IDeletableEntityRepository<ApplicationUser> userRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.userRepository = userRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await this.userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{this.userManager.GetUserId(User)}'.");
            }

            RequirePassword = await this.userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this.userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{this.userManager.GetUserId(User)}'.");
            }

            RequirePassword = await this.userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await this.userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            var userId = await this.userManager.GetUserIdAsync(user);
            var userToBeRemoved = await this.userRepository
                .All()
                .FirstOrDefaultAsync(u => u.Id == userId);

            userToBeRemoved.Account = null;
            userToBeRemoved.UserName = null;
            userToBeRemoved.NormalizedUserName = null;
            userToBeRemoved.Email = null;
            userToBeRemoved.NormalizedEmail = null;
            userToBeRemoved.PasswordHash = null;

            await this.userRepository.SaveChangesAsync();

            await this.signInManager.SignOutAsync();

            this.logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }
    }
}
