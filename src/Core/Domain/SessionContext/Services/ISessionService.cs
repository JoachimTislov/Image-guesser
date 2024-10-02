using System.Security.Claims;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.SessionContext.Services;
public interface ISessionService
{
    Task CreateSession(ClaimsPrincipal User, Guid SessionId);

    Task<Session> GetSessionById(Guid Id);

    Task UpdateSession(Session session);

    Task UpdateSessionOptions(Guid Id, ViewModelOptions options);

    Task<bool> CheckIfOracleIsAI(Guid sessionId);

    Task UpdateChosenOracleIfUserWasOracle(Guid sessionId, Guid userId);

    Task<bool> AddUserToSession(string userId, string SessionId);

    Task<bool> RemoveUserFromSession(User user, string SessionId);

    Task DeleteSessionById(Guid Id);

    List<Session> GetAllOpenSessions();

}