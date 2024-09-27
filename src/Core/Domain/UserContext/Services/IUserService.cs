using System.Security.Claims;

namespace Image_guesser.Core.Domain.UserContext.Services;

public interface IUserService
{
    Task<Guid?> GetSessionIdForGivenUserWithClaimPrincipal(ClaimsPrincipal User);
    Task<User> GetUserByClaimsPrincipal(ClaimsPrincipal User);
    Task<User> GetUserById(Guid Id);
    Task<User> GetUserByName(string name);
}