using Image_guesser.Core.Exceptions;
using Image_guesser.Infrastructure.GenericRepository;

namespace Image_guesser.Core.Domain.ImageContext.Repository;

public class ImageRepository(IRepository repository) : IImageRepository
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public void AddImageRecord(ImageRecord ImageRecord)
    {
        _repository.Add(ImageRecord);
    }

    public async Task<ImageRecord> GetImageRecordById(string ImageIdentifier)
    {
        var ImageRecord = await _repository.GetSingleWhere<ImageRecord, string>(i => i.Identifier == ImageIdentifier, ImageIdentifier);
        return ImageRecord;
    }

    public List<ImageRecord> GetImageRecordsByIds(List<string> ImageIdentifiers)
    {
        return _repository.Where<ImageRecord>(imageRecord => ImageIdentifiers.Contains(imageRecord.Identifier)).ToList();
    }

    public async Task<int> GetImagePieceCountById(string ImageIdentifier)
    {
        var ImagePieceCount = await _repository.WhereAndSelect_SingleOrDefault<ImageRecord, int?>(
        /*Where*/    i => i.Identifier == ImageIdentifier,
        /*Select*/   i => i.PieceCount) ?? throw new EntityNotFoundException($"Image Piece count was not found by image identifier: {ImageIdentifier}");

        return ImagePieceCount;
    }

    public async Task<string> GetRandomImageIdentifier()
    {
        var MappedImagesFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet", "Image_dictionary.csv");

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

    public async Task CreateImageDictionary()
    {
        var DataSetPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet");

        var image_mapping_lines = await File.ReadAllLinesAsync(Path.Combine(DataSetPath, "image_mapping.csv"));

        string[] subdirectories = Directory.GetDirectories(Path.Combine(DataSetPath, "Scattered_Images"));

        // Print the names of all subdirectories
        foreach (var subdirectory in subdirectories)
        {
            var name = subdirectory.Split('\\')[^1];

            foreach (var line in image_mapping_lines)
            {
                var split = line.Split(' ');
                var imageIdentifier = split[0];
                var image_Label = split[1];

                var imageName = await GetImageNameBy_Label_Mapping(DataSetPath, image_Label);

                if (name == $"{imageIdentifier}_scattered" && !string.IsNullOrEmpty(imageName))
                {
                    File.AppendAllLines(Path.Combine(DataSetPath, "Image_dictionary.csv"), [$"{imageIdentifier}_scattered {imageName}"]);
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
            var image_name = string.Join(" ", split.Skip(1));

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
        var Image_dictionary = Path.Combine(DataSetFolder, "Image_dictionary.csv");

        if (!File.Exists(Image_dictionary))
        {
            await CreateImageDictionary();
        }

        foreach (var line in await File.ReadAllLinesAsync(Image_dictionary))
        {
            var split = line.Split(' ');
            var image_Identifier = split[0];
            var image_Name = string.Join(" ", split.Skip(1));

            var pathToImagePieces = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet", "Scattered_Images", image_Identifier);
            if (Directory.Exists(pathToImagePieces))
            {
                var ImageLink = Path.Combine("DataSet", "MergedImages", $"{image_Identifier}.png");
                var ImagePiecesFolderLink = Path.Combine("DataSet", "Scattered_Images", image_Identifier);
                var pieceCount = Directory.GetFiles(Path.Combine("wwwroot", ImagePiecesFolderLink)).Length;

                AddImageRecord(new ImageRecord(image_Name, image_Identifier, ImageLink, ImagePiecesFolderLink, pieceCount));
            }
            else
            {
                Console.WriteLine($"ImagePiecesFolder with identifier {image_Identifier} was not found in the DataSet folder with path: {pathToImagePieces}");
            }
        }
    }
}