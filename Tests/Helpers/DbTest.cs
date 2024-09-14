using Image_guesser.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Tests.Helpers;

public class DbTest
{
    private readonly ImageGameContext _context;
    private readonly DbContextOptions<ImageGameContext> _contextOptions;
    private readonly SqliteConnection _connection;
    private readonly ITestOutputHelper _output;
    public DbTest(ITestOutputHelper output)
    {
        _output = output;

        _connection = CreateInMemoryDatabase();

        _contextOptions = new DbContextOptionsBuilder<ImageGameContext>()
                .UseSqlite(_connection)
                .LogTo(_output.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging()
                .Options;

        _context = new ImageGameContext(_contextOptions);
    }

    public DbContextOptions<ImageGameContext> ContextOptions => _contextOptions;
    public ImageGameContext Context => _context;

    private static SqliteConnection CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        return connection;
    }

    public void Dispose() => _connection.Dispose();
}