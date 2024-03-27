using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using wmbaApp.Models;
using wmbaApp.ViewModels;

namespace wmbaApp.Data
{
    /// <summary>
    /// Context class for code first DB approach
    /// - Nadav Hilu, Emmanuel James, Farooq Jidelola
    /// </summary>
    public class WmbaContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        public WmbaContext(DbContextOptions<WmbaContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext != null)
            {
                //We have a HttpContext, but there might not be anyone Authenticated
                UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
                UserName ??= "Unknown";
            }
            else
            {
                //No HttpContext so seeding data
                UserName = "Seed Data";
            }
        }

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
        //public DbSet<PlayerPosition> PlayerPositions { get; set; }
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

            ////Many to many junction table
            //modelBuilder.Entity<PlayerPosition>()
            //    .HasKey(pp => new { pp.PlayerID, pp.PositionID });

            //Prevent cascade delete from division to teams
            modelBuilder.Entity<Division>()
                .HasMany<Team>(d => d.Teams)
                .WithOne(t => t.Division)
                .HasForeignKey(t => t.DivisionID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent cascade delete from division to teams
            modelBuilder.Entity<Team>()
                .HasMany<Player>(t => t.Players)
                .WithOne(p => p.Team)
                .HasForeignKey(p => p.TeamID)
                .OnDelete(DeleteBehavior.Restrict);


            //Prevent cascade delete from division to coaches
            modelBuilder.Entity<Division>()
                .HasMany<DivisionCoach>(d => d.DivisionCoaches)
                .WithOne(c => c.Division)
                .HasForeignKey(c => c.DivisionID)
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

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }
    }
}
