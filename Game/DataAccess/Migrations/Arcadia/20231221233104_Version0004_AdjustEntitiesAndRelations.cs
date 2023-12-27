#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Navislamia.Game.DataAccess.Migrations.Arcadia
{
    /// <inheritdoc />
    public partial class Version0004_AdjustEntitiesAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetItemEffectResources_EffectResources_EffectId",
                table: "SetItemEffectResources");

            migrationBuilder.DropIndex(
                name: "IX_SetItemEffectResources_EffectId",
                table: "SetItemEffectResources");

            migrationBuilder.AddColumn<long>(
                name: "SetId",
                table: "EffectResources",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "SetParts",
                table: "EffectResources",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EffectResources_SetId_SetParts",
                table: "EffectResources",
                columns: new[] { "SetId", "SetParts" });

            migrationBuilder.AddForeignKey(
                name: "FK_EffectResources_SetItemEffectResources_SetId_SetParts",
                table: "EffectResources",
                columns: new[] { "SetId", "SetParts" },
                principalTable: "SetItemEffectResources",
                principalColumns: new[] { "Id", "Parts" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EffectResources_SetItemEffectResources_SetId_SetParts",
                table: "EffectResources");

            migrationBuilder.DropIndex(
                name: "IX_EffectResources_SetId_SetParts",
                table: "EffectResources");

            migrationBuilder.DropColumn(
                name: "SetId",
                table: "EffectResources");

            migrationBuilder.DropColumn(
                name: "SetParts",
                table: "EffectResources");

            migrationBuilder.CreateIndex(
                name: "IX_SetItemEffectResources_EffectId",
                table: "SetItemEffectResources",
                column: "EffectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SetItemEffectResources_EffectResources_EffectId",
                table: "SetItemEffectResources",
                column: "EffectId",
                principalTable: "EffectResources",
                principalColumn: "Id");
        }
    }
}
