using MediatR;

// This code is not used anymore, unless
// we lose the mapping file, which we also don't need
// Unless we loose all ImageRecords in the database

namespace Image_guesser.Core.Domain.ImageContext.Pipelines;

public class MappingImages
{
    public record Request() : IRequest<Unit>;

    public class Handler(IMediator mediator) : IRequestHandler<Request, Unit>
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var DataSetPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet");

            var image_mapping_lines = await File.ReadAllLinesAsync(Path.Combine(DataSetPath, "image_mapping.csv"), cancellationToken);

            var imagePath = Path.Combine(DataSetPath, "ScatteredImages");

            string[] subdirectories = Directory.GetDirectories(imagePath);

            // Print the names of all subdirectories
            for (var i = 0; i < subdirectories.Length; i++)
            {
                var split2 = subdirectories[i].Split('\\');
                var name = split2[^1];
                for (var j = 0; j < image_mapping_lines.Length; j++)
                {
                    var Line = image_mapping_lines[j];
                    var split = Line.Split(' ');
                    var imageIdentifier = split[0];
                    var image_nameId = split.Length > 1 ? split[1] : null;

                    if (name == $"{imageIdentifier}_scattered" && image_nameId != null)
                    {
                        var imageName = await _mediator.Send(new GetImageNameBy_Label_Mapping.Request(image_nameId, DataSetPath), cancellationToken);
                        Console.WriteLine($"{imageIdentifier}_scattered, {imageName}");
                    }
                }
            }

            return Unit.Value;
        }
    }
}