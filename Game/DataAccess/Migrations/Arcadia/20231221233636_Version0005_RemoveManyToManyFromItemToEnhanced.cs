#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Navislamia.Game.DataAccess.Migrations.Arcadia
{
    /// <inheritdoc />
    public partial class Version0005_RemoveManyToManyFromItemToEnhanced : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemResources_EnhanceResources_EnhanceResourceEntityId_Enha~",
                table: "ItemResources");

            migrationBuilder.DropIndex(
                name: "IX_ItemResources_EnhanceResourceEntityId_EnhanceResourceEntity~",
                table: "ItemResources");

            migrationBuilder.DropColumn(
                name: "EnhanceResourceEntityId",
                table: "ItemResources");

            migrationBuilder.DropColumn(
                name: "EnhanceResourceEntityLocalFlag",
                table: "ItemResources");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EnhanceResourceEntityId",
                table: "ItemResources",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnhanceResourceEntityLocalFlag",
                table: "ItemResources",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemResources_EnhanceResourceEntityId_EnhanceResourceEntity~",
                table: "ItemResources",
                columns: new[] { "EnhanceResourceEntityId", "EnhanceResourceEntityLocalFlag" });

            migrationBuilder.AddForeignKey(
                name: "FK_ItemResources_EnhanceResources_EnhanceResourceEntityId_Enha~",
                table: "ItemResources",
                columns: new[] { "EnhanceResourceEntityId", "EnhanceResourceEntityLocalFlag" },
                principalTable: "EnhanceResources",
                principalColumns: new[] { "Id", "LocalFlag" });
        }
    }
}
