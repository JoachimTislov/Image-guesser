using Image_guesser.Core.Domain.ImageContext.Repository;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Image_guesser.Core.Domain.ImageContext.Services;

public class ImageService(IWebHostEnvironment hostingEnvironment, IImageRepository imageRepository) : IImageService
{
    private readonly IWebHostEnvironment _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
    private readonly IImageRepository _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));

    public async Task<ImageRecord> GetImageRecordById(string ImageIdentifier)
    {
        ImageRecord ImageRecord = await _imageRepository.GetImageRecordById(ImageIdentifier);

        ImageRecord.Link = MakeLinkCompatibleWithJavascript(ImageRecord.Link);
        ImageRecord.FolderWithImagePiecesLink = MakeLinkCompatibleWithJavascript(ImageRecord.FolderWithImagePiecesLink);

        return ImageRecord;
    }

    public async Task<int> GetImagePieceCountById(string ImageIdentifier)
    {
        return await _imageRepository.GetImagePieceCountById(ImageIdentifier);
    }

    public void AddImageRecord(ImageRecord ImageRecord)
    {
        _imageRepository.AddImageRecord(ImageRecord);
    }

    public async Task<List<ImageRecord>> GetXAmountOfImageRecords(int amount)
    {
        List<string> imageIdentifiers = [];

        for (var i = 0; i < amount; i++)
        {
            string imageId = await _imageRepository.GetRandomImageIdentifier();
            imageIdentifiers.Add(imageId);
        }

        var imageRecords = _imageRepository.GetImageRecordsByIds(imageIdentifiers);

        foreach (var record in imageRecords)
        {
            record.Link = MakeLinkCompatibleWithJavascript(record.Link);
            record.FolderWithImagePiecesLink = MakeLinkCompatibleWithJavascript(record.FolderWithImagePiecesLink);
        }

        return imageRecords;
    }

    private static string MakeLinkCompatibleWithJavascript(string Link)
    {
        return Link.Replace("\\", "/");
    }

    public List<string> GetFileNameOfImagePieces(string imageIdentifier)
    {
        var imagePiecesFolderPath = Path.Combine("wwwroot", "DataSet", "ScatteredImages", imageIdentifier);
        var imagePieceFileNames = Directory.GetFiles(imagePiecesFolderPath).ToList();

        //Filter out all files that are not images
        //This is needed because the folder contains a .DS_Store file
        List<string> validExtensions = [".png"];
        List<string> filteredFileNames = imagePieceFileNames
            .Where(fileName => validExtensions.Contains(Path.GetExtension(fileName).ToLower())).ToList();

        return filteredFileNames;
    }

    public List<(string ImageId, List<(int X, int Y)> Coordinates)> GetCoordinatesForImagePieces(string imagePiecesFolderPath, List<string> ImagePieceList)
    {
        List<(string ImageId, List<(int X, int Y)> Coordinates)> _imageCoordinates = [];

        if (Directory.Exists(imagePiecesFolderPath))
        {
            foreach (var image in ImagePieceList)
            {
                var modifiedPath = image.Replace($"wwwroot{Path.DirectorySeparatorChar}", "");
                var relativeImagePath = Path.Combine(_hostingEnvironment.WebRootPath, modifiedPath);
                _imageCoordinates.Add((image, GetNonTransparentPixelCoordinates(relativeImagePath)));
            }

            return _imageCoordinates;
        }
        else
        {
            throw new Exception($"{imagePiecesFolderPath} does not exist");
        }
    }

    private static List<(int X, int Y)> GetNonTransparentPixelCoordinates(string imagePiecePath)
    {
        List<(int X, int Y)> nonTransparentPixels = [];

        using (Image<Rgba32> image = Image.Load<Rgba32>(imagePiecePath))
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (image[x, y].A != 0)
                    {
                        nonTransparentPixels.Add((x, y));
                    }
                }
            }
        }

        return nonTransparentPixels;
    }
}