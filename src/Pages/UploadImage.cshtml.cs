using Image_guesser.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Image_guesser.Pages;

[RequireLogin]
public class UploadImageModel(ILogger<UploadImageModel> logger) : PageModel
{
    private readonly ILogger<UploadImageModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public List<string> ImagesUploadedByUser { get; set; } = [];

    public void OnGet()
    {
        LoadImagesUploadedByUser();
    }

    private void LoadImagesUploadedByUser()
    {
        var pathToImageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet", "Uploaded_Images", User.Identity?.Name!);
        if (Directory.Exists(pathToImageFolder))
        {
            var imageDirectories = Directory.GetDirectories(pathToImageFolder);
            foreach (var imageDirectory in imageDirectories)
            {
                var image = Directory.GetFiles(imageDirectory);
                ImagesUploadedByUser.Add(image[0]);
            }
        }
    }

    public async Task<IActionResult> OnPostUploadImageAsync(IFormFile imageFile)
    {
        if (!ModelState.IsValid)
        {
            LoadImagesUploadedByUser();
            return Page();
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            var validExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(imageFile.FileName);

            if (!validExtensions.Contains(extension.ToLower()))
            {
                ModelState.AddModelError("ImageFile", "Please select a valid image file");
                LoadImagesUploadedByUser();
                return Page();
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName);
            var pathToImageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet", "Uploaded_Images", User.Identity?.Name!, fileNameWithoutExtension);

            if (!Directory.Exists(pathToImageFolder))
            {
                Directory.CreateDirectory(pathToImageFolder);
            }

            var filePath = Path.Combine(pathToImageFolder, imageFile.FileName);

            using var stream = new FileStream(filePath, FileMode.Create);

            await imageFile.CopyToAsync(stream);

            ViewData["Message"] = "Image uploaded successfully!";
            _logger.LogInformation("User uploaded an image, file: {ImageFile}", imageFile);
        }
        else
        {
            ModelState.AddModelError("ImageFile", "Please select an image file.");
        }

        LoadImagesUploadedByUser();

        return Page();
    }

    public void OnPostSliceImage(string relativeImagePath, int tileHeight, int tileWidth)
    {
        // Uhm getting the path directly is buggy.. idk why
        var pathToFolderWithImageUploadedByUser = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet", "Uploaded_Images", User.Identity?.Name!, Path.GetFileNameWithoutExtension(relativeImagePath));
        var imagePath = Path.Combine(pathToFolderWithImageUploadedByUser, Path.GetFileName(relativeImagePath));
        var pathToImageTilesFolder = Path.Combine(pathToFolderWithImageUploadedByUser, "Tiles");
        if (Directory.Exists(pathToImageTilesFolder))
        {
            Directory.Delete(pathToImageTilesFolder, true);
        }

        Directory.CreateDirectory(pathToImageTilesFolder);

        using Image<Rgba32> image = Image.Load<Rgba32>(imagePath);

        var horizontalTiles = image.Height / tileHeight;
        var verticalTiles = image.Width / tileWidth;

        for (var x = 0; x < horizontalTiles; x++)
        {
            for (var y = 0; y < verticalTiles; y++)
            {
                Rectangle rectangleTile = new(x * tileWidth, y * tileHeight, tileWidth, tileHeight);

                using Image<Rgba32> imageTile = image.Clone(x => x.Crop(rectangleTile));

                imageTile.Save(Path.Combine(pathToImageTilesFolder, $"{x + y}.png"));
            }
        }

        ViewData["Message"] = "Image sliced into tiles successfully!";
        _logger.LogInformation("User sliced an image into, file: {imagePath}", imagePath);

        LoadImagesUploadedByUser();
    }
}