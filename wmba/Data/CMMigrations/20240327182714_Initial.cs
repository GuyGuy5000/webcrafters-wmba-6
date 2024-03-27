using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wmbaApp.Data.CMMigrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coaches",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CoachFirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CoachLastName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    CoachEmail = table.Column<string>(type: "TEXT", nullable: true),
                    CoachPhone = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coaches", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DivName = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GameLocations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLocations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Lineups",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lineups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PlayerActions",
                columns: table => new
                {
                    PlayerActionID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerActionName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerActions", x => x.PlayerActionID);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StatsGP = table.Column<int>(type: "INTEGER", nullable: true),
                    StatsPA = table.Column<int>(type: "INTEGER", nullable: true),
                    StatsAB = table.Column<int>(type: "INTEGER", nullable: true),
                    StatsAVG = table.Column<double>(type: "REAL", nullable: true),
                    StatsOBP = table.Column<double>(type: "REAL", nullable: true),
                    StatsOPS = table.Column<double>(type: "REAL", nullable: true),
                    StatsSLG = table.Column<double>(type: "REAL", nullable: true),
                    StatsH = table.Column<int>(type: "INTEGER", nullable: true),
                    StatsR = table.Column<int>(type: "INTEGER", nullable: true),
                    StatsK = table.Column<int>(type: "INTEGER", nullable: true),
                    StatsHR = table.Column<int>(type: "INTEGER", nullable: true),
                    StatsRBI = table.Column<int>(type: "INTEGER", nullable: true),
                    StatsBB = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false, collation: "NOCASE"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DivisionID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Team_Divisions_DivisionID",
                        column: x => x.DivisionID,
                        principalTable: "Divisions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileProperty",
                columns: table => new
                {
                    FilePropertyID = table.Column<int>(type: "INTEGER", nullable: false),
                    Property = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileProperty", x => x.FilePropertyID);
                    table.ForeignKey(
                        name: "FK_FileProperty_UploadedFiles_FilePropertyID",
                        column: x => x.FilePropertyID,
                        principalTable: "UploadedFiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DivisionCoaches",
                columns: table => new
                {
                    DivisionID = table.Column<int>(type: "INTEGER", nullable: false),
                    CoachID = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DivisionCoaches", x => new { x.DivisionID, x.CoachID });
                    table.ForeignKey(
                        name: "FK_DivisionCoaches_Coaches_CoachID",
                        column: x => x.CoachID,
                        principalTable: "Coaches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DivisionCoaches_Divisions_DivisionID",
                        column: x => x.DivisionID,
                        principalTable: "Divisions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DivisionCoaches_Team_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Team",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameStartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GameEndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GameLocationID = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeTeamScore = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayTeamScore = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentInning = table.Column<int>(type: "INTEGER", nullable: false),
                    HasStarted = table.Column<bool>(type: "INTEGER", nullable: false),
                    HomeTeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayTeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    DivisionID = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeLineupID = table.Column<int>(type: "INTEGER", nullable: true),
                    AwayLineupID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Games_Divisions_DivisionID",
                        column: x => x.DivisionID,
                        principalTable: "Divisions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_GameLocations_GameLocationID",
                        column: x => x.GameLocationID,
                        principalTable: "GameLocations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Lineups_AwayLineupID",
                        column: x => x.AwayLineupID,
                        principalTable: "Lineups",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Games_Lineups_HomeLineupID",
                        column: x => x.HomeLineupID,
                        principalTable: "Lineups",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Games_Team_AwayTeamID",
                        column: x => x.AwayTeamID,
                        principalTable: "Team",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Team_HomeTeamID",
                        column: x => x.HomeTeamID,
                        principalTable: "Team",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlyrFirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PlyrLastName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    PlyrJerseyNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    PlyrMemberID = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    StatisticID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Players_Statistics_StatisticID",
                        column: x => x.StatisticID,
                        principalTable: "Statistics",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Players_Team_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Team",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameTeams",
                columns: table => new
                {
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false),
                    GmtmLineup = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    GmtmScore = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTeams", x => new { x.TeamID, x.GameID });
                    table.ForeignKey(
                        name: "FK_GameTeams_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameTeams_Team_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Team",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Innings",
                columns: table => new
                {
                    InningID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HomeTeamScore = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayTeamScore = table.Column<int>(type: "INTEGER", nullable: false),
                    gameID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Innings", x => x.InningID);
                    table.ForeignKey(
                        name: "FK_Innings_Games_gameID",
                        column: x => x.gameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerLineup",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LineupID = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerLineup", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlayerLineup_Lineups_LineupID",
                        column: x => x.LineupID,
                        principalTable: "Lineups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerLineup_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PlayByPlays",
                columns: table => new
                {
                    PlayByPlayID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerActionID = table.Column<int>(type: "INTEGER", nullable: false),
                    InningID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayByPlays", x => x.PlayByPlayID);
                    table.ForeignKey(
                        name: "FK_PlayByPlays_Innings_InningID",
                        column: x => x.InningID,
                        principalTable: "Innings",
                        principalColumn: "InningID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayByPlays_PlayerActions_PlayerActionID",
                        column: x => x.PlayerActionID,
                        principalTable: "PlayerActions",
                        principalColumn: "PlayerActionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayByPlays_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DivisionCoaches_CoachID_DivisionID",
                table: "DivisionCoaches",
                columns: new[] { "CoachID", "DivisionID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DivisionCoaches_TeamID",
                table: "DivisionCoaches",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_DivName",
                table: "Divisions",
                column: "DivName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameLocations_Name",
                table: "GameLocations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_AwayLineupID",
                table: "Games",
                column: "AwayLineupID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_AwayTeamID",
                table: "Games",
                column: "AwayTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_DivisionID",
                table: "Games",
                column: "DivisionID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameLocationID",
                table: "Games",
                column: "GameLocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_HomeLineupID",
                table: "Games",
                column: "HomeLineupID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_HomeTeamID",
                table: "Games",
                column: "HomeTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_GameTeams_GameID",
                table: "GameTeams",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_GameTeams_TeamID_GameID",
                table: "GameTeams",
                columns: new[] { "TeamID", "GameID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Innings_gameID",
                table: "Innings",
                column: "gameID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayByPlays_InningID",
                table: "PlayByPlays",
                column: "InningID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayByPlays_PlayerActionID",
                table: "PlayByPlays",
                column: "PlayerActionID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayByPlays_PlayerID",
                table: "PlayByPlays",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerActions_PlayerActionName",
                table: "PlayerActions",
                column: "PlayerActionName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLineup_LineupID",
                table: "PlayerLineup",
                column: "LineupID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLineup_PlayerID_LineupID",
                table: "PlayerLineup",
                columns: new[] { "PlayerID", "LineupID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlyrJerseyNumber_TeamID",
                table: "Players",
                columns: new[] { "PlyrJerseyNumber", "TeamID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlyrMemberID",
                table: "Players",
                column: "PlyrMemberID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_StatisticID",
                table: "Players",
                column: "StatisticID");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamID",
                table: "Players",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Team_DivisionID",
                table: "Team",
                column: "DivisionID");

            migrationBuilder.CreateIndex(
                name: "IX_Team_TmName",
                table: "Team",
                column: "TmName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DivisionCoaches");

            migrationBuilder.DropTable(
                name: "FileProperty");

            migrationBuilder.DropTable(
                name: "GameTeams");

            migrationBuilder.DropTable(
                name: "PlayByPlays");

            migrationBuilder.DropTable(
                name: "PlayerLineup");

            migrationBuilder.DropTable(
                name: "Coaches");

            migrationBuilder.DropTable(
                name: "UploadedFiles");

            migrationBuilder.DropTable(
                name: "Innings");

            migrationBuilder.DropTable(
                name: "PlayerActions");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "GameLocations");

            migrationBuilder.DropTable(
                name: "Lineups");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "Divisions");
        }
    }
}
