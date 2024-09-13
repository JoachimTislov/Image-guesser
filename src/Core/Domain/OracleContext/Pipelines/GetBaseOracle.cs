using Image_guesser.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.OracleContext.Pipelines;

public class GetBaseOracleById
{
    public record Request(Guid OracleId) : IRequest<BaseOracle>;

    public class Handler(ImageGameContext db) : IRequestHandler<Request, BaseOracle>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<BaseOracle> Handle(Request request, CancellationToken cancellationToken)
        {
            var Oracle = await _db.Oracles
              .Where(o => o.Id == request.OracleId)
              .SingleOrDefaultAsync(cancellationToken) ?? throw new Exception($"BaseOracle with Id: {request.OracleId}, not found");

            return Oracle;
        }
    }
}
