
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
        var (pathName, userId) = notification;

        var sessionId = await _userService.GetSessionIdByUserId(Guid.Parse(userId));

        if (sessionId != null && !IsPathNamePermitted(pathName))
        {
            // If the user is in a session and navigates to a page that is not permitted, remove them from the session.
            await _mediator.Publish(new UserLeftSession(sessionId.Value, userId), cancellationToken);
        }

        // Update the user's current page URL. This is used to display for other users in the session.
        var user = await _userService.GetUserById(notification.UserId);

        user.CurrentPageUrl = pathName;

        await _userService.UpdateUser(user);
    }

    private static bool IsPathNamePermitted(string pathName)
    {
        var pagesPermittedWhileInSession = new List<string> { "/Lobby/Options/Text/Id", "/Lobby/Id", "/Lobby/Id/Game/Id" };

        var parts = pathName.Split('/').ToList();

        foreach (var path in pagesPermittedWhileInSession)
        {
            var permittedPathParts = path.Split('/').ToList();

            if (parts.Count == permittedPathParts.Count)
            {
                var counter = 0;
                for (var i = 0; i < parts.Count; i++)
                {
                    if (parts[i] != permittedPathParts[i] && permittedPathParts[i] != "Text" && permittedPathParts[i] != "Id")
                    {
                        break;
                    }
                    counter++;
                }

                if (counter == parts.Count)
                {
                    return true;
                }
            }
        }

        return false;
    }
}