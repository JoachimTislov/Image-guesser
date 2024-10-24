using System.Text.Json;
using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Image_guesser.Pages;

[RequireLogin]
public class SliceImageModel(ILogger<SliceImageModel> logger, IImageService imageService) : PageModel
{
    private readonly ILogger<SliceImageModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IImageService _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));

    [BindProperty(SupportsGet = true)]
    public string? ImageFile { get; set; }

    public string NameOfImageWithOutExtension { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    public void OnGet()
    {
        ConfigureImagePath();
    }

    private void ConfigureImagePath()
    {
        if (!string.IsNullOrEmpty(ImageFile))
        {
            var pathToUploaded_Images = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet", "Uploaded_Images");
            NameOfImageWithOutExtension = Path.GetFileNameWithoutExtension(ImageFile);

            ImagePath = Path.Combine(pathToUploaded_Images, User.Identity?.Name!, NameOfImageWithOutExtension, ImageFile).Replace("\\", "/").Split("wwwroot")[1];
        }
    }

    public void OnPostSliceImage(string stringifiedPoints)
    {
        if (string.IsNullOrEmpty(ImageFile))
        {
            return;
        }

        ConfigureImagePath();

        var DataSetPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet");

        var pathToImage = Path.Combine(DataSetPath, "Uploaded_Images", User.Identity?.Name!, NameOfImageWithOutExtension, ImageFile);
        var outPutFolder = Path.Combine(DataSetPath, "Sliced_Images", User.Identity?.Name!, NameOfImageWithOutExtension, "Tiles");

        if (string.IsNullOrEmpty(stringifiedPoints))
        {
            _logger.LogWarning("No points were provided to slice the image");
            return;
        }

        List<int?>? points = JsonSerializer.Deserialize<List<int?>>(stringifiedPoints);

        if (points == null)
        {
            _logger.LogWarning("Failed to deserialize points");
            return;
        }
        var polygons = new List<List<Point>>();
        var horizontalOffSetRecord = new Dictionary<int, int>();
        var verticalOffSetRecord = new Dictionary<int, int>();

        Image<Rgba32> image = Image.Load<Rgba32>(pathToImage);

        // var minimumTileSize = 100;
        var pixelCount = 0;

        var startX = 0;
        var startY = 0;
        do
        {
            var tile_Points = new List<Point>();

            bool tileComplete = false;

            do
            {
                var x = startX;
                var y = startY;

                verticalOffSetRecord.TryGetValue(y, out int PreviousVerticalValue);
                horizontalOffSetRecord.TryGetValue(x, out int PreviousHorizontalValue);

                var verticalIndex = x + PreviousVerticalValue;
                var horizontalIndex = y + PreviousHorizontalValue;

                tile_Points.Add(new Point(verticalIndex, horizontalIndex)); pixelCount++;

                horizontalIndex++;
                verticalIndex++;


                if (horizontalIndex == image.Width)
                {
                    y = 0;
                    x++;
                }

                if (tile_Points.Count > 0 && startX == x && startY == y)
                {
                    tileComplete = true;
                }

            } while (!tileComplete || image.Height * image.Width > pixelCount); // Safeguard to prevent infinite loop if the image size is equal to the pixel count

            //pixelCount += tile_Points.Count;
            polygons.Add(tile_Points);

        } while (image.Height * image.Width > pixelCount);
    }
}