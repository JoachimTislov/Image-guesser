namespace Image_guesser.Core.Domain.ImageContext.Repositories;

public interface IImageRepository
{
    void AddImageRecord(ImageRecord ImageRecord);
    Task<ImageRecord> GetImageRecordById(string ImageIdentifier);
    Task<int> GetImagePieceCountById(string ImageIdentifier);
    Task<string> GetRandomImageIdentifier();
    Task AddAllMappedImagesToDatabase();
}