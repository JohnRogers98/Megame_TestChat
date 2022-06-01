using ChatWebService.Helpers;
using ChatWebService.Models;
using ChatWebService.Models.Responses;
using ChatWebService.Services.Interfaces;
using ChatWebService.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ChatWebService.Services
{
    public class MicrosoftIdentityService : IMicrosoftIdentityService
    {
        #region Fields

        private readonly UserManager<AppUser> _userManager;
        private readonly JWT _jwt;

        #endregion

        public MicrosoftIdentityService(UserManager<AppUser> userManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
        }

        #region Public Methods

        public async Task<IdentityResult> CreateUserAsync(String email, String password, String username, String role)
        {
            var user = new AppUser { Email = email, UserName = username };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                user = await _userManager.FindByEmailAsync(email);
                await _userManager.AddToRoleAsync(user, role);
            }

            return result;
        }

        public virtual async Task<LoginResponse> AuthenticateAsync(String email, String password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            var passwordCheckResult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (passwordCheckResult == PasswordVerificationResult.Success)
            {
                var claims = new List<Claim>
                {
                    new Claim("id", user.Id)
                };

                claims.Add(new Claim("email", user.Email));

                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

                foreach (var role in await _userManager.GetRolesAsync(user))
                {
                    claims.Add(new Claim("roles", role));
                }
                var token = TokenHelper.GenerateJwtToken(user, claims, _jwt);

                return new LoginResponse { Token = token, Id = user.Id};

            }

            return null;
        }

        #endregion

    }
}
