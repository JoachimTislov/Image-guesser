using Image_guesser.Core.Domain.SignalRContext.Hubs;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Infrastructure.GenericRepository;
using Microsoft.AspNetCore.SignalR;

namespace Image_guesser.Core.Domain.SessionContext.Services;

public class SessionService(IHubContext<GameHub, IGameClient> hubContext, IRepository repository) : ISessionService
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IHubContext<GameHub, IGameClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    public async Task<Session> CreateSession(User user)
    {
        Session session = new(user);
        await AddSession(session);

        await _hubContext.Groups.AddToGroupAsync(user.Id.ToString(), session.Id.ToString());

        return session;
    }

    public async Task AddSession(Session session)
    {
        await _repository.Add(session);
    }

    public async Task<Session> GetSessionById(Guid Id)
    {
        return await _repository.GetById<Session>(Id);
    }

    public async Task<bool> CheckIfOracleIsAI(Guid sessionId)
    {
        var session = await GetSessionById(sessionId);

        return session.Options.UseAI;
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
