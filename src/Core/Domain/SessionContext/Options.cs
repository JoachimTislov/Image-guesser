using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.SessionContext;

[Owned]
public class Options
{
    public Options() { }

    // Init as SinglePlayer for new Sessions
    public int AmountOfGamesPlayed { get; private set; }
    public int LobbySize { get; private set; } = 1;
    public GameMode GameMode { get; private set; } = GameMode.SinglePlayer;
    public OracleTypes OracleType { get; private set; } = OracleTypes.AI;
    public UserOracleMode UserOracleMode { get; private set; }
    public AI_Type AI_Type { get; private set; } = AI_Type.Random;
    public PictureMode PictureMode { get; private set; } = PictureMode.Random;

    public void SetOptionsValues(ViewModelOptions options)
    {
        switch (options.GameMode)
        {
            case GameMode.SinglePlayer:
                OracleType = OracleTypes.AI;
                LobbySize = 1;
                break;

            case GameMode.Duo:
                OracleType = OracleTypes.User;
                LobbySize = 2;
                break;

            case GameMode.FreeForAll:
                OracleType = options.OracleType;
                LobbySize = ValidateRange(options.LobbySize, 3, 10);
                break;

            default:
                break;
        }

        GameMode = options.GameMode;
        AI_Type = options.AI_Type;
        UserOracleMode = options.UserOracleMode;

        PictureMode = !IsGameMode(GameMode.SinglePlayer) ? options.PictureMode : PictureMode.Random;
    }

    public void IncrementAmountOfGamesPlayed()
    {
        AmountOfGamesPlayed++;
    }

    public bool IsOracleAI()
    {
        return OracleType == OracleTypes.AI;
    }

    public bool IsGameMode(GameMode gameMode)
    {
        return GameMode == gameMode;
    }

    private static int ValidateRange(int number, int start, int end)
    {
        return number < start ? start : number > end ? end : number;
    }
}