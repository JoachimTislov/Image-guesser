using System.Security.Claims;
using Image_guesser.Core.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Image_guesser.Core.Domain.UserContext.Services;

public class UserService(UserManager<User> userManager) : IUserService
{
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<Guid?> GetSessionIdForGivenUserWithClaimPrincipal(ClaimsPrincipal User)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return null;
        }

        return user.SessionId;
    }

    public async Task<User> GetUserByClaimsPrincipal(ClaimsPrincipal User)
    {
        return await _userManager.GetUserAsync(User) ?? throw new EntityNotFoundException($"User with ClaimsPrincipal: {User} was not found");
    }

    public async Task<User> GetUserById(Guid Id)
    {
        string stringId = Id.ToString();
        return await _userManager.FindByIdAsync(stringId) ?? throw new EntityNotFoundException($"User with ID: {stringId} was not found");
    }

    public async Task<User> GetUserByName(string name)
    {
        return await _userManager.FindByNameAsync(name) ?? throw new EntityNotFoundException($"User with name: {name} was not found");
    }
}