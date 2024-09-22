using Image_guesser.SharedKernel;

namespace Tests.Unit.SharedKernel;
public class PasswordRequirementsTests
{
    [Fact]
    public void DefaultRequirements_ShouldBeValid()
    {
        var requirements = new PasswordRequirements();

        Assert.Equal(8, requirements.MinLength);
        Assert.True(requirements.RequireDigit);
        Assert.True(requirements.RequireLowercase);
        Assert.True(requirements.RequireNonAlphanumeric);
        Assert.True(requirements.RequireUppercase);
    }
}