using Microsoft.AspNetCore.Identity;

namespace Image_guesser.Core.Domain.UserContext;

public class User : IdentityUser<Guid>
{
    public User() { }

    public User(string username)
    {
        UserName = username;
    }

    public override string? Email { get; set; }
    public Guid? SessionId { get; set; }
    public int Played_Games { get; set; }
    public int Correct_Guesses { get; set; }
    public int Points { get; set; }
    public string CustomSizedImageTilesDirectoryId { get; set; } = string.Empty;

    // Probably unnecessary to store the last visited page in the database, but an easy way to register were the user is. 
    public string CurrentPageUrl { get; set; } = "/Home/Index";
}