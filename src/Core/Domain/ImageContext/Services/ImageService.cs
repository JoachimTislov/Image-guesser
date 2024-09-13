using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Image_guesser.Core.Domain.ImageContext.Services;

public class ImageService(IWebHostEnvironment hostingEnvironment) : IImageService
{
    private readonly IWebHostEnvironment _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));

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

    private readonly List<(string ImageId, List<(int X, int Y)> Coordinates)> _imageCoordinates = [];

    public List<(string ImageId, List<(int X, int Y)> Coordinates)> GetCoordinatesForImagePieces(string imagePiecesFolderPath, List<string> ImagePieceList)
    {
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

    public List<(int X, int Y)> GetNonTransparentPixelCoordinates(string imagePiecePath)
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