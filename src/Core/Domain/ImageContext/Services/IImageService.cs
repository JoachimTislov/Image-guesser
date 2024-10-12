
namespace Image_guesser.Core.Domain.ImageContext.Services;

public interface IImageService
{
    Task<ImageRecord> GetImageRecordById(string ImageIdentifier);
    Task<int> GetImagePieceCountById(string ImageIdentifier);
    Task<string> GetDifferentRandomImageIdentifier(List<string> imageIdentifiers);
    Task<string> GetRandomImageIdentifier();
    Task<List<ImageRecord>> GetXAmountOfImageRecords(int amount);
    Task<List<string>> GetFileNameOfImagePieces(string imageIdentifier);
    List<(string ImageId, List<(int X, int Y)> Coordinates)> GetCoordinatesForImagePieces(List<string> ImagePieceList);
    Task SetSizeOfImagePieces(string imageIdentifier, int width, int height, double imageSize);
}