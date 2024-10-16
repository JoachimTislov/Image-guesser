using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Image_guesser.Core.Domain.UserContext.Services;

public interface IUserService
{
    Task<(bool Succeeded, IdentityError[] Errors)> Register(string username, string password);
    Task<(bool Succeeded, string ErrorMessage)> Login(string username, string password, bool rememberMe);
    Task UpdateCurrentImageIdentifier(User user, string imageIdentifier);
    Task<bool> CheckIfClientHasAnAccount(string userId);
    Task<Guid?> GetSessionIdForGivenUserWithClaimPrincipal(ClaimsPrincipal User);
    Task<Guid?> GetSessionIdByUserId(Guid Id);
    Task<User> GetUserByClaimsPrincipal(ClaimsPrincipal User);
    string? GetUserIdByClaimsPrincipal(ClaimsPrincipal User);
    Task<User> GetUserById(string Id);
    Task<User> GetUserByName(string name);
    Task<string> GetUserNameByUserId(string Id);
    Task UpdateUser(User user);
}