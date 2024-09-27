using Image_guesser.Core.Domain.SessionContext.ViewModels;
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

        //await _hubContext.Groups.AddToGroupAsync(user.Id.ToString(), session.Id.ToString());

        return session;
    }

    public async Task AddSession(Session session)
    {
        await _repository.Add(session);
    }

    public async Task UpdateSession(Guid Id, ViewModelOptions options)
    {
        var session = await GetSessionById(Id);

        session.Options.SetOptionsValues(options);

        await _repository.Update(session);
    }

    public async Task<Session> GetSessionById(Guid Id)
    {
        return await _repository.GetById<Session, Guid>(Id);
    }

    public async Task<bool> CheckIfOracleIsAI(Guid sessionId)
    {
        var session = await GetSessionById(sessionId);

        return session.Options.IsOracleAI();
    }

    public async Task<bool> JoinSession(User user, Session session)
    {
        if (session.SessionUsers.Contains(user) || user == null)
        {
            return false;
        }
        else
        {
            session.SessionUsers.Add(user);

            // unsure if this is needed
            // Adding a user to a session, requires the sessionId to the user to be updated
            await _repository.Update(session);
            await _repository.Update(user);

            return true;
        }
    }

    public async Task<bool> LeaveSession(User user, Session session)
    {
        var result = session.SessionUsers.Remove(user);

        // unsure if this is needed
        // Removing a user from a session, requires the sessionId to the user to be updated
        await _repository.Update(session);
        await _repository.Update(user);

        return result;
    }
}
