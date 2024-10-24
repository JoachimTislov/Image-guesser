using System.Security.Claims;
using Image_guesser.Core.Exceptions;
using Image_guesser.Infrastructure.GenericRepository;
using Microsoft.AspNetCore.Identity;

namespace Image_guesser.Core.Domain.UserContext.Services;

public class UserService(UserManager<User> userManager, IRepository repository, SignInManager<User> signInManager) : IUserService
{
    private readonly SignInManager<User> _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    private static User CreateUser(string username)
    {
        return new User(username);
    }

    public async Task<(bool Succeeded, IdentityError[] Errors)> Register(string username, string password)
    {
        var user = CreateUser(username);
        var createUserResult = await _userManager.CreateAsync(user, password);
        if (createUserResult.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);

            return (true, Array.Empty<IdentityError>());
        }
        else
        {
            return (false, createUserResult.Errors.ToArray());
        }
    }

    public async Task<(bool Succeeded, string ErrorMessage)> Login(string username, string password, bool rememberMe)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure: false);
        var user = await _userManager.FindByNameAsync(username);

        return signInResult.Succeeded ? (true, string.Empty) : (false, user != null ? "Wrong password" : "Invalid username");
    }

    public async Task UpdateCurrentImageIdentifier(User user, string imageIdentifier)
    {
        user.CustomSizedImageTilesDirectoryId = imageIdentifier;

        await UpdateUser(user);
    }

    public async Task<bool> CheckIfClientHasAnAccount(string userId)
    {
        return await _userManager.FindByIdAsync(userId) != null;
    }

    public async Task<Guid?> GetSessionIdForGivenUserWithClaimPrincipal(ClaimsPrincipal User)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return null;
        }

        return user.SessionId;
    }

    public async Task<Guid?> GetSessionIdByUserId(Guid Id)
    {
        return await _repository.WhereAndSelect_SingleOrDefault<User, Guid?>(u => u.Id == Id, u => u.SessionId);
    }

    public async Task<User> GetUserByClaimsPrincipal(ClaimsPrincipal User)
    {
        return await _userManager.GetUserAsync(User) ?? throw new EntityNotFoundException($"User with ClaimsPrincipal: {User} was not found");
    }

    public string? GetUserIdByClaimsPrincipal(ClaimsPrincipal User)
    {
        return _userManager.GetUserId(User);
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
        await _userManager.UpdateAsync(user);
    }
}