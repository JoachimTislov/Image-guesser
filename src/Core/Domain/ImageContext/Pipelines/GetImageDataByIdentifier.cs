using Image_guesser.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.ImageContext.Pipelines;
public class GetImageDataByIdentifier
{
    public record Request(string Identifier) : IRequest<ImageData>;

    public class Handler(ImageGameContext db) : IRequestHandler<Request, ImageData?>
    {
        private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<ImageData?> Handle(Request request, CancellationToken cancellationToken)
        {
            var Image = await _db.ImageRecords
            .Where(i => i.Identifier == request.Identifier)
            .SingleOrDefaultAsync(cancellationToken);

            if (Image != null)
            {
                Image.Link = Image.Link.Replace("\\", Path.DirectorySeparatorChar.ToString());
                Image.FolderWithImagePiecesLink = Image.FolderWithImagePiecesLink.Replace("\\", Path.DirectorySeparatorChar.ToString());
            }

            return Image;
        }
    }
}