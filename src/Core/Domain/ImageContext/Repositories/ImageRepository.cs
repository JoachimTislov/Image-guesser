using Image_guesser.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Core.Domain.ImageContext.Repositories;

public class ImageRepository(ImageGameContext db) : IImageRepository
{
    private readonly ImageGameContext _db = db ?? throw new ArgumentNullException(nameof(db));

    public void AddImageRecord(ImageRecord ImageRecord)
    {
        _db.ImageRecords.Add(ImageRecord);
    }

    public async Task<ImageRecord> GetImageRecordById(string ImageIdentifier)
    {
        // Handling random image game case
        if (ImageIdentifier == string.Empty)
        {
            ImageIdentifier = await GetRandomImageIdentifier();
        }

        var ImageRecord = await _db.ImageRecords.Where(i => i.Identifier == ImageIdentifier)
        .SingleOrDefaultAsync() ?? throw new ArgumentNullException($"Image record was not found with identifier: {ImageIdentifier}");

        return ImageRecord;
    }

    public async Task<int> GetImagePieceCountById(string ImageIdentifier)
    {
        // Handling missing image identifier case
        if (ImageIdentifier == string.Empty)
        {
            throw new ArgumentException("Image Identifier is required to find piece count for given image");
        }

        var ImagePieceCount = await _db.ImageRecords.Where(i => i.Identifier == ImageIdentifier).Select(i => i.PieceCount)
        .SingleOrDefaultAsync();

        return ImagePieceCount;
    }

    public async Task<string> GetRandomImageIdentifier()
    {
        var MappedImagesFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet", "Mapped_Images.csv");

        if (!File.Exists(MappedImagesFile))
        {
            throw new Exception($"{MappedImagesFile} does not exist");
        }
        var Mapped_Images_Lines = await File.ReadAllLinesAsync(MappedImagesFile);

        var random = new Random();

        var randomNumber = random.Next(0, Mapped_Images_Lines.Length);
        var Line = Mapped_Images_Lines[randomNumber];

        var split = Line.Split(' ');
        var image_Identifier = split[0];

        return image_Identifier;
    }

    public async Task Map_Images()
    {
        var DataSetPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet");

        var image_mapping_lines = await File.ReadAllLinesAsync(Path.Combine(DataSetPath, "image_mapping.csv"));

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
                var image_Label = split.Length > 1 ? split[1] : null;

                if (name == $"{imageIdentifier}_scattered" && image_Label != null)
                {
                    var imageName = await GetImageNameBy_Label_Mapping(image_Label, DataSetPath);
                    Console.WriteLine($"{imageIdentifier}_scattered, {imageName}");
                }
            }
        }
    }

    private async static Task<string> GetImageNameBy_Label_Mapping(string DataSetPath, string ImageLabel)
    {
        var label_mappingPath = Path.Combine($"{DataSetPath}/label_mapping.csv");
        var label_mapping_lines = await File.ReadAllLinesAsync(label_mappingPath);

        foreach (var line in label_mapping_lines)
        {
            var split = line.Split(' ');
            var nameId = split[0];
            var image_name = split[1];

            if (nameId == ImageLabel)
            {
                return image_name;
            }
        }
        return string.Empty;
    }

    public async Task AddAllMappedImagesToDatabase()
    {
        var DataSetFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet");
        var Mapped_ImagesFile = Path.Combine(DataSetFolder, "Mapped_Images.csv");

        if (File.Exists(Mapped_ImagesFile))
        {
            var Mapped_Images_Lines = await File.ReadAllLinesAsync(Mapped_ImagesFile);

            foreach (var line in Mapped_Images_Lines)
            {
                var split = line.Split(' ');
                var image_Identifier = split[0];
                var image_Name = split[1];

                var ImageLink = Path.Combine("DataSet", "MergedImages", image_Identifier + ".png");

                var ImagePiecesFolderLink = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet", "ScatteredImages", image_Identifier);
                var pieceCount = Directory.GetFiles(ImagePiecesFolderLink).Length;

                var ImageRecord = new ImageRecord(image_Name, image_Identifier, ImageLink, ImagePiecesFolderLink, pieceCount);
                AddImageRecord(ImageRecord);

                //MERGE IMAGES
                //await _mediator.Send(new ImageMerger.Request(ImageRecord.Identifier), cancellationToken);
            }
        }
    }
}