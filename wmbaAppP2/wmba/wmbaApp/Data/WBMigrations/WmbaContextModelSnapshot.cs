﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using wmbaApp.Data;

#nullable disable

namespace wmbaApp.Data.WBMigrations
{
    [DbContext(typeof(WmbaContext))]
    partial class WmbaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("NOCASE")
                .HasAnnotation("ProductVersion", "7.0.17");

            modelBuilder.Entity("wmbaApp.Models.Coach", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CoachEmail")
                        .HasColumnType("TEXT");

                    b.Property<string>("CoachFirstName")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("CoachLastName")
                        .HasMaxLength(80)
                        .HasColumnType("TEXT");

                    b.Property<string>("CoachPhone")
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Coaches");
                });

            modelBuilder.Entity("wmbaApp.Models.Division", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DivName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .UseCollation("NOCASE");

                    b.HasKey("ID");

                    b.HasIndex("DivName")
                        .IsUnique();

                    b.ToTable("Divisions");
                });

            modelBuilder.Entity("wmbaApp.Models.DivisionCoach", b =>
                {
                    b.Property<int>("DivisionID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CoachID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamID")
                        .HasColumnType("INTEGER");

                    b.HasKey("DivisionID", "CoachID");

                    b.HasIndex("TeamID");

                    b.HasIndex("CoachID", "DivisionID")
                        .IsUnique();

                    b.ToTable("DivisionCoaches");
                });

            modelBuilder.Entity("wmbaApp.Models.Game", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AwayLineupID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AwayTeamID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AwayTeamScore")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentInning")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DivisionID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("GameEndTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("GameLocationID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("GameStartTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("HasStarted")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("HomeLineupID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HomeTeamID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HomeTeamScore")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("AwayLineupID");

                    b.HasIndex("AwayTeamID");

                    b.HasIndex("DivisionID");

                    b.HasIndex("GameLocationID");

                    b.HasIndex("HomeLineupID");

                    b.HasIndex("HomeTeamID");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("wmbaApp.Models.GameLocation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("GameLocations");
                });

            modelBuilder.Entity("wmbaApp.Models.GameTeam", b =>
                {
                    b.Property<int>("TeamID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GmtmLineup")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.Property<int?>("GmtmScore")
                        .HasColumnType("INTEGER");

                    b.HasKey("TeamID", "GameID");

                    b.HasIndex("GameID");

                    b.HasIndex("TeamID", "GameID")
                        .IsUnique();

                    b.ToTable("GameTeams");
                });

            modelBuilder.Entity("wmbaApp.Models.Inning", b =>
                {
                    b.Property<int>("InningID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AwayTeamScore")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HomeTeamScore")
                        .HasColumnType("INTEGER");

                    b.Property<int>("gameID")
                        .HasColumnType("INTEGER");

                    b.HasKey("InningID");

                    b.HasIndex("gameID");

                    b.ToTable("Innings");
                });

            modelBuilder.Entity("wmbaApp.Models.Lineup", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Lineups");
                });

            modelBuilder.Entity("wmbaApp.Models.PlayByPlay", b =>
                {
                    b.Property<int>("PlayByPlayID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("InningID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerActionID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerID")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlayByPlayID");

                    b.HasIndex("InningID");

                    b.HasIndex("PlayerActionID");

                    b.HasIndex("PlayerID");

                    b.ToTable("PlayByPlays");
                });

            modelBuilder.Entity("wmbaApp.Models.Player", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlyrFirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int?>("PlyrJerseyNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlyrLastName")
                        .HasMaxLength(80)
                        .HasColumnType("TEXT");

                    b.Property<string>("PlyrMemberID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("StatisticID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("PlyrMemberID")
                        .IsUnique();

                    b.HasIndex("StatisticID");

                    b.HasIndex("TeamID");

                    b.HasIndex("PlyrJerseyNumber", "TeamID")
                        .IsUnique();

                    b.ToTable("Players");
                });

            modelBuilder.Entity("wmbaApp.Models.PlayerAction", b =>
                {
                    b.Property<int>("PlayerActionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlayerActionName")
                        .HasColumnType("TEXT");

                    b.HasKey("PlayerActionID");

                    b.HasIndex("PlayerActionName")
                        .IsUnique();

                    b.ToTable("PlayerActions");
                });

            modelBuilder.Entity("wmbaApp.Models.PlayerLineup", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("LineupID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PlayerID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("LineupID");

                    b.HasIndex("PlayerID", "LineupID")
                        .IsUnique();

                    b.ToTable("PlayerLineup");
                });

            modelBuilder.Entity("wmbaApp.Models.Statistic", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rating")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("StatsAB")
                        .HasColumnType("INTEGER");

                    b.Property<double?>("StatsAVG")
                        .HasColumnType("REAL");

                    b.Property<int?>("StatsBB")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("StatsGP")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("StatsH")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("StatsHR")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("StatsK")
                        .HasColumnType("INTEGER");

                    b.Property<double?>("StatsOBP")
                        .HasColumnType("REAL");

                    b.Property<double?>("StatsOPS")
                        .HasColumnType("REAL");

                    b.Property<int?>("StatsPA")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("StatsR")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("StatsRBI")
                        .HasColumnType("INTEGER");

                    b.Property<double?>("StatsSLG")
                        .HasColumnType("REAL");

                    b.HasKey("ID");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("wmbaApp.Models.Team", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DivisionID")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TmName")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("TEXT")
                        .UseCollation("NOCASE");

                    b.HasKey("ID");

                    b.HasIndex("DivisionID");

                    b.HasIndex("TmName")
                        .IsUnique();

                    b.ToTable("Team");
                });

            modelBuilder.Entity("wmbaApp.ViewModels.FileProperty", b =>
                {
                    b.Property<int>("FilePropertyID")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Property")
                        .HasColumnType("BLOB");

                    b.HasKey("FilePropertyID");

                    b.ToTable("FileProperty");
                });

            modelBuilder.Entity("wmbaApp.ViewModels.UploadedFile", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileName")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("MimeType")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("UploadedFiles");
                });

            modelBuilder.Entity("wmbaApp.Models.DivisionCoach", b =>
                {
                    b.HasOne("wmbaApp.Models.Coach", "Coach")
                        .WithMany("DivisionCoaches")
                        .HasForeignKey("CoachID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("wmbaApp.Models.Division", "Division")
                        .WithMany("DivisionCoaches")
                        .HasForeignKey("DivisionID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("wmbaApp.Models.Team", "Team")
                        .WithMany("DivisionCoaches")
                        .HasForeignKey("TeamID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Coach");

                    b.Navigation("Division");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("wmbaApp.Models.Game", b =>
                {
                    b.HasOne("wmbaApp.Models.Lineup", "AwayLineup")
                        .WithMany("AwayGames")
                        .HasForeignKey("AwayLineupID");

                    b.HasOne("wmbaApp.Models.Team", "AwayTeam")
                        .WithMany("AwayGames")
                        .HasForeignKey("AwayTeamID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("wmbaApp.Models.Division", "Division")
                        .WithMany()
                        .HasForeignKey("DivisionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("wmbaApp.Models.GameLocation", "GameLocation")
                        .WithMany("Games")
                        .HasForeignKey("GameLocationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("wmbaApp.Models.Lineup", "HomeLineup")
                        .WithMany("HomeGames")
                        .HasForeignKey("HomeLineupID");

                    b.HasOne("wmbaApp.Models.Team", "HomeTeam")
                        .WithMany("HomeGames")
                        .HasForeignKey("HomeTeamID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AwayLineup");

                    b.Navigation("AwayTeam");

                    b.Navigation("Division");

                    b.Navigation("GameLocation");

                    b.Navigation("HomeLineup");

                    b.Navigation("HomeTeam");
                });

            modelBuilder.Entity("wmbaApp.Models.GameTeam", b =>
                {
                    b.HasOne("wmbaApp.Models.Game", "Game")
                        .WithMany("GameTeams")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("wmbaApp.Models.Team", "Team")
                        .WithMany("GameTeams")
                        .HasForeignKey("TeamID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("wmbaApp.Models.Inning", b =>
                {
                    b.HasOne("wmbaApp.Models.Game", "Game")
                        .WithMany("Innings")
                        .HasForeignKey("gameID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("wmbaApp.Models.PlayByPlay", b =>
                {
                    b.HasOne("wmbaApp.Models.Inning", "Inning")
                        .WithMany("PlayByPlays")
                        .HasForeignKey("InningID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("wmbaApp.Models.PlayerAction", "PlayerAction")
                        .WithMany("PlayByPlays")
                        .HasForeignKey("PlayerActionID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("wmbaApp.Models.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inning");

                    b.Navigation("Player");

                    b.Navigation("PlayerAction");
                });

            modelBuilder.Entity("wmbaApp.Models.Player", b =>
                {
                    b.HasOne("wmbaApp.Models.Statistic", "Statistics")
                        .WithMany("Players")
                        .HasForeignKey("StatisticID");

                    b.HasOne("wmbaApp.Models.Team", "Team")
                        .WithMany("Players")
                        .HasForeignKey("TeamID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Statistics");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("wmbaApp.Models.PlayerLineup", b =>
                {
                    b.HasOne("wmbaApp.Models.Lineup", "Lineup")
                        .WithMany("PlayerLineups")
                        .HasForeignKey("LineupID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("wmbaApp.Models.Player", "Player")
                        .WithMany("PlayerLineups")
                        .HasForeignKey("PlayerID");

                    b.Navigation("Lineup");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("wmbaApp.Models.Team", b =>
                {
                    b.HasOne("wmbaApp.Models.Division", "Division")
                        .WithMany("Teams")
                        .HasForeignKey("DivisionID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Division");
                });

            modelBuilder.Entity("wmbaApp.ViewModels.FileProperty", b =>
                {
                    b.HasOne("wmbaApp.ViewModels.UploadedFile", "UploadedFile")
                        .WithOne("FileProperty")
                        .HasForeignKey("wmbaApp.ViewModels.FileProperty", "FilePropertyID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UploadedFile");
                });

            modelBuilder.Entity("wmbaApp.Models.Coach", b =>
                {
                    b.Navigation("DivisionCoaches");
                });

            modelBuilder.Entity("wmbaApp.Models.Division", b =>
                {
                    b.Navigation("DivisionCoaches");

                    b.Navigation("Teams");
                });

            modelBuilder.Entity("wmbaApp.Models.Game", b =>
                {
                    b.Navigation("GameTeams");

                    b.Navigation("Innings");
                });

            modelBuilder.Entity("wmbaApp.Models.GameLocation", b =>
                {
                    b.Navigation("Games");
                });

            modelBuilder.Entity("wmbaApp.Models.Inning", b =>
                {
                    b.Navigation("PlayByPlays");
                });

            modelBuilder.Entity("wmbaApp.Models.Lineup", b =>
                {
                    b.Navigation("AwayGames");

                    b.Navigation("HomeGames");

                    b.Navigation("PlayerLineups");
                });

            modelBuilder.Entity("wmbaApp.Models.Player", b =>
                {
                    b.Navigation("PlayerLineups");
                });

            modelBuilder.Entity("wmbaApp.Models.PlayerAction", b =>
                {
                    b.Navigation("PlayByPlays");
                });

            modelBuilder.Entity("wmbaApp.Models.Statistic", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("wmbaApp.Models.Team", b =>
                {
                    b.Navigation("AwayGames");

                    b.Navigation("DivisionCoaches");

                    b.Navigation("GameTeams");

                    b.Navigation("HomeGames");

                    b.Navigation("Players");
                });

            modelBuilder.Entity("wmbaApp.ViewModels.UploadedFile", b =>
                {
                    b.Navigation("FileProperty");
                });
#pragma warning restore 612, 618
        }
    }
}
