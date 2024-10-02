using System.Security.Claims;
using Image_guesser.Core.Domain.ImageContext.Repository;
using Image_guesser.Core.Domain.SessionContext.Repository;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Domain.UserContext.Services;

namespace Image_guesser.Core.Domain.SessionContext.Services;

public class SessionService(ISessionRepository sessionRepository, IUserService userService, IImageRepository imageRepository) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly IImageRepository _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));

    public async Task CreateSession(ClaimsPrincipal User, Guid Id)
    {
        var user = await _userService.GetUserByClaimsPrincipal(User);

        Session session = new(user, Id);

        await _sessionRepository.AddSession(session);
    }

    public async Task<Session> GetSessionById(Guid Id)
    {
        return await _sessionRepository.GetSessionById(Id);
    }

    public async Task UpdateSession(Session session)
    {
        await _sessionRepository.UpdateSession(session);
    }

    public async Task UpdateSessionOptions(Guid Id, ViewModelOptions options)
    {
        var session = await GetSessionById(Id);

        await session.Options.SetOptionsValues(options, _imageRepository);

        await UpdateSession(session);
    }

    public async Task<bool> CheckIfOracleIsAI(Guid sessionId)
    {
        var session = await GetSessionById(sessionId);

        return session.Options.IsOracleAI();
    }

    public async Task UpdateChosenOracleIfUserWasOracle(Guid sessionId, Guid userId)
    {
        var session = await GetSessionById(sessionId);

        if (session.ChosenOracleId == userId)
        {
            session.ChosenOracleId = session.SessionHostId;
        }
    }

    public async Task<bool> AddUserToSession(string userId, string sessionId)
    {
        User user = await _userService.GetUserById(userId);
        var session = await GetSessionById(Guid.Parse(sessionId));
        if (session.SessionUsers.Contains(user))
        {
            return false;
        }
        else
        {
            session.SessionUsers.Add(user);
            await UpdateSession(session);

            await _userService.UpdateUser(user);

            return true;
        }
    }

    public async Task<bool> RemoveUserFromSession(User user, string sessionId)
    {
        var session = await GetSessionById(Guid.Parse(sessionId));
        var result = session.SessionUsers.Remove(user);
        await UpdateSession(session);

        await _userService.UpdateUser(user);

        return result;
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
