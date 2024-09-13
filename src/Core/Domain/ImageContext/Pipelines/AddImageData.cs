using Image_guesser.Infrastructure;
using MediatR;

namespace Image_guesser.Core.Domain.ImageContext.Pipelines;

public class AddImageData
{
    public record Request(ImageData ImageData) : IRequest<ImageData>;

    public class Handler(ImageGameContext db) : IRequestHandler<Request, ImageData>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<ImageData> Handle(Request request, CancellationToken cancellationToken)
        {
            _db.ImageRecords.Add(request.ImageData);

            await _db.SaveChangesAsync(cancellationToken);

            return request.ImageData;
        }
    }
}