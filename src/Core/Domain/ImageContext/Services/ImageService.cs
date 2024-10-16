using Image_guesser.Core.Domain.ImageContext.Repository;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Image_guesser.Core.Domain.ImageContext.Services;

public class ImageService(IWebHostEnvironment hostingEnvironment, IImageRepository imageRepository) : IImageService
{
    private readonly IWebHostEnvironment _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
    private readonly IImageRepository _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));

    public bool CheckIfUserHasCustomSizedImageTiles(string username, string customSizedImageTiles_ImageIdentifier)
    {
        return Directory.Exists(GetPathToUsersCustomFolder(username, customSizedImageTiles_ImageIdentifier));
    }

    public async Task<ImageRecord> GetImageRecordById(string ImageIdentifier)
    {
        return await _imageRepository.GetImageRecordById(ImageIdentifier);
    }

    public async Task<int> GetImagePieceCountById(string ImageIdentifier)
    {
        return await _imageRepository.GetImagePieceCountById(ImageIdentifier);
    }

    public async Task<string> GetDifferentRandomImageIdentifier(List<string> imageIdentifiers)
    {
        string imageId;
        do
        {
            imageId = await GetRandomImageIdentifier();
        }
        while (imageIdentifiers.Contains(imageId));

        return imageId;
    }

    public async Task<string> GetRandomImageIdentifier()
    {
        return await _imageRepository.GetRandomImageIdentifier();
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

        return imageRecords;
    }

    private string GetPathToUsersCustomFolder(string username, string customSizedImageTiles_ImageIdentifier) => Path.Combine(_hostingEnvironment.WebRootPath, "DataSet", "Custom_Sized_Image_Tiles", $"{username}_{customSizedImageTiles_ImageIdentifier}");

    public List<string> GetFileNameOfImagePieces(string imageIdentifier, string username, string customSizedImageTiles_ImageIdentifier)
    {
        var pathToDataSet = Path.Combine(_hostingEnvironment.WebRootPath, "DataSet");

        var pathToCustomSizedImageTiles = GetPathToUsersCustomFolder(username, customSizedImageTiles_ImageIdentifier);
        var pathToImagePieces = Path.Combine(pathToDataSet, "Scattered_Images", imageIdentifier);

        var piecesFolder = Directory.Exists(pathToCustomSizedImageTiles) ? pathToCustomSizedImageTiles : pathToImagePieces;

        var imagePiecesFileNames = Directory.GetFiles(piecesFolder).ToList();

        //Filter out all files that are not images
        //This is needed because the folder contains a .DS_Store file
        List<string> validExtensions = [".png", ".jpg", ".jpeg"];
        List<string> filteredFileNames = imagePiecesFileNames
            .Where(fileName => validExtensions.Contains(Path.GetExtension(fileName).ToLower())).ToList();

        return filteredFileNames;
    }

    public List<(string ImageId, List<(int X, int Y)> Coordinates)> GetCoordinatesForImagePieces(List<string> ImagePieceList)
    {
        List<(string ImageId, List<(int X, int Y)> Coordinates)> _imageCoordinates = [];

        foreach (var nameOfImagePath in ImagePieceList)
        {
            _imageCoordinates.Add((nameOfImagePath, GetNonTransparentPixelCoordinates(nameOfImagePath)));
        }

        return _imageCoordinates;
    }

    private void CheckIfDirectoryExistsAndIfNotCreateIt(string username, string pathToUsersCustomSizedImageTiles)
    {
        if (!Directory.Exists(pathToUsersCustomSizedImageTiles))
        {
            var foldersInCustomSizedImageTiles = Directory.EnumerateDirectories(Path.Combine(_hostingEnvironment.WebRootPath, "DataSet", "Custom_Sized_Image_Tiles")).ToList();

            // clean up other folders related to the user
            foreach (var folder in foldersInCustomSizedImageTiles)
            {
                if (Path.GetFileName(folder).Contains(username, StringComparison.OrdinalIgnoreCase))
                {
                    Directory.Delete(folder, true);
                }
            }

            Directory.CreateDirectory(pathToUsersCustomSizedImageTiles);
        }
    }

    public void SetSizeOfImagePieces(string imageIdentifier, int width, int height, double imageSize, string username, string customSizedImageTiles_ImageIdentifier)
    {
        var imagePieceList = GetFileNameOfImagePieces(imageIdentifier, username, customSizedImageTiles_ImageIdentifier);

        var pathToUsersCustomSizedImageTiles = GetPathToUsersCustomFolder(username, customSizedImageTiles_ImageIdentifier);
        CheckIfDirectoryExistsAndIfNotCreateIt(username, pathToUsersCustomSizedImageTiles);

        // Calculating the percentage height and width of the image container size
        var percentHeight = (int)Math.Round(height * imageSize);
        var percentWidth = (int)Math.Round(width * imageSize);

        foreach (var sourceFilePath in imagePieceList)
        {
            var imageFileName = Path.GetFileName(sourceFilePath);
            var destinationFilePath = Path.Combine(pathToUsersCustomSizedImageTiles, imageFileName);

            ChangeSizeOfImagePiece(sourceFilePath, destinationFilePath, percentHeight, percentWidth);
        }
    }


    private static void ChangeSizeOfImagePiece(string sourceFilePath, string destinationFilePath, int height, int width)
    {
        using Image<Rgba32> image = Image.Load<Rgba32>(sourceFilePath);

        image.Mutate(x => x.Resize(new Size(width, height)));

        image.Save(destinationFilePath);

        image.Dispose();
    }

    private static List<(int X, int Y)> GetNonTransparentPixelCoordinates(string sourceFilePath)
    {
        List<(int X, int Y)> nonTransparentPixels = [];

        using (Image<Rgba32> image = Image.Load<Rgba32>(sourceFilePath))
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