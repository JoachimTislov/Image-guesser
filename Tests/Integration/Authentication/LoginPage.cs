using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Pages.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Helpers;

namespace Tests.Integration.Authentication;

public class LoginTests
{
    private readonly Mock<FakeUserManager> _mockUserManager = new();
    private readonly Mock<FakeSignInManager> _mockSignInManager = new();
    private readonly Mock<ILogger<LoginModel>> _mockLoginLogger = new();
    private readonly LoginModel _loginPageModel;
    public LoginTests()
    {
        _loginPageModel = new(_mockUserManager.Object, _mockSignInManager.Object, _mockLoginLogger.Object)
        {
            Username = "TestUser",
            Password = "Test@Password",
            RememberMe = false
        };
    }

    [Fact]
    public async Task OnPostAsync_ShouldLogModelStateErrors_AndReturnPage_WhenModelStateIsInvalid()
    {
        _loginPageModel.Username = string.Empty;
        var error = "Username is required";
        _loginPageModel.ModelState.AddModelError("Username", error);

        var result = await _loginPageModel.OnPostAsync();

        Assert.IsAssignableFrom<IActionResult>(result);

        _mockLoginLogger.Verify(
          logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == $"ModelState error: {error}" && @type.Name == "FormattedLogValues"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),

            Times.Once
        );
    }

    [Fact]
    public async Task OnPostAsync_ShouldAssignInvalidUsernameError_AndReturnPage_WhenUsernameIsInvalid()
    {
        _mockSignInManager
            .Setup(sm => sm.PasswordSignInAsync(
                _loginPageModel.Username,
                _loginPageModel.Password,
                _loginPageModel.RememberMe,
                false
            ))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var result = await _loginPageModel.OnPostAsync();

        Assert.Equal("Invalid username", _loginPageModel.LoginErrorMessage);
        Assert.IsAssignableFrom<IActionResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ShouldAssignWrongPasswordError_AndReturnPage_WhenPasswordIsWrong()
    {
        _mockSignInManager
            .Setup(sm => sm.PasswordSignInAsync(
                _loginPageModel.Username,
                _loginPageModel.Password,
                _loginPageModel.RememberMe,
                false
            ))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        _mockUserManager.Setup(sm => sm.FindByNameAsync(_loginPageModel.Username))
        .ReturnsAsync(new User()
        {
            UserName = _loginPageModel.Username
        });

        var result = await _loginPageModel.OnPostAsync();

        Assert.Equal("Wrong password", _loginPageModel.LoginErrorMessage);
        Assert.IsAssignableFrom<IActionResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ShouldSucceedAndRedirectToHomeIndex_WhenSignInResultSucceeds_AndLogIt()
    {
        _mockSignInManager
            .Setup(sm => sm.PasswordSignInAsync(
                _loginPageModel.Username,
                _loginPageModel.Password,
                _loginPageModel.RememberMe,
                false
            ))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var result = await _loginPageModel.OnPostAsync();

        var redirectResult = Assert.IsAssignableFrom<RedirectToPageResult>(result);
        Assert.Equal("/Home/Index", redirectResult.PageName);

        _mockLoginLogger.Verify(
        logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == $"User with username: {_loginPageModel.Username} successfully signed in" && @type.Name == "FormattedLogValues"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),

            Times.Once
        );
    }

}