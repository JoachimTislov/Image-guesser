using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SessionContext.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.SessionContext;

[Owned]
public class Options
{
    public Options() { }

    public Options(string randomImageIdentifier)
    {
        ImageIdentifier = randomImageIdentifier;
    }

    // Init as SinglePlayer for new Sessions
    public int AmountOfGamesPlayed { get; private set; }
    public int LobbySize { get; private set; } = 1;
    public GameMode GameMode { get; private set; } = GameMode.SinglePlayer;
    public bool RandomUserOracle { get; private set; }
    public OracleTypes Oracle { get; private set; } = OracleTypes.AI;
    public AI_Type AI_Type { get; private set; } = AI_Type.Random;
    public PictureMode PictureMode { get; private set; } = PictureMode.Random;
    public string ImageIdentifier { get; set; } = string.Empty;

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

        GameMode = options.GameMode;
        AI_Type = options.AI_Type;

        RandomUserOracle = !IsGameMode(GameMode.SinglePlayer) && !IsOracleAI() && options.RandomUserOracle;

        PictureMode = !IsGameMode(GameMode.SinglePlayer) ? options.PictureMode : PictureMode.Random;
        ImageIdentifier = options.ImageIdentifier;
    }

    public void ResetAmountOfGamesPlayed() => AmountOfGamesPlayed = 0;

    public void IncrementAmountOfGamesPlayed()
    {
        AmountOfGamesPlayed++;
    }

    public bool IsOracleAI()
    {
        return Oracle == OracleTypes.AI;
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