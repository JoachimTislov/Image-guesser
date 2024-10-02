
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Infrastructure.GenericRepository;

namespace Image_guesser.Core.Domain.SessionContext.Repository;

public class SessionRepository(IRepository repository) : ISessionRepository
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task AddSession(Session session)
    {
        await _repository.Add(session);
    }

    public async Task UpdateSession(Session session)
    {
        await _repository.Update(session);
    }

    public async Task<Session> GetSessionById(Guid Id)
    {
        return await _repository.WhereAndInclude_SingleOrDefault<Session, List<User>>(s => s.Id == Id, s => s.SessionUsers);
    }

    public List<Session> GetAllOpenSessions()
    {
        var sessions = _repository.Where<Session>(s => s.SessionStatus != SessionStatus.Closed).ToList();
        return sessions;
    }

    public async Task DeleteSessionById(Guid Id)
    {
        var session = await GetSessionById(Id);
        await _repository.Delete(session);
    }

}