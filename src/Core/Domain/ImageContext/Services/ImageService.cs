using Image_guesser.Core.Domain.ImageContext.Repository;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Image_guesser.Core.Domain.ImageContext.Services;

public class ImageService(IWebHostEnvironment hostingEnvironment, IImageRepository imageRepository) : IImageService
{
    private readonly IWebHostEnvironment _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
    private readonly IImageRepository _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));

    private static void MakeLinksCompatibleWithJavascript(ImageRecord imageRecord)
    {
        imageRecord.Link = MakeLinkCompatibleWithJavascript(imageRecord.Link);
        imageRecord.FolderWithImagePiecesLink = MakeLinkCompatibleWithJavascript(imageRecord.FolderWithImagePiecesLink);
    }

    public async Task<ImageRecord> GetImageRecordById(string ImageIdentifier)
    {
        ImageRecord ImageRecord = await _imageRepository.GetImageRecordById(ImageIdentifier);

        MakeLinksCompatibleWithJavascript(ImageRecord);

        return ImageRecord;
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

        foreach (var record in imageRecords)
        {
            MakeLinksCompatibleWithJavascript(record);
        }

        return imageRecords;
    }

    private static string MakeLinkCompatibleWithJavascript(string Link)
    {
        return Link.Replace("\\", Path.DirectorySeparatorChar.ToString());
    }

    public async Task<List<string>> GetFileNameOfImagePieces(string imageIdentifier)
    {
        var ImageRecord = await GetImageRecordById(imageIdentifier);
        var imagePieceFileNames = Directory.GetFiles(Path.Combine("wwwroot", ImageRecord.FolderWithImagePiecesLink)).ToList();

        //Filter out all files that are not images
        //This is needed because the folder contains a .DS_Store file
        List<string> validExtensions = [".png"];
        List<string> filteredFileNames = imagePieceFileNames
            .Where(fileName => validExtensions.Contains(Path.GetExtension(fileName).ToLower())).ToList();

        return filteredFileNames;
    }

    public List<(string ImageId, List<(int X, int Y)> Coordinates)> GetCoordinatesForImagePieces(List<string> ImagePieceList)
    {
        List<(string ImageId, List<(int X, int Y)> Coordinates)> _imageCoordinates = [];

        foreach (var imageName in ImagePieceList)
        {
            var relativeImagePath = GetRelativeImagePath(imageName);

            _imageCoordinates.Add((imageName, GetNonTransparentPixelCoordinates(relativeImagePath)));
        }

        return _imageCoordinates;
    }

    private string GetRelativeImagePath(string imageName) => Path.Combine(_hostingEnvironment.WebRootPath, imageName.Replace($"wwwroot{Path.DirectorySeparatorChar}", ""));

    public async Task SetSizeOfImagePieces(string imageIdentifier, int width, int height, double imageSize)
    {
        var imagePieceList = await GetFileNameOfImagePieces(imageIdentifier);

        // Calculating the percentage height and width of the image container size
        var percentHeight = (int)Math.Round(height * imageSize);
        var percentWidth = (int)Math.Round(width * imageSize);

        foreach (var imageName in imagePieceList)
        {
            var relativeImagePath = GetRelativeImagePath(imageName);

            ChangeSizeOfImagePiece(relativeImagePath, percentHeight, percentWidth);
        }
    }


    private static void ChangeSizeOfImagePiece(string imageName, int height, int width)
    {
        using Image<Rgba32> image = Image.Load<Rgba32>(imageName);

        Console.WriteLine($"Resizing image: {imageName} to width: {width} and height: {height}");

        image.Mutate(x => x.Resize(new Size(width, height)));

        image.Save(imageName);

        image.Dispose();
    }

    private static List<(int X, int Y)> GetNonTransparentPixelCoordinates(string imageName)
    {
        List<(int X, int Y)> nonTransparentPixels = [];

        using (Image<Rgba32> image = Image.Load<Rgba32>(imageName))
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