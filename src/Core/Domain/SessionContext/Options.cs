using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.SessionContext;

[Owned]
public class Options
{
    public Options() { }

    // Init as SinglePlayer for new Sessions
    public int NumberOfRounds { get; private set; } = 1;
    public int LobbySize { get; private set; } = 1;
    public GameMode GameMode { get; private set; } = GameMode.SinglePlayer;
    public bool RandomUserOracle { get; private set; }
    public OracleTypes Oracle { get; private set; } = OracleTypes.AI;
    public AI_Type AI_Type { get; private set; } = AI_Type.Random;
    public bool RandomPictureMode { get; private set; } = true;
    public string ImageIdentifier { get; private set; } = string.Empty;

    public void SetOptionsValues(ViewModelOptions options)
    {
        switch (options.GameMode)
        {
            case GameMode.SinglePlayer:
                Oracle = OracleTypes.AI;
                LobbySize = 1;
                break;

            case GameMode.Duo:
                Oracle = OracleTypes.User;
                LobbySize = 2;
                break;

            case GameMode.FreeForAll:
                Oracle = options.Oracle;
                LobbySize = ValidateRange(options.LobbySize, 3, 10);
                break;

            default:
                break;
        }

        NumberOfRounds = ValidateRange(options.NumberOfRounds, 1, 10);
        GameMode = options.GameMode;
        AI_Type = options.AI_Type;
        RandomUserOracle = !IsGameMode(GameMode.SinglePlayer) && !IsOracleAI() && options.RandomUserOracle;
        RandomPictureMode = IsGameMode(GameMode.SinglePlayer) || options.RandomPictureMode;
        ImageIdentifier = !IsGameMode(GameMode.SinglePlayer) && !RandomPictureMode ? options.ImageIdentifier : string.Empty;
    }

    public bool IsOracleAI()
    {
        return Oracle == OracleTypes.AI;
    }

    public bool IsGameMode(GameMode gameMode)
    {
        return GameMode == gameMode;
    }

    public void DecrementNumberOfRounds() => NumberOfRounds--;

    private static int ValidateRange(int number, int start, int end)
    {
        return number < start ? start : number > end ? end : number;
    }
}