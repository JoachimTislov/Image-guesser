namespace Image_guesser.Core.Domain.ImageContext.Services;

public interface IImageService
{
    Task<ImageRecord> GetImageRecordById(string ImageIdentifier);
    Task<int> GetImagePieceCountById(string ImageIdentifier);
    List<string> GetFileNameOfImagePieces(string imageIdentifier);
    List<(string ImageId, List<(int X, int Y)> Coordinates)> GetCoordinatesForImagePieces(string imagePiecesFolderPath, List<string> ImagePieceList);
}