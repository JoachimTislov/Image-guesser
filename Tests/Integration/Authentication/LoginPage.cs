using System.Security.Claims;
using Image_guesser.Core.Domain.UserContext.Services;
using Image_guesser.Pages.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Integration.Authentication;

public class LoginTests
{
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<ILogger<LoginModel>> _mockLoginLogger = new();
    private readonly LoginModel _loginPageModel;
    public LoginTests()
    {
        _loginPageModel = new(_mockUserService.Object, _mockLoginLogger.Object)
        {
            ViewModel = new()
            {
                Username = "TestUser",
                Password = "Test@Password",
                RememberMe = false
            }
        };
    }

    [Fact]
    public async Task OnPostAsync_ShouldLogModelStateErrors_AndReturnPage_WhenModelStateIsInvalid()
    {
        _loginPageModel.ViewModel.Username = string.Empty;
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
        var error = "Invalid username";
        _mockUserService
            .Setup(sm => sm.Login(
                _loginPageModel.ViewModel.Username,
                _loginPageModel.ViewModel.Password,
                _loginPageModel.ViewModel.RememberMe
            ))
            .Returns(Task.FromResult((false, error)));

        var result = await _loginPageModel.OnPostAsync();

        Assert.Equal(error, _loginPageModel.ViewModel.LoginErrorMessage);
        Assert.IsAssignableFrom<IActionResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ShouldAssignWrongPasswordError_AndReturnPage_WhenPasswordIsWrong()
    {
        var error = "Wrong password";
        _mockUserService
            .Setup(sm => sm.Login(
                _loginPageModel.ViewModel.Username,
                _loginPageModel.ViewModel.Password,
                _loginPageModel.ViewModel.RememberMe
            ))
            .Returns(Task.FromResult((false, error)));

        var result = await _loginPageModel.OnPostAsync();

        Assert.Equal(error, _loginPageModel.ViewModel.LoginErrorMessage);
        Assert.IsAssignableFrom<IActionResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ShouldSucceedAndRedirectToHomeIndex_WhenSignInResultSucceeds_AndLogIt()
    {
        var error = string.Empty;
        _mockUserService
            .Setup(sm => sm.Login(
                _loginPageModel.ViewModel.Username,
                _loginPageModel.ViewModel.Password,
                _loginPageModel.ViewModel.RememberMe
            ))
            .Returns(Task.FromResult((true, error)));

        var result = await _loginPageModel.OnPostAsync();

        var redirectResult = Assert.IsAssignableFrom<RedirectToPageResult>(result);
        Assert.Equal("/Home/Index", redirectResult.PageName);

        _mockLoginLogger.Verify(
        logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == $"{_loginPageModel.ViewModel.Username} logged in" && @type.Name == "FormattedLogValues"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),

            Times.Once
        );
    }

}