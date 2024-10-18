using Image_guesser.Core.Domain.ImageContext.Services;
using Image_guesser.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        if (!string.IsNullOrEmpty(ImageFile))
        {
            var pathToUploaded_Images = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DataSet", "Uploaded_Images");
            NameOfImageWithOutExtension = Path.GetFileNameWithoutExtension(ImageFile);

            ImagePath = Path.Combine(pathToUploaded_Images, User.Identity?.Name!, NameOfImageWithOutExtension, ImageFile).Replace("\\", "/").Split("wwwroot")[1];
        }
    }
}