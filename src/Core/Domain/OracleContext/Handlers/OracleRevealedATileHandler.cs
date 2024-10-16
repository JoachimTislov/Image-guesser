using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.Core.Domain.OracleContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.OracleContext.Handlers;

public class OracleRevealedATileHandler(IOracleService oracleService, IImageService imageService) : INotificationHandler<OracleRevealedATile>
{
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));

    public async Task Handle(OracleRevealedATile notification, CancellationToken cancellationToken)
    {
        var (oracleId, imagePath) = notification;

        var oracle = await _oracleService.GetBaseOracleById(oracleId);
        var imageRecord = await _imageService.GetImageRecordById(oracle.ImageIdentifier);

        if (oracle.NumberOfTilesRevealed < imageRecord.PieceCount)
        {
            oracle.IncrementTiles();
        }

        var imageFileName = Path.GetFileName(imagePath);
        if (!oracle.ImageTileOrderLog.Contains(imageFileName))
        {
            oracle.ImageTileOrderLog.Add(imageFileName);
        }

        await _oracleService.UpdateOracle(oracle);
    }
}