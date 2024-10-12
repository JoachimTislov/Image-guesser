using System.Security.Claims;

namespace Image_guesser.Core.Domain.UserContext.Services;

public interface IUserService
{
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