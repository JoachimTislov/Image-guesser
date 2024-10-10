using System.ComponentModel.DataAnnotations;

namespace Image_guesser.Core.Domain.SessionContext;

public enum GameMode
{
    [Display(Name = "Singleplayer")]
    SinglePlayer,

    Duo,

    [Display(Name = "Free for all")]
    FreeForAll
}