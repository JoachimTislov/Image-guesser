using Image_guesser.Core.Domain.GameContext;
using Image_guesser.Core.Domain.ImageContext;
using Image_guesser.Core.Domain.OracleContext;
using Image_guesser.Core.Domain.SessionContext;
using Image_guesser.Core.Domain.UserContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Image_guesser.Infrastructure;

public class ImageGameContext(DbContextOptions<ImageGameContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<AI> AIs { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<BaseGame> Games { get; set; }
    public DbSet<BaseOracle> Oracles { get; set; }
    public DbSet<Guesser> Guessers { get; set; }
    public DbSet<ImageRecord> ImageRecords { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Creates a TPH type hierarchy for the Games table
        modelBuilder.Entity<BaseGame>()
            .HasDiscriminator<string>("Oracle")
            .HasValue<Game<User>>("User")
            .HasValue<Game<AI>>("AI");

        // Creates a TPH type hierarchy for the Oracles table
        modelBuilder.Entity<BaseOracle>()
            .HasDiscriminator<string>("Oracle")
            .HasValue<Oracle<User>>("User")
            .HasValue<Oracle<AI>>("AI");

        // Configure the foreign key for Oracle<User>
        modelBuilder.Entity<Oracle<User>>()
            .HasOne(o => o.Entity)
            .WithMany()
            .HasForeignKey("UserId")
            .HasConstraintName("FK_Oracle_User")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the foreign key for Oracle<AI>
        modelBuilder.Entity<Oracle<AI>>()
            .HasOne(o => o.Entity)
            .WithMany()
            .HasForeignKey("AI_Id")
            .HasConstraintName("FK_Oracle_AI")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
