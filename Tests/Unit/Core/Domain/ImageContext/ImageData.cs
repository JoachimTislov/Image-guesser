using Image_guesser.Core.Domain.ImageContext;

namespace Tests.Unit.Core.Domain.ImageContext;

public class ImageDataTests
{

    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var imageData = new ImageData();

        Assert.Equal(Guid.Empty, imageData.Id);
        Assert.Equal(string.Empty, imageData.Name);
        Assert.Equal(string.Empty, imageData.Identifier);
        Assert.Equal(string.Empty, imageData.Link);
        Assert.Equal(string.Empty, imageData.FolderWithImagePiecesLink);
        Assert.Equal(0, imageData.PieceCount);
    }

    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        var name = "ImageTest";
        var identifier = "ImageID";
        var link = "ImageLink";
        var folderWithImagePiecesLink = "FolderLink";
        var pieceCount = 1;

        var ImageData = new ImageData(name, identifier, link, folderWithImagePiecesLink, pieceCount);

        Assert.Equal(name, ImageData.Name);
        Assert.Equal(identifier, ImageData.Identifier);
        Assert.Equal(link, ImageData.Link);
        Assert.Equal(folderWithImagePiecesLink, ImageData.FolderWithImagePiecesLink);
        Assert.Equal(pieceCount, ImageData.PieceCount);
    }

}