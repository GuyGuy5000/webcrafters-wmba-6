using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wmbaApp.Data.WMMigrations
{
    /// <inheritdoc />
    public partial class Initial1 : Migration
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
                    CoachPhone = table.Column<string>(type: "TEXT", nullable: true)
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
                    DivName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameStartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GameEndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GameLocation = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PosName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.ID);
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
                name: "Teams",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    TmAbbreviation = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    DivisionID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Teams_Divisions_DivisionID",
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
                        name: "FK_DivisionCoaches_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_GameTeams_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
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
                    PlyrDOB = table.Column<DateTime>(type: "TEXT", nullable: true),
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
                        name: "FK_Players_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPhotos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPhotos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlayerPhotos_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPositions",
                columns: table => new
                {
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false),
                    PositionID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPositions", x => new { x.PlayerID, x.PositionID });
                    table.ForeignKey(
                        name: "FK_PlayerPositions_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerPositions_Positions_PositionID",
                        column: x => x.PositionID,
                        principalTable: "Positions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerThumbnails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerThumbnails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlayerThumbnails_Players_PlayerID",
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
                name: "IX_GameTeams_GameID",
                table: "GameTeams",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_GameTeams_TeamID_GameID",
                table: "GameTeams",
                columns: new[] { "TeamID", "GameID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPhotos_PlayerID",
                table: "PlayerPhotos",
                column: "PlayerID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPositions_PlayerID",
                table: "PlayerPositions",
                column: "PlayerID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPositions_PositionID",
                table: "PlayerPositions",
                column: "PositionID");

            migrationBuilder.CreateIndex(
                name: "IX_Players_StatisticID",
                table: "Players",
                column: "StatisticID");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamID",
                table: "Players",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerThumbnails_PlayerID",
                table: "PlayerThumbnails",
                column: "PlayerID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PosName",
                table: "Positions",
                column: "PosName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_DivisionID",
                table: "Teams",
                column: "DivisionID");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TmAbbreviation",
                table: "Teams",
                column: "TmAbbreviation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TmName",
                table: "Teams",
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
                name: "PlayerPhotos");

            migrationBuilder.DropTable(
                name: "PlayerPositions");

            migrationBuilder.DropTable(
                name: "PlayerThumbnails");

            migrationBuilder.DropTable(
                name: "Coaches");

            migrationBuilder.DropTable(
                name: "UploadedFiles");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Divisions");
        }
    }
}
