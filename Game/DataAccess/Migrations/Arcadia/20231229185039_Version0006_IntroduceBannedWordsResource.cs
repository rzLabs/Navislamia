using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navislamia.Game.DataAccess.Migrations.Arcadia
{
    /// <inheritdoc />
    public partial class Version0006_IntroduceBannedWordsResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BannedWordsResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Word = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannedWordsResources", x => new { x.Id, x.Word });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BannedWordsResources");
        }
    }
}
