using System.Security.Claims;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.SessionContext.Services;

public interface ISessionService
{
    Task CreateSession(ClaimsPrincipal User, Guid SessionId);
    Task BackToLobbyEvent(Guid Id);
    Task<List<User>> GetUsersInSessionById(Guid Id);
    Task<Session> GetSessionById(Guid Id);
    Task<Guid> GetSessionHostIdById(Guid Id);
    Task<bool> CheckIfSessionHasReachedSetNumberOfGamesToPlay(Guid Id);
    Task UpdateSession(Session session);
    Task UpdateSessionOptions(Guid Id, ViewModelOptions options);
    Task<bool> CheckIfOracleIsAI(Guid sessionId);
    Task<bool> CheckIfUserIsOracle(Guid sessionId, Guid userId);
    Task<bool> CheckIfUserIsSessionHost(Guid sessionId, Guid userId);
    Task UpdateChosenOracleIfUserWasOracle(Guid sessionId, Guid userId);
    Task AddUserToSession(string userId, string SessionId);
    Task RemoveUserFromSession(string userId, string SessionId);
    Task DeleteSessionById(Guid Id);
    List<Session> GetAllOpenSessions();

}