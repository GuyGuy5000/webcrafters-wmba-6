using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wmbaApp.Data.WBMigrations
{
    /// <inheritdoc />
    public partial class AddedHasStarted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AwayTeamScore",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentInning",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasStarted",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HomeTeamScore",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayByPlays");

            migrationBuilder.DropTable(
                name: "Innings");

            migrationBuilder.DropTable(
                name: "PlayerActions");

            migrationBuilder.DropColumn(
                name: "AwayTeamScore",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CurrentInning",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HasStarted",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HomeTeamScore",
                table: "Games");
        }
    }
}
