using System.Security.Claims;
using Image_guesser.Core.Exceptions;
using Image_guesser.Infrastructure.GenericRepository;
using Microsoft.AspNetCore.Identity;

namespace Image_guesser.Core.Domain.UserContext.Services;

public class UserService(UserManager<User> userManager, IRepository repository) : IUserService
{
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

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

    public Guid GetUserIdByClaimsPrincipal(ClaimsPrincipal User)
    {
        var Id = _userManager.GetUserId(User) ?? throw new EntityNotFoundException($"UserId with ClaimsPrincipal: {User} was not found");
        return Guid.Parse(Id);
    }

    public async Task<User> GetUserById(string Id)
    {
        return await _userManager.FindByIdAsync(Id) ?? throw new EntityNotFoundException($"User with ID: {Id} was not found");
    }

    public async Task<User> GetUserByName(string name)
    {
        return await _userManager.FindByNameAsync(name) ?? throw new EntityNotFoundException($"User with name: {name} was not found");
    }

    public async Task<string> GetUserNameByUserId(string Id)
    {
        var user = await GetUserById(Id);
        return await _userManager.GetUserNameAsync(user) ?? throw new Exception($"User with Id: {Id} does not have a name...");
    }

    public async Task UpdateUser(User user)
    {
        await _repository.Update(user);
    }
}