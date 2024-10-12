namespace Image_guesser.Core.Domain.ImageContext.Repository;

public interface IImageRepository
{
    void AddImageRecord(ImageRecord ImageRecord);
    Task<ImageRecord> GetImageRecordById(string ImageIdentifier);
    List<ImageRecord> GetImageRecordsByIds(List<string> ImageIdentifiers);
    Task<int> GetImagePieceCountById(string ImageIdentifier);
    Task<string> GetRandomImageIdentifier();
    Task CreateImageDictionary();
    Task AddAllMappedImagesToDatabase();
}