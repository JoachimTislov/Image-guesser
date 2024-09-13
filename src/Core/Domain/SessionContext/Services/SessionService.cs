using Image_guesser.Core.Domain.SessionContext.Pipelines;
using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Image_guesser.Core.Domain.SessionContext.Services;

public class SessionService(IMediator mediator, IHubContext<GameHub, IGameClient> hubContext) : ISessionService
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IHubContext<GameHub, IGameClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    public async Task<Session> CreateSession(User user)
    {
        var session = await _mediator.Send(new AddSession.Request(new Session(user)));

        await _hubContext.Groups.AddToGroupAsync(user.Id.ToString(), session.Id.ToString());

        return session;
    }

    public bool JoinSession(User user, Session session)
    {
        if (session.SessionUsers.Contains(user) || user == null)
        {
            return false;
        }
        else
        {
            session.SessionUsers.Add(user);
            return true;
        }
    }

    public bool LeaveSession(User user, Session session)
    {
        return session.SessionUsers.Remove(user);
    }
}