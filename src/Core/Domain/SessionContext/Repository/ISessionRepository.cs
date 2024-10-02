
using Image_guesser.Core.Domain.SessionContext.ViewModels;

namespace Image_guesser.Core.Domain.SessionContext.Repository;

public interface ISessionRepository
{
    Task AddSession(Session session);

    Task UpdateSession(Session session);

    Task<Session> GetSessionById(Guid Id);

    List<Session> GetAllOpenSessions();

    Task DeleteSessionById(Guid Id);
}