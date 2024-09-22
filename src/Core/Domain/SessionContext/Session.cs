using Image_guesser.Core.Domain.UserContext;
using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.SessionContext;

public class Session : BaseEntity
{
    public Session() { }

    public Session(User User)
    {
        SessionHostId = User.Id;
        ChosenOracleId = User.Id;
        SessionUsers = [User];
    }

    public Guid Id { get; protected set; }
    public Guid SessionHostId { get; set; }
    public Guid ChosenOracleId { get; set; }
    public List<User> SessionUsers { get; set; } = [];
    public Options Options { get; set; } = new();
    public DateTime TimeOfCreation { get; set; } = DateTime.Now;
    public string ImageIdentifier { get; set; } = string.Empty;
    public string ChosenImageName { get; set; } = string.Empty;
    public SessionStatus SessionStatus { get; set; } = SessionStatus.Lobby;

}