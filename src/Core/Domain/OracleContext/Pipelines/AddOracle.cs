using Image_guesser.Infrastructure;
using MediatR;

namespace Image_guesser.Core.Domain.OracleContext.Pipelines
{
    public class AddOracle<T> where T : class
    {
        public record Request(T Oracle, string ImageIdentifier) : IRequest<Guid>;

        public class Handler(ImageGameContext db) : IRequestHandler<Request, Guid>
        {
            private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

            public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
            {
                // Create an instance of GenericOracle<T> where T is the type of Oracle
                var genericOracle = new GenericOracle<T>(request.Oracle);
                genericOracle.AssignImageId(request.ImageIdentifier);

                _db.Oracles.Add(genericOracle);
                await _db.SaveChangesAsync(cancellationToken);

                return genericOracle.Id;
            }
        }
    }
}