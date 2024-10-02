
using Image_guesser.Core.Domain.GameContext.Events;
using Image_guesser.Core.Domain.SessionContext;

namespace Tests.Unit.Core.Domain.GameContext.Events;

public class CreateGameTests
{
    [Fact]
    public void ConstructorWithSession_ShouldAssignValues()
    {
        var session = new Session();
        var createGame = new CreateGame(session);

        Assert.Equal(session, createGame.Session);
    }
}