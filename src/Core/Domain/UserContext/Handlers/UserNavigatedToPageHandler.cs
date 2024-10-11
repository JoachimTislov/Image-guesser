
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.UserContext.Handlers;

public class UserNavigatedToPageHandler(IUserService userService, IMediator mediator) : INotificationHandler<UserNavigatedToPage>
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    public async Task Handle(UserNavigatedToPage notification, CancellationToken cancellationToken)
    {
        var (currentPageUrl, userId) = notification;

        var sessionId = await _userService.GetSessionIdByUserId(Guid.Parse(userId));

        var pagesPermittedWhileInSession = new List<string> { "/Lobby/ConfigureSessionOptions", "/Lobby/Session", "/Game" };
        if (sessionId != null && !pagesPermittedWhileInSession.Contains(currentPageUrl))
        {
            // If the user is in a session and navigates to a page that is not permitted, remove them from the session.
            await _mediator.Publish(new UserLeftSession(sessionId.Value, userId), cancellationToken);
        }

        // Update the user's current page URL. This is used to display for other users in the session.
        var user = await _userService.GetUserById(notification.UserId);

        user.CurrentPageUrl = currentPageUrl;

        await _userService.UpdateUser(user);
    }
}