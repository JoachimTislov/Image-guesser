using System.Security.Principal;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Tests.Helpers;

namespace Tests.Integration.Authentication;

public class IdentityTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    // private readonly CustomWebApplicationFactory<Program> _fixture;
    // private readonly DefaultHttpContextFactory _DefaultHttpContextFactory;
    private readonly UserManager<User> _UserManager;
    private readonly SignInManager<User> _SignInManager;
    private readonly HttpContext _HttpContext;

    private readonly string Username = "TestUser";
    private readonly string Password = "Test@Password";
    private readonly User User;

    public IdentityTests(CustomWebApplicationFactory<Program> fixture)
    {
        //_fixture = fixture;
        //_DefaultHttpContextFactory = new DefaultHttpContextFactory(fixture.Services);
        _UserManager = fixture.Services.GetRequiredService<UserManager<User>>();
        _SignInManager = fixture.Services.GetRequiredService<SignInManager<User>>();
        _HttpContext = fixture.Services.GetRequiredService<IHttpContextAccessor>().HttpContext!;

        User = new() { UserName = Username };

        var db = fixture.Services.GetRequiredService<ImageGameContext>();
        db.Database.EnsureCreatedAsync().Wait();
    }

    [Fact]
    public async Task CreateLoginAndDeleteUser()
    {
        var createUser = await _UserManager.CreateAsync(User, Password);
        Assert.True(createUser.Succeeded);

        await _SignInManager.SignInAsync(User, isPersistent: false);

        var IsSignedIn = _SignInManager.IsSignedIn(_HttpContext.User);
        Assert.True(IsSignedIn);
        Assert.True(_HttpContext.User.Identity!.IsAuthenticated);

        // A redirect to another page is needed to get a new httpContext
        // await _SignInManager.SignOutAsync();
        // Remove cookies: _HttpContext.Response.Cookies["CookieName"].Expires = DateTime.Now.AddYears(-1);
        // Assert.False(_HttpContext.User.Identity!.IsAuthenticated);

        // Simulating signing out without redirecting -- https://stackoverflow.com/questions/4050925/page-user-identity-isauthenticated-still-true-after-formsauthentication-signout
        _HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

        var loginUser = await _SignInManager.PasswordSignInAsync(Username, Password, isPersistent: false, lockoutOnFailure: false);
        Assert.True(loginUser.Succeeded);
        Assert.True(_HttpContext.User.Identity!.IsAuthenticated);

        var deleteUser = await _UserManager.DeleteAsync(User);
        Assert.True(deleteUser.Succeeded);
    }
}