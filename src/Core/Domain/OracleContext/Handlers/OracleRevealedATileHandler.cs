using Image_guesser.Core.Domain.OracleContext.Services;
using MediatR;

namespace Image_guesser.Core.Domain.OracleContext.Handlers;

public class OracleRevealedATileHandler(IOracleService oracleService) : INotificationHandler<OracleRevealedATile>
{
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));

    public async Task Handle(OracleRevealedATile notification, CancellationToken cancellationToken)
    {
        var oracle = await _oracleService.GetBaseOracleById(notification.OracleId);

        oracle.IncrementTiles();

        await _oracleService.UpdateBaseOracle(oracle);
    }
}