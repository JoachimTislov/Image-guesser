using System.Security.Claims;
using Image_guesser.Core.Domain.SessionContext.Repository;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;

namespace Image_guesser.Core.Domain.SessionContext.Services;

public class SessionService(ISessionRepository sessionRepository, IUserService userService) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    public async Task CreateSession(ClaimsPrincipal User, Guid Id)
    {
        var user = await _userService.GetUserByClaimsPrincipal(User);

        Session session = new(user, Id);

        await _sessionRepository.AddSession(session);
    }

    public async Task BackToLobbyEvent(Guid Id)
    {
        var session = await GetSessionById(Id);

        session.Options.ResetAmountOfGamesPlayed();
        session.InLobby();

        await UpdateSession(session);
    }

    public async Task<List<User>> GetUsersInSessionById(Guid Id)
    {
        return await _sessionRepository.GetUsersInSessionById(Id);
    }

    public async Task<Session> GetSessionById(Guid Id)
    {
        return await _sessionRepository.GetSessionById(Id);
    }

    public async Task<Guid> GetSessionHostIdById(Guid Id)
    {
        return await _sessionRepository.GetSessionHostIdBySessionId(Id);
    }

    public async Task UpdateSession(Session session)
    {
        await _sessionRepository.UpdateSession(session);
    }

    public async Task UpdateSessionOptions(Guid Id, ViewModelOptions options)
    {
        var session = await GetSessionById(Id);

        session.Options.SetOptionsValues(options);

        await UpdateSession(session);
    }

    public async Task<bool> CheckIfOracleIsAI(Guid sessionId)
    {
        var session = await GetSessionById(sessionId);

        return session.Options.IsOracleAI();
    }

    public async Task<bool> CheckIfUserIsOracle(Guid sessionId, Guid userId)
    {
        var session = await GetSessionById(sessionId);

        return session.UserIsOracle(userId);
    }

    public async Task<bool> CheckIfUserIsSessionHost(Guid sessionId, Guid userId)
    {
        var session = await GetSessionById(sessionId);

        return session.UserIsSessionHost(userId);
    }

    public async Task UpdateChosenOracleIfUserWasOracle(Guid sessionId, Guid userId)
    {
        var session = await GetSessionById(sessionId);

        if (session.ChosenOracleId == userId)
        {
            session.ChosenOracleId = session.SessionHostId;
        }
    }

    public async Task AddUserToSession(string userId, string sessionId)
    {
        User user = await _userService.GetUserById(userId);
        var session = await GetSessionById(Guid.Parse(sessionId));

        if (!session.AddUser(user))
        {
            // Do something if user is already added ? 
            return;
        }

        await UpdateSessionAndUser(session, user);
    }

    private async Task UpdateSessionAndUser(Session session, User user)
    {
        await UpdateSession(session);
        await _userService.UpdateUser(user);
    }

    public async Task RemoveUserFromSession(string userId, string sessionId)
    {
        User user = await _userService.GetUserById(userId);
        var session = await GetSessionById(Guid.Parse(sessionId));

        if (!session.RemoveUser(user))
        {
            // No user to remove...
            return;
        }

        await UpdateSessionAndUser(session, user);
    }

    public async Task DeleteSessionById(Guid Id)
    {
        await _sessionRepository.DeleteSessionById(Id);
    }

    public List<Session> GetAllOpenSessions()
    {
        return _sessionRepository.GetAllOpenSessions();
    }
}
