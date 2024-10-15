using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.Pages.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Helpers;

namespace Tests.Integration.Authentication;

public class RegisterTests
{
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<ILogger<RegisterModel>> _mockRegisterLogger = new();
    private readonly RegisterModel _registerPageModel;

    public RegisterTests()
    {
        _registerPageModel = new(_mockUserService.Object, _mockRegisterLogger.Object)
        {
            ViewModel = new()
            {
                Username = "TestUser",
                Password = "Test@Password",
                Repeat_Password = "Test@Password"
            }
        };
    }

    [Fact]
    public async Task OnPostAsync_ShouldAssignPasswordDoesNotMatchErrorAndReturnPage_WhenPasswordsDoesNotMatch()
    {
        _registerPageModel.ViewModel.Repeat_Password = "DifferentTestPassword";

        var result = await _registerPageModel.OnPostAsync();

        Assert.Equal("Passwords do not match", _registerPageModel.ViewModel.RegisterErrorMessage);
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

        _mockUserService
            .Setup(sm => sm.Register(
                _registerPageModel.ViewModel.Username,
                _registerPageModel.ViewModel.Password
            )).Returns(Task.FromResult((false, errors)));

        var result = await _registerPageModel.OnPostAsync();

        Assert.Equal(errorDescription, _registerPageModel.ViewModel.Errors[0].Description);
        Assert.IsAssignableFrom<IActionResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ShouldSignInUserAndRedirectToHomeIndex_WhenUserCreationSucceeds()
    {
        _mockUserService
            .Setup(sm => sm.Register(
                _registerPageModel.ViewModel.Username,
                _registerPageModel.ViewModel.Password
            )).Returns(Task.FromResult((true, Array.Empty<IdentityError>())));

        var result = await _registerPageModel.OnPostAsync();

        var redirectResult = Assert.IsAssignableFrom<RedirectToPageResult>(result);
        Assert.Equal("/Home/Index", redirectResult.PageName);
    }

}