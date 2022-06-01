using ChatWebService.Models.Responses;
using Microsoft.AspNetCore.Identity;

namespace ChatWebService.Services.Interfaces
{
    public interface IMicrosoftIdentityService
    {
        Task<IdentityResult> CreateUserAsync(String email, String password, String username, String role);
        Task<LoginResponse> AuthenticateAsync(String email, String password);
    }
}
