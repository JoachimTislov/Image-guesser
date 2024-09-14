using Image_guesser.Core.Domain.UserContext;

namespace Tests.Core.Domain.UserContext;

public class UserTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var user = new User();

        Assert.Null(user.Email);
        Assert.Null(user.SessionId);
        Assert.Equal(0, user.Played_Games);
        Assert.Equal(0, user.Correct_Guesses);
        Assert.Equal(0, user.Points);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var number = 1;
        var email = "player.test@gmail.com";
        var username = "TestPlayer";
        var sessionId = Guid.NewGuid();
        var user = new User()
        {
            UserName = username,
            Email = email,
            SessionId = sessionId,
            Played_Games = number,
            Correct_Guesses = number,
            Points = number,
        };

        Assert.Equal(username, user.UserName);
        Assert.Equal(email, user.Email);
        Assert.Equal(sessionId, user.SessionId);
        Assert.Equal(number, user.Played_Games);
        Assert.Equal(number, user.Correct_Guesses);
        Assert.Equal(number, user.Points);

    }

}