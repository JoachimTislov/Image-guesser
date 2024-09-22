using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;

namespace Tests.Unit.Core.Domain.SessionContext;

public class SessionTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var session = new Session();
        var sessionOptions = new Options();

        Assert.Equal(Guid.Empty, session.Id);
        Assert.Equal(Guid.Empty, session.SessionHostId);
        Assert.Equal(Guid.Empty, session.ChosenOracleId);
        Assert.Empty(session.SessionUsers);

        foreach (var property in sessionOptions.GetType().GetProperties())
        {
            var expectedValue = property.GetValue(sessionOptions);
            var actualValue = property.GetValue(session.Options);

            Assert.Equal(expectedValue, actualValue);
        }

        var now = DateTime.Now;
        Assert.InRange(session.TimeOfCreation, now.AddSeconds(-1), now.AddSeconds(1));

        Assert.Equal(string.Empty, session.ImageIdentifier);
        Assert.Equal(string.Empty, session.ChosenImageName);
        Assert.Equal(SessionStatus.Lobby, session.SessionStatus);
    }

    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        var user = new User();
        var session = new Session(user);

        Assert.Equal(user.Id, session.SessionHostId);
        Assert.Equal(user.Id, session.ChosenOracleId);

        Assert.Single(session.SessionUsers);
    }
}