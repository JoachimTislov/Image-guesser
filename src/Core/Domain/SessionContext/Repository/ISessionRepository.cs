
using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.SessionContext.Repository;

public interface ISessionRepository
{
    Task AddSession(Session session);

    Task UpdateSession(Session session);

    Task<List<User>> GetUsersInSessionById(Guid Id);

    Task<Session> GetSessionById(Guid Id);

    List<BaseGame> GetGamesInSessionById(Guid Id);

    Task<Guid> GetSessionHostIdBySessionId(Guid Id);

    List<Session> GetAllOpenSessions();

    Task DeleteSessionById(Guid Id);
}