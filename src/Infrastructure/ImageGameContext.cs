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
        // Prevents foreign key errors when deleting a session
        modelBuilder.Entity<Session>()
            .HasMany(s => s.SessionUsers)
            .WithOne()
            .HasForeignKey(u => u.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevent duplicate foreign keys for game table
        modelBuilder.Entity<BaseGame>()
            .HasMany(bg => bg.Guessers)
            .WithOne()
            .HasForeignKey(g => g.GameId);

        // Creates a TPH type hierarchy for the BaseGame table
        modelBuilder.Entity<BaseGame>()
            .HasDiscriminator<string>("OracleType")
            .HasValue<Game<User>>("User")
            .HasValue<Game<AI>>("AI");

        // Configure the foreign key for Game<User>
        modelBuilder.Entity<Game<User>>()
            .HasOne(o => o.Oracle)
            .WithMany()
            .HasForeignKey("UserOracleId")
            .HasConstraintName("FK_Oracle_User")
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the foreign key for Game<AI>
        modelBuilder.Entity<Game<AI>>()
            .HasOne(o => o.Oracle)
            .WithOne()
            .HasForeignKey<Game<AI>>("AIOracleId")
            .HasConstraintName("FK_Oracle_AI")
            .IsRequired(false) // Since to delete an AI, we must let the Oracle be null
            .OnDelete(DeleteBehavior.Restrict);

        // Creates a TPH type hierarchy for the BaseOracle table
        modelBuilder.Entity<BaseOracle>()
            .HasDiscriminator<string>("Type")
            .HasValue<Oracle<User>>("User")
            .HasValue<Oracle<AI>>("AI");

        // Configure the foreign key for Oracle<User>
        modelBuilder.Entity<Oracle<User>>()
            .HasOne(o => o.Entity)
            .WithMany()
            .HasForeignKey("UserId")
            .HasConstraintName("FK_User")
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the foreign key for Oracle<AI>
        modelBuilder.Entity<Oracle<AI>>()
            .HasOne(o => o.Entity)
            .WithOne()
            .HasForeignKey<Oracle<AI>>("AI_Id")
            .HasConstraintName("FK_AI")
            .OnDelete(DeleteBehavior.Cascade);


        base.OnModelCreating(modelBuilder);
    }
}
