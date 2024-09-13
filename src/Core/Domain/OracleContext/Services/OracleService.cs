using Image_guesser.Core.Domain.ImageContext.Pipelines;
using Image_guesser.Core.Domain.OracleContext.Pipelines;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static Image_guesser.Core.Domain.OracleContext.Services.IOracleService;

namespace Image_guesser.Core.Domain.OracleContext.Services;

public class OracleService(IMediator mediator, UserManager<User> userManager) : IOracleService
{

    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    /*This method generates an array of random numbers,  
    where each number in the row represents a piece of that image.*/

    // This type of "AI" determines which order the tiles are revealed 
    // We can run the function again, and get a new array or new "AI" 

    public RandomNumbersAI CreateRandomNumbersAI(int ImagePieceCount)
    {
        // Enumerates the numbers from 1 to the number of image pieces/tiles in the image
        // Creates an array of the numbers
        int[] ArrayOfNumbers = Enumerable.Range(1, ImagePieceCount).ToArray();

        // Shuffles the array of numbers
        var MixedArray = ShuffleArray(ArrayOfNumbers);

        RandomNumbersAI OracleAI = new(MixedArray);

        return OracleAI;
    }

    // Creates a random object
    readonly Random random = new();

    //  Fisher-Yates shuffle algorithm
    public int[] ShuffleArray(int[] ArrayOfNumbers)
    {
        int LengthOfArray = ArrayOfNumbers.Length;

        while (LengthOfArray > 1)
        {
            int randomNumber = random.Next(LengthOfArray--);

            // Swaps the numbers in the array
            (ArrayOfNumbers[randomNumber], ArrayOfNumbers[LengthOfArray]) = (ArrayOfNumbers[LengthOfArray], ArrayOfNumbers[randomNumber]);
        }
        return ArrayOfNumbers;
    }

    public async Task<Guid> CreateOracle(bool OracleIsAI, string imageIdentifier,
                                int NumberOfRounds, User ChosenOracle, string GameMode)
    {
        var ImageData = imageIdentifier == "RandomImage"
                    ? await _mediator.Send(new GetRandomImage.Request())
                    : await _mediator.Send(new GetImageDataByIdentifier.Request(imageIdentifier));

        var ImageIdentifier = ImageData.Identifier;
        var PieceCount = ImageData.PieceCount;

        switch (OracleIsAI)
        {
            case true:
                var oracleAI = CreateRandomNumbersAI(PieceCount);
                return await _mediator.Send(new AddOracle<RandomNumbersAI>.Request(oracleAI, ImageIdentifier));

            case false:
                return await _mediator.Send(new AddOracle<User>.Request(ChosenOracle, ImageIdentifier));
        }
    }

    public async Task<Response> CheckGuess(string Guess, string ImageIdentifier, User User, Session Session)
    {
        var imageData = await _mediator.Send(new GetImageDataByIdentifier.Request(ImageIdentifier));

        var winnerText = "Congratulations: ";

        if (Guess.Equals(imageData.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            if (User != null)
                winnerText += User.UserName;

            if (Session != null)
            {
                var ChosenOracleId = Session.ChosenOracle;
                if (Session.Options.GameMode == GameMode.Duo)
                {
                    var oracle = await _userManager.FindByIdAsync(ChosenOracleId.ToString());
                    if (oracle != null) winnerText += " and " + oracle.UserName;
                }
            }

            return new Response(true, winnerText);
        }

        return new Response(false, "Wrong");
    }
}
