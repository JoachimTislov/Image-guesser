using Image_guesser.Core.Domain.SessionContext.ViewModels;

namespace Image_guesser.Core.Domain.ImageContext.Services;

public interface IImageService
{
    Task<ImageRecord> GetImageRecordById(string ImageIdentifier);
    Task<int> GetImagePieceCountById(string ImageIdentifier);
    Task<string> GetRandomImageIdentifier();
    Task<List<ImageRecord>> GetXAmountOfImageRecords(int amount);
    List<string> GetFileNameOfImagePieces(string imageIdentifier);
    List<(string ImageId, List<(int X, int Y)> Coordinates)> GetCoordinatesForImagePieces(List<string> ImagePieceList);
}