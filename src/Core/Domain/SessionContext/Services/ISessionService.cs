using Image_guesser.Core.Domain.UserContext;

namespace Image_guesser.Core.Domain.SessionContext.Services;
public interface ISessionService
{
    Task<Session> CreateSession(User user);

    bool JoinSession(User user, Session session);

    bool LeaveSession(User user, Session session);

}