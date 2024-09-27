using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.SessionContext.Services;
public interface ISessionService
{
    Task<Session> CreateSession(User user);

    Task AddSession(Session session);

    Task UpdateSession(Guid Id, ViewModelOptions options);

    Task<Session> GetSessionById(Guid Id);

    Task<bool> CheckIfOracleIsAI(Guid sessionId);

    Task<bool> JoinSession(User user, Session session);

    Task<bool> LeaveSession(User user, Session session);

}