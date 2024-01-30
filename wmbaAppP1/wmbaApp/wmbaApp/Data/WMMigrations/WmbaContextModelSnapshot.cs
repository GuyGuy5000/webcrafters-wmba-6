﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using wmbaApp.Data;

#nullable disable

namespace wmbaApp.Data.WMMigrations
{
    [DbContext(typeof(WmbaContext))]
    partial class WmbaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.15");

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
                        .HasColumnType("TEXT");

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

            modelBuilder.Entity("wmbaApp.Models.FileProperty", b =>
                {
                    b.Property<int>("FilePropertyID")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Property")
                        .HasColumnType("BLOB");

                    b.HasKey("FilePropertyID");

                    b.ToTable("FileProperty");
                });

            modelBuilder.Entity("wmbaApp.Models.Game", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("GameEndTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("GameLocation")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("GameStartTime")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Games");
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

            modelBuilder.Entity("wmbaApp.Models.Player", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("PlyrDOB")
                        .HasColumnType("TEXT");

                    b.Property<string>("PlyrFirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int?>("PlyrJerseyNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlyrLastName")
                        .HasMaxLength(80)
                        .HasColumnType("TEXT");

                    b.Property<int?>("StatisticID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("StatisticID");

                    b.HasIndex("TeamID");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("wmbaApp.Models.PlayerPhoto", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Content")
                        .HasColumnType("BLOB");

                    b.Property<string>("MimeType")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayerID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("PlayerID")
                        .IsUnique();

                    b.ToTable("PlayerPhotos");
                });

            modelBuilder.Entity("wmbaApp.Models.PlayerPosition", b =>
                {
                    b.Property<int>("PlayerID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PositionID")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlayerID", "PositionID");

                    b.HasIndex("PlayerID")
                        .IsUnique();

                    b.HasIndex("PositionID");

                    b.ToTable("PlayerPositions");
                });

            modelBuilder.Entity("wmbaApp.Models.PlayerThumbnail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Content")
                        .HasColumnType("BLOB");

                    b.Property<string>("MimeType")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayerID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("PlayerID")
                        .IsUnique();

                    b.ToTable("PlayerThumbnails");
                });

            modelBuilder.Entity("wmbaApp.Models.Position", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("PosName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("PosName")
                        .IsUnique();

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("wmbaApp.Models.Statistic", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
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

                    b.Property<string>("TmAbbreviation")
                        .HasMaxLength(3)
                        .HasColumnType("TEXT");

                    b.Property<string>("TmName")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("DivisionID");

                    b.HasIndex("TmAbbreviation")
                        .IsUnique();

                    b.HasIndex("TmName")
                        .IsUnique();

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("wmbaApp.Models.UploadedFile", b =>
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

            modelBuilder.Entity("wmbaApp.Models.FileProperty", b =>
                {
                    b.HasOne("wmbaApp.Models.UploadedFile", "UploadedFile")
                        .WithOne("FileProperty")
                        .HasForeignKey("wmbaApp.Models.FileProperty", "FilePropertyID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UploadedFile");
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

            modelBuilder.Entity("wmbaApp.Models.PlayerPhoto", b =>
                {
                    b.HasOne("wmbaApp.Models.Player", "Player")
                        .WithOne("PlayerPhoto")
                        .HasForeignKey("wmbaApp.Models.PlayerPhoto", "PlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("wmbaApp.Models.PlayerPosition", b =>
                {
                    b.HasOne("wmbaApp.Models.Player", "Player")
                        .WithMany("PlayerPositions")
                        .HasForeignKey("PlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("wmbaApp.Models.Position", "Position")
                        .WithMany("PlayerPositions")
                        .HasForeignKey("PositionID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Position");
                });

            modelBuilder.Entity("wmbaApp.Models.PlayerThumbnail", b =>
                {
                    b.HasOne("wmbaApp.Models.Player", "Player")
                        .WithOne("PlayerThumbnail")
                        .HasForeignKey("wmbaApp.Models.PlayerThumbnail", "PlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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
                });

            modelBuilder.Entity("wmbaApp.Models.Player", b =>
                {
                    b.Navigation("PlayerPhoto");

                    b.Navigation("PlayerPositions");

                    b.Navigation("PlayerThumbnail");
                });

            modelBuilder.Entity("wmbaApp.Models.Position", b =>
                {
                    b.Navigation("PlayerPositions");
                });

            modelBuilder.Entity("wmbaApp.Models.Statistic", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("wmbaApp.Models.Team", b =>
                {
                    b.Navigation("DivisionCoaches");

                    b.Navigation("GameTeams");

                    b.Navigation("Players");
                });

            modelBuilder.Entity("wmbaApp.Models.UploadedFile", b =>
                {
                    b.Navigation("FileProperty");
                });
#pragma warning restore 612, 618
        }
    }
}