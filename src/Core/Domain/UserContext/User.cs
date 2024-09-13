using Microsoft.AspNetCore.Identity;

namespace Image_guesser.Core.Domain.UserContext;

public class User() : IdentityUser<Guid>
{
    public override string? Email { get; set; }
    public Guid? SessionId { get; set; }
    public int Played_Games { get; set; }
    public int Correct_Guesses { get; set; }
    public int Points { get; set; }
}