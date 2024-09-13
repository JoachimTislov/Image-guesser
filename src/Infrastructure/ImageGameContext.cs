using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.ImageContext;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Image_guesser.Infrastructure;

public class ImageGameContext(DbContextOptions<ImageGameContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<Guesser> Guessers { get; set; } = null!;
    public DbSet<BaseOracle> Oracles { get; set; } = null!;
    public DbSet<ImageData> ImageRecords { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Creates a TPH type hierarchy for the Oracle table
        modelBuilder.Entity<BaseOracle>()
            .HasDiscriminator<string>("OracleType")
            .HasValue<GenericOracle<User>>("User")
            .HasValue<GenericOracle<RandomNumbersAI>>("RandomNumbersAI");

        //Json converter for the different types of Oracles
        //Needed to store the data in the Oracle table
        modelBuilder.Entity<GenericOracle<RandomNumbersAI>>()
        .Property(o => o.Oracle)
        .HasColumnName("RandomNumbersAI")
        .IsRequired(false)
        .HasConversion(
            v => JsonConvert.SerializeObject(v.NumbersForImagePieces),
            v => new RandomNumbersAI()
            {
                NumbersForImagePieces = JsonConvert.DeserializeObject<int[]>(v)
            });

        modelBuilder.Entity<GenericOracle<User>>()
            .Property(o => o.Oracle)
            .IsRequired(false)
            .HasColumnName("UserInfo")
            .HasConversion(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<User>(v) ?? new User());

        base.OnModelCreating(modelBuilder);
    }
}
