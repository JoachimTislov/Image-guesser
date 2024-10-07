using Image_guesser.Core.Domain.UserContext;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.SessionContext;

public class Session : BaseEntity
{
    public Session() { }

    public Session(User user, Guid id, string randomImageIdentifier)
    {
        Id = id;
        SessionHostId = user.Id;
        ChosenOracleId = user.Id;
        SessionUsers = [user];
        Options = new(randomImageIdentifier);
    }

    public Guid Id { get; private set; }
    public Guid SessionHostId { get; set; }
    public Guid ChosenOracleId { get; set; } // Maybe change this to nullable, since it wont be used unless the session has an oracle
    public List<User> SessionUsers { get; set; } = [];
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

    public bool HasPlayedSetAmountGames => Options.NumberOfGamesToPlay <= Options.AmountOfGamesPlayed;

    public bool UserIsOracle(Guid userId) => ChosenOracleId == userId;
    public bool UserIsSessionHost(Guid userId) => SessionHostId == userId;
}