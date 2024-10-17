
using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.Core.Exceptions;
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

    public async Task<List<User>> GetUsersInSessionById(Guid Id)
    {
        return await _repository.WhereAndSelect_SingleOrDefault<Session, List<User>>(s => s.Id == Id, s => s.SessionUsers)
         ?? throw new EntityNotFoundException($"List of users was not found in session with Id {Id}");
    }

    public async Task<Session> GetSessionById(Guid Id)
    {
        return await _repository.WhereAndInclude_SingleOrDefault<Session, List<User>>(s => s.Id == Id, s => s.SessionUsers) ?? throw new EntityNotFoundException($"Session with Id {Id} was not found");
    }

    public List<BaseGame> GetGamesInSessionById(Guid Id)
    {
        return _repository.WhereAndInclude<BaseGame, List<Guesser>>(s => s.SessionId == Id, s => s.Guessers);
    }

    public async Task<Guid> GetSessionHostIdBySessionId(Guid Id)
    {
        return await _repository.WhereAndSelect_SingleOrDefault<Session, Guid?>(
                /*Where*/    s => s.Id == Id,
                /*Select*/   s => s.SessionHostId
                ) ?? throw new EntityNotFoundException($"SessionHostId was not found by session Id: {Id}");
    }

    public List<Session> GetAllOpenSessions()
    {
        var sessions = _repository.WhereAndInclude<Session, List<User>>
        (
            s => s.SessionStatus != SessionStatus.Closed
            && s.Options.GameMode != GameMode.SinglePlayer
            && s.SessionStatus != SessionStatus.Idle,
            s => s.SessionUsers
        ).ToList();

        sessions.Reverse(); // show the most recent sessions first
        return sessions;
    }

    public async Task DeleteSessionById(Guid Id)
    {
        var session = await GetSessionById(Id);
        await _repository.Delete(session);
    }

}