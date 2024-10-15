using Image_guesser.Core.Domain.OracleContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.OracleContext.Handlers;

public class OracleRevealedATileHandler(IOracleService oracleService) : INotificationHandler<OracleRevealedATile>
{
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));

    public async Task Handle(OracleRevealedATile notification, CancellationToken cancellationToken)
    {
        var (oracleId, imageId) = notification;

        var oracle = await _oracleService.GetBaseOracleById(oracleId);

        oracle.IncrementTiles();
        if (!oracle.ImageTileOrderLog.Contains(imageId))
        {
            oracle.ImageTileOrderLog.Add(imageId);
        }

        await _oracleService.UpdateOracle(oracle);
    }
}