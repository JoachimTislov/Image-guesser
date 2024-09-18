using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.SessionContext.Events;

namespace Tests.Unit.Core.Domain.SessionContext.Events;

public class SessionClosedTests
{
    [Fact]
    public void ConstructorWithSession_ShouldAssignTheSession()
    {
        var session = new Session();
        var sessionClosed = new SessionClosed(session);

        Assert.Equal(session.Id, sessionClosed.Session.Id);
        Assert.Equal(session.ChosenImageName, sessionClosed.Session.ChosenImageName);
    }
}