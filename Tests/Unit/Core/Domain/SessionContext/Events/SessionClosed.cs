using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Events;

namespace Tests.Unit.Core.Domain.SessionContext.Events;

public class SessionClosedTests
{
    [Fact]
    public void ConstructorWithSession_ShouldAssignTheSession()
    {
        var sessionId = Guid.NewGuid();
        var sessionClosed = new SessionClosed(sessionId);

        Assert.Equal(sessionId, sessionClosed.SessionId);
    }
}