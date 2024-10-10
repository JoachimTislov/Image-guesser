
using Image_guesser.Core.Domain.UserContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.UserContext.Handlers;

public class UserNavigatedToPageHandler(IUserService userService) : INotificationHandler<UserNavigatedToPage>
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    public async Task Handle(UserNavigatedToPage notification, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserById(notification.UserId);

        user.CurrentPageUrl = notification.PageUrl;

        await _userService.UpdateUser(user);
    }
}