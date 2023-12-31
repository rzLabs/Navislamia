#nullable disable

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Navislamia.Game.DataAccess.Migrations.Telecaster
{
    /// <inheritdoc />
    public partial class Version0002_AddSoftDeletableContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                table: "Items",
                newName: "ModifiedOn");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Summons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Summons",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Summons",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Pets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Pets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Pets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Parties",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Parties",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Parties",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ItemStorages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ItemStorages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "ItemStorages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Items",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Guilds",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Guilds",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Guilds",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Dungeons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Dungeons",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Dungeons",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OtpVerifiedAt",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LogoutTime",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LoginTime",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "GuildBlockTime",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedOn",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Auctions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Auctions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Auctions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Alliances",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Alliances",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Alliances",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Summons");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Summons");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Summons");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Parties");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Parties");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Parties");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ItemStorages");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ItemStorages");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "ItemStorages");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Alliances");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Alliances");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Alliances");

            migrationBuilder.RenameColumn(
                name: "ModifiedOn",
                table: "Items",
                newName: "UpdateTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Items",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OtpVerifiedAt",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LogoutTime",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LoginTime",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "GuildBlockTime",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedOn",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
