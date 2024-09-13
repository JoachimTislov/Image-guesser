using MediatR;

namespace Image_guesser.Core.Domain.ImageContext.Pipelines;

//This is only needed if you want add the Images with relevant info to the database 
//For example when you delete the database and want to add the images again

//Does not work atm, will delete this later
public class AddAllMappedImagesToDatabase
{
    public record Request() : IRequest<Unit>;

    public class Handler(IMediator mediator) : IRequestHandler<Request, Unit>
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var DataSetFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet");
            var Mapped_ImagesFile = Path.Combine(DataSetFolder, "Mapped_Images.csv");

            if (File.Exists(Mapped_ImagesFile))
            {
                var Mapped_Images_Lines = await File.ReadAllLinesAsync(Mapped_ImagesFile, cancellationToken);

                foreach (var line in Mapped_Images_Lines)
                {
                    var split = line.Split(' ');
                    var image_Identifier = split[0];
                    var image_Name = split[1];

                    var ImageLink = Path.Combine("DataSet", "MergedImages", image_Identifier + ".png");

                    var ImagePiecesFolderLink = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet", "ScatteredImages", image_Identifier);
                    var pieceCount = Directory.GetFiles(ImagePiecesFolderLink).Length;

                    var ImageData = new ImageData(image_Name, image_Identifier, ImageLink, ImagePiecesFolderLink, pieceCount);
                    await _mediator.Send(new AddImageData.Request(ImageData), cancellationToken);

                    //MERGE IMAGES
                    //await _mediator.Send(new ImageMerger.Request(ImageData.Identifier), cancellationToken);
                }
            }
            return Unit.Value;
        }
    }
}