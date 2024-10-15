using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.UserContext;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.SessionContext;

public class Session : BaseEntity
{
    public Session() { }

    public Session(User user, Guid id)
    {
        Id = id;
        SessionHostId = user.Id;
        ChosenOracleId = user.Id;
        SessionUsers = [user];
    }

    public Guid Id { get; private set; }
    public Guid SessionHostId { get; set; }
    public Guid ChosenOracleId { get; set; }
    public List<User> SessionUsers { get; set; } = [];
    public Guid? CurrentGameId { get; set; }
    public List<BaseGame> Games { get; set; } = [];
    public Options Options { get; set; } = new();
    public DateTime TimeOfCreation { get; set; } = DateTime.Now;
    public SessionStatus SessionStatus { get; private set; } = SessionStatus.InLobby;

    public bool AddUser(User user)
    {
        if (!SessionUsers.Contains(user))
        {
            SessionUsers.Add(user);
            return true;
        }

        return false;
    }

    public bool RemoveUser(User user)
    {
        return SessionUsers.Remove(user);
    }

    public void ClearUsers()
    {
        SessionUsers.Clear();
    }

    public bool AddGame(BaseGame game)
    {
        if (!Games.Contains(game))
        {
            Games.Add(game);

            // Set the current game to the newly added game
            CurrentGameId = game.Id;
            return true;
        }
        return false;
    }

    public bool IsClosed()
    {
        return CheckStatus(SessionStatus.Closed);
    }

    public bool IsInGame()
    {
        return CheckStatus(SessionStatus.InGame);
    }

    public bool IsInLobby()
    {
        return CheckStatus(SessionStatus.InLobby);
    }

    private bool CheckStatus(SessionStatus status)
    {
        return SessionStatus == status;
    }

    public void InGame()
    {
        UpdateStatus(SessionStatus.InGame);
    }

    public void InLobby()
    {
        UpdateStatus(SessionStatus.InLobby);
    }

    public void CloseLobby()
    {
        UpdateStatus(SessionStatus.Closed);
    }

    private void UpdateStatus(SessionStatus status)
    {
        SessionStatus = status;
    }

    public bool UserIsOracle(Guid userId) => ChosenOracleId == userId;
    public bool UserIsSessionHost(Guid userId) => SessionHostId == userId;
}