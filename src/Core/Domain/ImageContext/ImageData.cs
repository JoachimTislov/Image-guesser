using Image_guesser.SharedKernel;

namespace Image_guesser.Core.Domain.ImageContext;

public class ImageRecord : BaseEntity
{
    public ImageRecord() { }

    public ImageRecord(string name, string identifier, string link, string folderWithImagePiecesLink, int pieceCount)
    {
        Name = name;
        Identifier = identifier;
        Link = link;
        FolderWithImagePiecesLink = folderWithImagePiecesLink;
        PieceCount = pieceCount;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string FolderWithImagePiecesLink { get; set; } = string.Empty;
    public int PieceCount { get; set; }
}