using Image_guesser.Core.Domain.ImageContext;

namespace Tests.Unit.Core.Domain.ImageContext;

public class ImageRecordTests
{

    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var ImageRecord = new ImageRecord();

        Assert.Equal(Guid.Empty, ImageRecord.Id);
        Assert.Equal(string.Empty, ImageRecord.Name);
        Assert.Equal(string.Empty, ImageRecord.Identifier);
        Assert.Equal(string.Empty, ImageRecord.Link);
        Assert.Equal(0, ImageRecord.PieceCount);
    }

    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        var name = "ImageTest";
        var identifier = "ImageID";
        var link = "ImageLink";
        var folderWithImagePiecesLink = "FolderLink";
        var pieceCount = 1;

        var ImageRecord = new ImageRecord(name, identifier, link, folderWithImagePiecesLink, pieceCount);

        Assert.Equal(name, ImageRecord.Name);
        Assert.Equal(identifier, ImageRecord.Identifier);
        Assert.Equal(link, ImageRecord.Link);
        Assert.Equal(pieceCount, ImageRecord.PieceCount);
    }

}