using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using wmbaApp.Models;
using wmbaApp.ViewModels;

namespace wmbaApp.Data
{
    /// <summary>
    /// Context class for code first DB approach
    /// - Nadav Hilu, Emmanuel James
    /// </summary>
    public class WmbaContext : DbContext
    {
        public WmbaContext(DbContextOptions<WmbaContext> options) : base(options)
        { 
        }

        public DbSet<Division> Divisions { get; set; }
        public DbSet<DivisionCoach> DivisionCoaches { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<GameTeam> GameTeams { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<GameLocation> GameLocations { get; set; }
        public DbSet<Statistic> Statistics { get; set; }

        public DbSet<Team> HomeTeams { get; set; }
        public DbSet<Team> AwayTeams { get; set; }

        public DbSet<Lineup> Lineups { get; set; }

        public DbSet<PlayerLineup> PlayerLineup { get; set; }

        public DbSet<UploadedFile> UploadedFiles { get; set; }

        public DbSet<PlayByPlay> PlayByPlays { get; set; }
        public DbSet<Inning> Innings { get; set; }
        public DbSet<PlayerAction> PlayerActions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //include collation for case-insensitive unique constraints
            modelBuilder.UseCollation("NOCASE");

            //set case-insensitive collation for division names so that unique constraints
            modelBuilder.Entity<Division>()
                .Property(d => d.DivName)
                .UseCollation("NOCASE");

            modelBuilder.Entity<Team>()
                .Property(t => t.TmName)
                .UseCollation("NOCASE");

            //Unique constraint for division names
            modelBuilder.Entity<Division>()
                .HasIndex(d => d.DivName)
                .IsUnique();

            //Unique constraint for team names
            modelBuilder.Entity<Team>()
                .HasIndex(t => t.TmName)
                .IsUnique();

            //Unique constraint for player member ID
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.PlyrMemberID)
                .IsUnique();


            //Unique constraint for PlayerActionName
            modelBuilder.Entity<PlayerAction>()
                .HasIndex(pa => pa.PlayerActionName)
                .IsUnique();

            //Many to many junction table
            modelBuilder.Entity<DivisionCoach>()
                .HasKey(dc => new { dc.DivisionID, dc.CoachID });

            //Many to many junction table
            modelBuilder.Entity<GameTeam>()
                .HasKey(gt => new { gt.TeamID, gt.GameID });

            //Prevent cascade delete from division to teams
            modelBuilder.Entity<Division>()
                .HasMany<Team>(d => d.Teams)
                .WithOne(t => t.Division)
                .HasForeignKey(t => t.DivisionID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent cascade delete from division to teams
            modelBuilder.Entity<Team>()
               .HasMany(t => t.Players)
               .WithOne(p => p.Team)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Team>()
              .HasMany(t => t.DivisionCoaches)
              .WithOne(dc => dc.Team)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Team>()
    .HasMany(t => t.GameTeams)
    .WithOne(dc => dc.Team)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Team>()
    .HasMany(t => t.HomeGames)
    .WithOne(g => g.HomeTeam)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.AwayGames)
                .WithOne(g => g.AwayTeam)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Player>()
            .HasMany(t => t.PlayerLineups)
            .WithOne(p => p.Player)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Player>()
      .HasOne(p => p.Statistics)
      .WithMany(s => s.Players)
      .HasForeignKey(s => s.StatisticID)
      .OnDelete(DeleteBehavior.Cascade);

            // Prevent cascade delete from division to coaches
            modelBuilder.Entity<Division>()
                .HasMany(d => d.DivisionCoaches)
                .WithOne(dc => dc.Division)
                .HasForeignKey(dc => dc.DivisionID)
                .OnDelete(DeleteBehavior.Restrict);


            //Prevent cascade delete from Game to Innings
            modelBuilder.Entity<Game>()
                .HasMany<Inning>(g => g.Innings)
                .WithOne(i => i.Game)
                .HasForeignKey(i => i.gameID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent cascade delete from Innings to PlayByPlay
            modelBuilder.Entity<Inning>()
                .HasMany<PlayByPlay>(i => i.PlayByPlays)
                .WithOne(pbp => pbp.Inning)
                .HasForeignKey(pbp => pbp.InningID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent cascade delete from PlayerAction to PlayByPlay
            modelBuilder.Entity<PlayerAction>()
                .HasMany<PlayByPlay>(pa => pa.PlayByPlays)
                .WithOne(pbp => pbp.PlayerAction)
                .HasForeignKey(pbp => pbp.PlayerActionID)
                .OnDelete(DeleteBehavior.Restrict);

            //Unique composite PK for division_coach
            modelBuilder.Entity<DivisionCoach>()
                .HasIndex(dc => new { dc.CoachID, dc.DivisionID})
                .IsUnique();

            //Unique composite PK for game_team
            modelBuilder.Entity<GameTeam>()
                .HasIndex(gt => new { gt.TeamID, gt.GameID })
                .IsUnique();

            //Unique composite PK for Player Jersey Number
            modelBuilder.Entity<Player>()
                .HasIndex(pp => new { pp.PlyrJerseyNumber, pp.TeamID })
                .IsUnique();

            modelBuilder.Entity<PlayerLineup>()
                .HasIndex(pp => new { pp.PlayerID, pp.LineupID })
                .IsUnique();

                //Unique constraint for GameLocation Name
            modelBuilder.Entity<GameLocation>()
                .HasIndex(p => p.Name)
                .IsUnique();
        }
    }
}
