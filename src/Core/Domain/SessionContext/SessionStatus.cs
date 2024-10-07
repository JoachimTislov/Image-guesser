using System.ComponentModel.DataAnnotations;

namespace Image_guesser.Core.Domain.SessionContext;
public enum SessionStatus
{
    [Display(Name = "In Lobby")]
    InLobby,

    [Display(Name = "In Game")]
    InGame,

    Closed,
}