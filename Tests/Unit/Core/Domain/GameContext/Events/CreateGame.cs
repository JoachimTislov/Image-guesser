using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.SessionContext;

namespace Tests.Unit.Core.Domain.GameContext.Events;

public class CreateGameTests
{
    [Fact]
    public void ConstructorWithSession_ShouldAssignValues()
    {
        var sessionId = Guid.NewGuid();
        var imageIdentifier = "imageIdentifier";
        var createGame = new CreateGame(sessionId, imageIdentifier);

        Assert.Equal(sessionId, createGame.SessionId);
        Assert.Equal(imageIdentifier, createGame.ImageIdentifier);
    }
}