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
        PieceCount = pieceCount;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Identifier { get; private set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public int PieceCount { get; private set; }
}