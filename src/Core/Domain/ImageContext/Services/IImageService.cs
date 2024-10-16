
namespace Image_guesser.Core.Domain.ImageContext.Services;

public interface IImageService
{
    bool CheckIfUserHasCustomSizedImageTiles(string username, string customSizedImageTiles_ImageIdentifier);
    Task<ImageRecord> GetImageRecordById(string ImageIdentifier);
    Task<int> GetImagePieceCountById(string ImageIdentifier);
    Task<string> GetDifferentRandomImageIdentifier(List<string> imageIdentifiers);
    Task<string> GetRandomImageIdentifier();
    Task<List<ImageRecord>> GetXAmountOfImageRecords(int amount);
    List<string> GetFileNameOfImagePieces(string imageIdentifier, string username, string customSizedImageTiles_ImageIdentifier);
    List<(string ImageId, List<(int X, int Y)> Coordinates)> GetCoordinatesForImagePieces(List<string> ImagePieceList);
    void SetSizeOfImagePieces(string imageIdentifier, int width, int height, double imageSize, string username, string customSizedImageTiles_ImageIdentifier);
}