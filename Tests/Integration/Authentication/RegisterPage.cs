using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Pages.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Helpers;

namespace Tests.Integration.Authentication;

public class RegisterTests
{
    private readonly Mock<FakeUserManager> _mockUserManager = new();
    private readonly Mock<FakeSignInManager> _mockSignInManager = new();
    private readonly Mock<ILogger<RegisterModel>> _mockRegisterLogger = new();
    private readonly RegisterModel _registerPageModel;

    public RegisterTests()
    {
        _registerPageModel = new(_mockUserManager.Object, _mockSignInManager.Object, _mockRegisterLogger.Object)
        {
            Username = "TestUser",
            Password = "Test@Password",
            Repeat_Password = "Test@Password"
        };
    }


    [Fact]
    public async Task OnPostAsync_ShouldAssignPasswordDoesNotMatchErrorAndReturnPage_WhenPasswordsDoesNotMatch()
    {
        _registerPageModel.Repeat_Password = "DifferentTestPassword";

        var result = await _registerPageModel.OnPostAsync();

        Assert.Equal("Passwords do not match", _registerPageModel.RegisterErrorMessage);
        Assert.IsAssignableFrom<IActionResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ShouldAssignUserCreationErrorAndReturnPage_WhenUserCreationDoesNotSucceed()
    {
        var errorDescription = "Creation of user failed";
        var errors = new IdentityError[]
        {
                new() { Description = errorDescription },
        };

        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), _registerPageModel.Password))
            .ReturnsAsync(IdentityResult.Failed(errors));

        var result = await _registerPageModel.OnPostAsync();

        Assert.Equal(errorDescription, _registerPageModel.Errors[0].Description);
        Assert.IsAssignableFrom<IActionResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ShouldSignInUserAndRedirectToHomeIndex_WhenUserCreationSucceeds()
    {

        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), _registerPageModel.Password))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _registerPageModel.OnPostAsync();

        _mockSignInManager.Verify(s => s.SignInAsync(It.Is<User>(u => u.UserName == _registerPageModel.Username), It.IsAny<bool>(), null), Times.Once);

        var redirectResult = Assert.IsAssignableFrom<RedirectToPageResult>(result);
        Assert.Equal("/Home/Index", redirectResult.PageName);
    }

}