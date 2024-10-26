﻿// <auto-generated />
using System;
using Image_guesser.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Image_guesser.Migrations
{
    [DbContext(typeof(ImageGameContext))]
    partial class ImageGameContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("Image_guesser.Core.Domain.GameContext.BaseGame", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BaseOracleId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameMode")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameStatus")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OracleType")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SessionId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SessionId");

                    b.ToTable("Games");

                    b.HasDiscriminator<string>("OracleType").HasValue("BaseGame");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.GameContext.Guess", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("BaseGameId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("GuessMessage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NameOfGuesser")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TimeOfGuess")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BaseGameId");

                    b.ToTable("Guess");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.GameContext.Guesser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("BaseGameId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Guesses")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Points")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WrongGuessCounter")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BaseGameId");

                    b.ToTable("Guessers");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.ImageContext.ImageRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PieceCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ImageRecords");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.LeaderboardContext.LeaderboardEntry", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Oracle")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("GamesPlayed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GamesWon")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OracleType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Score")
                        .HasColumnType("INTEGER");

                    b.HasKey("Name", "Oracle");

                    b.ToTable("LeaderboardEntries");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.OracleContext.AI", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("AI_Type")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("NumbersForImagePieces")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AIs");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.OracleContext.BaseOracle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageIdentifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageTileOrderLog")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("NumberOfTilesRevealed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TotalGuesses")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Oracles");

                    b.HasDiscriminator<string>("Type").HasValue("BaseOracle");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.SessionContext.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ChosenOracleId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("CurrentGameId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SessionHostId")
                        .HasColumnType("TEXT");

                    b.Property<int>("SessionStatus")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TimeOfCreation")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.UserContext.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<int>("Correct_Guesses")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CurrentPageUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomSizedImageTilesDirectoryId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Played_Games")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Points")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("SessionId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("SessionId");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.GameContext.Game<Image_guesser.Core.Domain.OracleContext.AI>", b =>
                {
                    b.HasBaseType("Image_guesser.Core.Domain.GameContext.BaseGame");

                    b.Property<Guid?>("AIOracleId")
                        .HasColumnType("TEXT");

                    b.HasIndex("AIOracleId")
                        .IsUnique();

                    b.HasDiscriminator().HasValue("AI");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.GameContext.Game<Image_guesser.Core.Domain.UserContext.User>", b =>
                {
                    b.HasBaseType("Image_guesser.Core.Domain.GameContext.BaseGame");

                    b.Property<Guid>("UserOracleId")
                        .HasColumnType("TEXT");

                    b.HasIndex("UserOracleId");

                    b.HasDiscriminator().HasValue("User");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.OracleContext.Oracle<Image_guesser.Core.Domain.OracleContext.AI>", b =>
                {
                    b.HasBaseType("Image_guesser.Core.Domain.OracleContext.BaseOracle");

                    b.Property<Guid>("AI_Id")
                        .HasColumnType("TEXT");

                    b.HasIndex("AI_Id")
                        .IsUnique();

                    b.HasDiscriminator().HasValue("AI");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.OracleContext.Oracle<Image_guesser.Core.Domain.UserContext.User>", b =>
                {
                    b.HasBaseType("Image_guesser.Core.Domain.OracleContext.BaseOracle");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasIndex("UserId");

                    b.HasDiscriminator().HasValue("User");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.GameContext.BaseGame", b =>
                {
                    b.HasOne("Image_guesser.Core.Domain.SessionContext.Session", null)
                        .WithMany("Games")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.GameContext.Guess", b =>
                {
                    b.HasOne("Image_guesser.Core.Domain.GameContext.BaseGame", null)
                        .WithMany("GuessLog")
                        .HasForeignKey("BaseGameId");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.GameContext.Guesser", b =>
                {
                    b.HasOne("Image_guesser.Core.Domain.GameContext.BaseGame", null)
                        .WithMany("Guessers")
                        .HasForeignKey("BaseGameId");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.SessionContext.Session", b =>
                {
                    b.OwnsOne("Image_guesser.Core.Domain.SessionContext.Options", "Options", b1 =>
                        {
                            b1.Property<Guid>("SessionId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("AI_Type")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("AmountOfGamesPlayed")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("GameMode")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("ImageIdentifier")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<int>("LobbySize")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("OracleType")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("PictureMode")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("UserOracleMode")
                                .HasColumnType("INTEGER");

                            b1.HasKey("SessionId");

                            b1.ToTable("Sessions");

                            b1.WithOwner()
                                .HasForeignKey("SessionId");
                        });

                    b.Navigation("Options")
                        .IsRequired();
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.UserContext.User", b =>
                {
                    b.HasOne("Image_guesser.Core.Domain.SessionContext.Session", null)
                        .WithMany("SessionUsers")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Image_guesser.Core.Domain.UserContext.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Image_guesser.Core.Domain.UserContext.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Image_guesser.Core.Domain.UserContext.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Image_guesser.Core.Domain.UserContext.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.GameContext.Game<Image_guesser.Core.Domain.OracleContext.AI>", b =>
                {
                    b.HasOne("Image_guesser.Core.Domain.OracleContext.Oracle<Image_guesser.Core.Domain.OracleContext.AI>", "Oracle")
                        .WithOne()
                        .HasForeignKey("Image_guesser.Core.Domain.GameContext.Game<Image_guesser.Core.Domain.OracleContext.AI>", "AIOracleId")
                        .HasConstraintName("FK_Oracle_AI");

                    b.Navigation("Oracle");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.GameContext.Game<Image_guesser.Core.Domain.UserContext.User>", b =>
                {
                    b.HasOne("Image_guesser.Core.Domain.OracleContext.Oracle<Image_guesser.Core.Domain.UserContext.User>", "Oracle")
                        .WithMany()
                        .HasForeignKey("UserOracleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Oracle_User");

                    b.Navigation("Oracle");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.OracleContext.Oracle<Image_guesser.Core.Domain.OracleContext.AI>", b =>
                {
                    b.HasOne("Image_guesser.Core.Domain.OracleContext.AI", "Entity")
                        .WithOne()
                        .HasForeignKey("Image_guesser.Core.Domain.OracleContext.Oracle<Image_guesser.Core.Domain.OracleContext.AI>", "AI_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_AI");

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.OracleContext.Oracle<Image_guesser.Core.Domain.UserContext.User>", b =>
                {
                    b.HasOne("Image_guesser.Core.Domain.UserContext.User", "Entity")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.GameContext.BaseGame", b =>
                {
                    b.Navigation("GuessLog");

                    b.Navigation("Guessers");
                });

            modelBuilder.Entity("Image_guesser.Core.Domain.SessionContext.Session", b =>
                {
                    b.Navigation("Games");

                    b.Navigation("SessionUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
