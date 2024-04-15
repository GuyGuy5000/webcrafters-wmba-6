using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wmbaApp.Data.WBMigrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameLocation",
                table: "Games");

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Statistics",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GameLocationID",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameLocationID",
                table: "Games",
                column: "GameLocationID");

            migrationBuilder.CreateIndex(
                name: "IX_GameLocations_Name",
                table: "GameLocations",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameLocations_GameLocationID",
                table: "Games",
                column: "GameLocationID",
                principalTable: "GameLocations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameLocations_GameLocationID",
                table: "Games");

            migrationBuilder.DropTable(
                name: "GameLocations");

            migrationBuilder.DropIndex(
                name: "IX_Games_GameLocationID",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "GameLocationID",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "GameLocation",
                table: "Games",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
