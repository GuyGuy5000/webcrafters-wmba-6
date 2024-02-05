using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wmbaApp.Data.WMMigrations
{
    /// <inheritdoc />
    public partial class FixedUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerPhotos");

            migrationBuilder.DropTable(
                name: "PlayerThumbnails");

            migrationBuilder.DropIndex(
                name: "IX_Teams_TmAbbreviation",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TmAbbreviation",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "PlyrDOB",
                table: "Players",
                newName: "PlyrMemberID");

            migrationBuilder.AlterDatabase(
                collation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "TmName",
                table: "Teams",
                type: "TEXT",
                maxLength: 80,
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "DivName",
                table: "Divisions",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlyrMemberID",
                table: "Players",
                column: "PlyrMemberID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_PlyrMemberID",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "PlyrMemberID",
                table: "Players",
                newName: "PlyrDOB");

            migrationBuilder.AlterDatabase(
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "TmName",
                table: "Teams",
                type: "TEXT",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 80,
                oldCollation: "NOCASE");

            migrationBuilder.AddColumn<string>(
                name: "TmAbbreviation",
                table: "Teams",
                type: "TEXT",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DivName",
                table: "Divisions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldCollation: "NOCASE");

            migrationBuilder.CreateTable(
                name: "PlayerPhotos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
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
                name: "PlayerThumbnails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
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
                name: "IX_Teams_TmAbbreviation",
                table: "Teams",
                column: "TmAbbreviation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPhotos_PlayerID",
                table: "PlayerPhotos",
                column: "PlayerID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerThumbnails_PlayerID",
                table: "PlayerThumbnails",
                column: "PlayerID",
                unique: true);
        }
    }
}
