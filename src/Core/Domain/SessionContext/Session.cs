using Image_guesser.Core.Domain.UserContext;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.SessionContext;

public class Session : BaseEntity
{
    public Session() { }

    public Session(User User, Guid id)
    {
        Id = id;
        SessionHostId = User.Id;
        ChosenOracleId = User.Id;
        SessionUsers = [User];
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SessionHostId { get; set; }
    public Guid ChosenOracleId { get; set; }
    public List<User> SessionUsers { get; set; } = [];
    public Options Options { get; set; } = new();
    public DateTime TimeOfCreation { get; set; } = DateTime.Now;
    public SessionStatus SessionStatus { get; private set; } = SessionStatus.InLobby;

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
}