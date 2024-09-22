using Image_guesser.Core.Domain.OracleContext.Events;
using Image_guesser.Core.Domain.OracleContext.Services;
using Image_guesser.Infrastructure.GenericRepository;
using MediatR;

namespace Image_guesser.Core.Domain.OracleContext.Handlers;

public class OracleRevealedATileHandler(IRepository repository, IOracleService oracleService) : INotificationHandler<OracleRevealedATile>
{
    private readonly IOracleService _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(oracleService));

    public async Task Handle(OracleRevealedATile notification, CancellationToken cancellationToken)
    {
        var oracle = await _oracleService.GetBaseOracleById(notification.OracleId);

        oracle.IncrementTiles();

        await _repository.Update(oracle);
    }
}