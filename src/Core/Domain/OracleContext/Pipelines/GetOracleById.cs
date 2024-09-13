using Image_guesser.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.OracleContext.Pipelines;

public class GetOracleById<T> where T : class
{
    public record Request(Guid OracleId) : IRequest<GenericOracle<T>>;

    public class Handler(ImageGameContext db) : IRequestHandler<Request, GenericOracle<T>>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<GenericOracle<T>> Handle(Request request, CancellationToken cancellationToken)
        {
            var Oracle = await _db.Oracles
              .Where(o => o.Id == request.OracleId)
              .OfType<GenericOracle<T>>()
              .SingleOrDefaultAsync(cancellationToken) ?? throw new Exception($"Oracle, with type: {typeof(T)} and Id: {request.OracleId} not found");

            return Oracle;
        }
    }
}
