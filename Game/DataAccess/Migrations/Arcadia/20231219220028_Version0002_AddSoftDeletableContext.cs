#nullable disable

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Navislamia.Game.DataAccess.Migrations.Arcadia
{
    /// <inheritdoc />
    public partial class Version0002_AddSoftDeletableContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "SummonResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "SummonResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "SummonResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "StringResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "StringResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "StringResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "StatResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "StatResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "StatResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "StateResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "StateResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "StateResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "SkillResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "SkillResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "SkillResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ItemResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ItemResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "ItemResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ItemEffectResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ItemEffectResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "ItemEffectResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "GlobalVariables",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "GlobalVariables",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "GlobalVariables",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "EnhanceResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "EnhanceResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "EnhanceResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "EffectResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "EffectResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "EffectResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ChannelResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ChannelResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "ChannelResources",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "SummonResources");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "SummonResources");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "SummonResources");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "StringResources");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "StringResources");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "StringResources");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "StatResources");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "StatResources");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "StatResources");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "StateResources");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "StateResources");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "StateResources");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "SkillResources");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "SkillResources");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "SkillResources");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ItemResources");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ItemResources");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "ItemResources");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ItemEffectResources");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ItemEffectResources");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "ItemEffectResources");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "GlobalVariables");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "GlobalVariables");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "GlobalVariables");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "EnhanceResources");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "EnhanceResources");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "EnhanceResources");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "EffectResources");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "EffectResources");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "EffectResources");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ChannelResources");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ChannelResources");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "ChannelResources");
        }
    }
}
