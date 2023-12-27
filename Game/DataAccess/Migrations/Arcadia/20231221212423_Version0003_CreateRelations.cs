#nullable disable

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Navislamia.Game.DataAccess.Migrations.Arcadia
{
    /// <inheritdoc />
    public partial class Version0003_CreateRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SetItemEffectResources",
                table: "SetItemEffectResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemEffectResources",
                table: "ItemEffectResources");

            migrationBuilder.DropColumn(
                name: "EvolveIntoSummonId",
                table: "SummonResources");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "SummonResources");

            migrationBuilder.DropColumn(
                name: "SkillTextIds",
                table: "SummonResources");

            migrationBuilder.DropColumn(
                name: "CastFxId",
                table: "StateResources");

            migrationBuilder.DropColumn(
                name: "CastFxPosId",
                table: "StateResources");

            migrationBuilder.DropColumn(
                name: "CastSkillId",
                table: "StateResources");

            migrationBuilder.DropColumn(
                name: "HitFxId",
                table: "StateResources");

            migrationBuilder.DropColumn(
                name: "HitFxPosId",
                table: "StateResources");

            migrationBuilder.DropColumn(
                name: "SpecialOutputFxDelay",
                table: "StateResources");

            migrationBuilder.DropColumn(
                name: "StateFxId",
                table: "StateResources");

            migrationBuilder.DropColumn(
                name: "StateFxPosId",
                table: "StateResources");

            migrationBuilder.DropColumn(
                name: "RequriedLevel",
                table: "SkillResources");

            migrationBuilder.DropColumn(
                name: "SetId",
                table: "SetItemEffectResources");

            migrationBuilder.DropColumn(
                name: "SetPartFlag",
                table: "ItemResources");

            migrationBuilder.DropColumn(
                name: "EffectId",
                table: "ItemEffectResources");

            migrationBuilder.RenameColumn(
                name: "ScriptText",
                table: "SummonResources",
                newName: "ModelName");

            migrationBuilder.RenameColumn(
                name: "SkillEnchantLinkId",
                table: "SkillResources",
                newName: "RequiredLevel");

            migrationBuilder.RenameColumn(
                name: "SetParts",
                table: "SetItemEffectResources",
                newName: "Parts");

            migrationBuilder.AlterColumn<long>(
                name: "TextFeatureId",
                table: "SummonResources",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "StatId",
                table: "SummonResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long?[]>(
                name: "SkillIds",
                table: "SummonResources",
                type: "bigint[]",
                nullable: true,
                oldClrType: typeof(int[]),
                oldType: "integer[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "NameId",
                table: "SummonResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "MotionFileId",
                table: "SummonResources",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "ModelId",
                table: "SummonResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "FaceId",
                table: "SummonResources",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "CardId",
                table: "SummonResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "EvolveTargetId",
                table: "SummonResources",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StringResourceEntityId",
                table: "SummonResources",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TooltipId",
                table: "StateResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "TextId",
                table: "StateResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "PosId",
                table: "StateResources",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "IconId",
                table: "StateResources",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "FxId",
                table: "StateResources",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "TooltipId",
                table: "SkillResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "TextId",
                table: "SkillResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "StateId",
                table: "SkillResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "RequiredStateId",
                table: "SkillResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "IconId",
                table: "SkillResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "DescriptionId",
                table: "SkillResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "SummonId",
                table: "SkillResources",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpgradeIntoSkillId",
                table: "SkillResources",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TooltipId",
                table: "SetItemEffectResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "TextId",
                table: "SetItemEffectResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "EffectId",
                table: "SetItemEffectResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "SetItemEffectResources",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "SetItemEffectResources",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "SetItemEffectResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "SetItemEffectResources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TooltipId",
                table: "ItemResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "SummonId",
                table: "ItemResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "StateId",
                table: "ItemResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "SkillId",
                table: "ItemResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "SetId",
                table: "ItemResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "NameId",
                table: "ItemResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EffectId",
                table: "ItemResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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

            migrationBuilder.AddColumn<short>(
                name: "SetPart",
                table: "ItemResources",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "EffectTrigger",
                table: "ItemEffectResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "RequiredItemId",
                table: "EnhanceResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "ItemEffectId",
                table: "EffectResources",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ItemEffectOrdinalId",
                table: "EffectResources",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModelEffectResourceEntityId",
                table: "EffectResources",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SetItemEffectResources",
                table: "SetItemEffectResources",
                columns: new[] { "Id", "Parts" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemEffectResources",
                table: "ItemEffectResources",
                columns: new[] { "Id", "OrdinalId" });

            migrationBuilder.CreateTable(
                name: "ModelEffectResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EffectFileId = table.Column<long>(type: "bigint", nullable: true),
                    LoopEffect = table.Column<bool>(type: "boolean", nullable: false),
                    EffectPosition = table.Column<int>(type: "integer", nullable: true),
                    BoneNames = table.Column<string[]>(type: "text[]", nullable: true),
                    BoneEffectIds = table.Column<long?[]>(type: "bigint[]", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelEffectResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelEffectResources_EffectResources_EffectFileId",
                        column: x => x.EffectFileId,
                        principalTable: "EffectResources",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SummonResources_CardId",
                table: "SummonResources",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_SummonResources_EvolveTargetId",
                table: "SummonResources",
                column: "EvolveTargetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SummonResources_ModelId",
                table: "SummonResources",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SummonResources_NameId",
                table: "SummonResources",
                column: "NameId");

            migrationBuilder.CreateIndex(
                name: "IX_SummonResources_StatId",
                table: "SummonResources",
                column: "StatId");

            migrationBuilder.CreateIndex(
                name: "IX_SummonResources_StringResourceEntityId",
                table: "SummonResources",
                column: "StringResourceEntityId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SummonResourceEntity_CameraPosition_MaxSize3",
                table: "SummonResources",
                sql: "cardinality(\"CameraPosition\") <= 3");

            migrationBuilder.CreateIndex(
                name: "IX_StateResources_TextId",
                table: "StateResources",
                column: "TextId");

            migrationBuilder.CreateIndex(
                name: "IX_StateResources_TooltipId",
                table: "StateResources",
                column: "TooltipId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_StateResourceEntity_Values_MaxSize20",
                table: "StateResources",
                sql: "cardinality(\"Values\") <= 20");

            migrationBuilder.CreateIndex(
                name: "IX_SkillResources_DescriptionId",
                table: "SkillResources",
                column: "DescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillResources_RequiredStateId",
                table: "SkillResources",
                column: "RequiredStateId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillResources_StateId",
                table: "SkillResources",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillResources_SummonId",
                table: "SkillResources",
                column: "SummonId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillResources_TextId",
                table: "SkillResources",
                column: "TextId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillResources_TooltipId",
                table: "SkillResources",
                column: "TooltipId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillResources_UpgradeIntoSkillId",
                table: "SkillResources",
                column: "UpgradeIntoSkillId",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_SkillResourceEntity_Values_MaxSize20",
                table: "SkillResources",
                sql: "cardinality(\"Values\") <= 20");

            migrationBuilder.CreateIndex(
                name: "IX_SetItemEffectResources_EffectId",
                table: "SetItemEffectResources",
                column: "EffectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SetItemEffectResources_TextId",
                table: "SetItemEffectResources",
                column: "TextId");

            migrationBuilder.CreateIndex(
                name: "IX_SetItemEffectResources_TooltipId",
                table: "SetItemEffectResources",
                column: "TooltipId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SetItemEffectResourceEntity_BaseTypes_MaxSize4",
                table: "SetItemEffectResources",
                sql: "cardinality(\"BaseTypes\") <= 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SetItemEffectResourceEntity_BaseValues_MaxSize8",
                table: "SetItemEffectResources",
                sql: "cardinality(\"BaseValues\") <= 8");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SetItemEffectResourceEntity_OptTypes_MaxSize4",
                table: "SetItemEffectResources",
                sql: "cardinality(\"OptTypes\") <= 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SetItemEffectResourceEntity_OptValues_MaxSize8",
                table: "SetItemEffectResources",
                sql: "cardinality(\"OptValues\") <= 8");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LevelResourceEntity_JLvs_MaxSize4",
                table: "LevelResources",
                sql: "cardinality(\"JLvs\") <= 4");

            migrationBuilder.CreateIndex(
                name: "IX_ItemResources_EffectId",
                table: "ItemResources",
                column: "EffectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemResources_EnhanceResourceEntityId_EnhanceResourceEntity~",
                table: "ItemResources",
                columns: new[] { "EnhanceResourceEntityId", "EnhanceResourceEntityLocalFlag" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemResources_NameId",
                table: "ItemResources",
                column: "NameId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemResources_SkillId",
                table: "ItemResources",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemResources_StateId",
                table: "ItemResources",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemResources_SummonId",
                table: "ItemResources",
                column: "SummonId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemResources_TooltipId",
                table: "ItemResources",
                column: "TooltipId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemResourceEntity_BaseTypes_MaxSize4",
                table: "ItemResources",
                sql: "cardinality(\"BaseTypes\") <= 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemResourceEntity_BaseValues_MaxSize8",
                table: "ItemResources",
                sql: "cardinality(\"BaseValues\") <= 8");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemResourceEntity_EnhanceIds_MaxSize2",
                table: "ItemResources",
                sql: "cardinality(\"EnhanceIds\") <= 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemResourceEntity_EnhanceValues_MaxSize8",
                table: "ItemResources",
                sql: "cardinality(\"EnhanceValues\") <= 8");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemResourceEntity_OptTypes_MaxSize4",
                table: "ItemResources",
                sql: "cardinality(\"OptTypes\") <= 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemResourceEntity_OptValues_MaxSize8",
                table: "ItemResources",
                sql: "cardinality(\"OptValues\") <= 8");

            migrationBuilder.CreateIndex(
                name: "IX_ItemEffectResources_TooltipId",
                table: "ItemEffectResources",
                column: "TooltipId");

            migrationBuilder.CreateIndex(
                name: "IX_EnhanceResources_RequiredItemId",
                table: "EnhanceResources",
                column: "RequiredItemId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_EnhanceResourceEntity_Percentage_MaxSize20",
                table: "EnhanceResources",
                sql: "cardinality(\"Percentage\") <= 20");

            migrationBuilder.CreateIndex(
                name: "IX_EffectResources_ItemEffectId_ItemEffectOrdinalId",
                table: "EffectResources",
                columns: new[] { "ItemEffectId", "ItemEffectOrdinalId" });

            migrationBuilder.CreateIndex(
                name: "IX_EffectResources_ModelEffectResourceEntityId",
                table: "EffectResources",
                column: "ModelEffectResourceEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelEffectResources_EffectFileId",
                table: "ModelEffectResources",
                column: "EffectFileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EffectResources_ItemEffectResources_ItemEffectId_ItemEffect~",
                table: "EffectResources",
                columns: new[] { "ItemEffectId", "ItemEffectOrdinalId" },
                principalTable: "ItemEffectResources",
                principalColumns: new[] { "Id", "OrdinalId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EffectResources_ModelEffectResources_ModelEffectResourceEnt~",
                table: "EffectResources",
                column: "ModelEffectResourceEntityId",
                principalTable: "ModelEffectResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EnhanceResources_ItemResources_RequiredItemId",
                table: "EnhanceResources",
                column: "RequiredItemId",
                principalTable: "ItemResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemEffectResources_StringResources_TooltipId",
                table: "ItemEffectResources",
                column: "TooltipId",
                principalTable: "StringResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemResources_EffectResources_EffectId",
                table: "ItemResources",
                column: "EffectId",
                principalTable: "EffectResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemResources_EnhanceResources_EnhanceResourceEntityId_Enha~",
                table: "ItemResources",
                columns: new[] { "EnhanceResourceEntityId", "EnhanceResourceEntityLocalFlag" },
                principalTable: "EnhanceResources",
                principalColumns: new[] { "Id", "LocalFlag" });

            migrationBuilder.AddForeignKey(
                name: "FK_ItemResources_SkillResources_SkillId",
                table: "ItemResources",
                column: "SkillId",
                principalTable: "SkillResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemResources_StateResources_StateId",
                table: "ItemResources",
                column: "StateId",
                principalTable: "StateResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemResources_StringResources_NameId",
                table: "ItemResources",
                column: "NameId",
                principalTable: "StringResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemResources_StringResources_TooltipId",
                table: "ItemResources",
                column: "TooltipId",
                principalTable: "StringResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemResources_SummonResources_SummonId",
                table: "ItemResources",
                column: "SummonId",
                principalTable: "SummonResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SetItemEffectResources_EffectResources_EffectId",
                table: "SetItemEffectResources",
                column: "EffectId",
                principalTable: "EffectResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SetItemEffectResources_StringResources_TextId",
                table: "SetItemEffectResources",
                column: "TextId",
                principalTable: "StringResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SetItemEffectResources_StringResources_TooltipId",
                table: "SetItemEffectResources",
                column: "TooltipId",
                principalTable: "StringResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillResources_SkillResources_UpgradeIntoSkillId",
                table: "SkillResources",
                column: "UpgradeIntoSkillId",
                principalTable: "SkillResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillResources_StateResources_RequiredStateId",
                table: "SkillResources",
                column: "RequiredStateId",
                principalTable: "StateResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillResources_StateResources_StateId",
                table: "SkillResources",
                column: "StateId",
                principalTable: "StateResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillResources_StringResources_DescriptionId",
                table: "SkillResources",
                column: "DescriptionId",
                principalTable: "StringResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillResources_StringResources_TextId",
                table: "SkillResources",
                column: "TextId",
                principalTable: "StringResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillResources_StringResources_TooltipId",
                table: "SkillResources",
                column: "TooltipId",
                principalTable: "StringResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillResources_SummonResources_SummonId",
                table: "SkillResources",
                column: "SummonId",
                principalTable: "SummonResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StateResources_StringResources_TextId",
                table: "StateResources",
                column: "TextId",
                principalTable: "StringResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StateResources_StringResources_TooltipId",
                table: "StateResources",
                column: "TooltipId",
                principalTable: "StringResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SummonResources_ItemResources_CardId",
                table: "SummonResources",
                column: "CardId",
                principalTable: "ItemResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SummonResources_ModelEffectResources_ModelId",
                table: "SummonResources",
                column: "ModelId",
                principalTable: "ModelEffectResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SummonResources_StatResources_StatId",
                table: "SummonResources",
                column: "StatId",
                principalTable: "StatResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SummonResources_StringResources_NameId",
                table: "SummonResources",
                column: "NameId",
                principalTable: "StringResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SummonResources_StringResources_StringResourceEntityId",
                table: "SummonResources",
                column: "StringResourceEntityId",
                principalTable: "StringResources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SummonResources_SummonResources_EvolveTargetId",
                table: "SummonResources",
                column: "EvolveTargetId",
                principalTable: "SummonResources",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EffectResources_ItemEffectResources_ItemEffectId_ItemEffect~",
                table: "EffectResources");

            migrationBuilder.DropForeignKey(
                name: "FK_EffectResources_ModelEffectResources_ModelEffectResourceEnt~",
                table: "EffectResources");

            migrationBuilder.DropForeignKey(
                name: "FK_EnhanceResources_ItemResources_RequiredItemId",
                table: "EnhanceResources");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemEffectResources_StringResources_TooltipId",
                table: "ItemEffectResources");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemResources_EffectResources_EffectId",
                table: "ItemResources");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemResources_EnhanceResources_EnhanceResourceEntityId_Enha~",
                table: "ItemResources");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemResources_SkillResources_SkillId",
                table: "ItemResources");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemResources_StateResources_StateId",
                table: "ItemResources");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemResources_StringResources_NameId",
                table: "ItemResources");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemResources_StringResources_TooltipId",
                table: "ItemResources");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemResources_SummonResources_SummonId",
                table: "ItemResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SetItemEffectResources_EffectResources_EffectId",
                table: "SetItemEffectResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SetItemEffectResources_StringResources_TextId",
                table: "SetItemEffectResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SetItemEffectResources_StringResources_TooltipId",
                table: "SetItemEffectResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillResources_SkillResources_UpgradeIntoSkillId",
                table: "SkillResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillResources_StateResources_RequiredStateId",
                table: "SkillResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillResources_StateResources_StateId",
                table: "SkillResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillResources_StringResources_DescriptionId",
                table: "SkillResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillResources_StringResources_TextId",
                table: "SkillResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillResources_StringResources_TooltipId",
                table: "SkillResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillResources_SummonResources_SummonId",
                table: "SkillResources");

            migrationBuilder.DropForeignKey(
                name: "FK_StateResources_StringResources_TextId",
                table: "StateResources");

            migrationBuilder.DropForeignKey(
                name: "FK_StateResources_StringResources_TooltipId",
                table: "StateResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SummonResources_ItemResources_CardId",
                table: "SummonResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SummonResources_ModelEffectResources_ModelId",
                table: "SummonResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SummonResources_StatResources_StatId",
                table: "SummonResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SummonResources_StringResources_NameId",
                table: "SummonResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SummonResources_StringResources_StringResourceEntityId",
                table: "SummonResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SummonResources_SummonResources_EvolveTargetId",
                table: "SummonResources");

            migrationBuilder.DropTable(
                name: "ModelEffectResources");

            migrationBuilder.DropIndex(
                name: "IX_SummonResources_CardId",
                table: "SummonResources");

            migrationBuilder.DropIndex(
                name: "IX_SummonResources_EvolveTargetId",
                table: "SummonResources");

            migrationBuilder.DropIndex(
                name: "IX_SummonResources_ModelId",
                table: "SummonResources");

            migrationBuilder.DropIndex(
                name: "IX_SummonResources_NameId",
                table: "SummonResources");

            migrationBuilder.DropIndex(
                name: "IX_SummonResources_StatId",
                table: "SummonResources");

            migrationBuilder.DropIndex(
                name: "IX_SummonResources_StringResourceEntityId",
                table: "SummonResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SummonResourceEntity_CameraPosition_MaxSize3",
                table: "SummonResources");

            migrationBuilder.DropIndex(
                name: "IX_StateResources_TextId",
                table: "StateResources");

            migrationBuilder.DropIndex(
                name: "IX_StateResources_TooltipId",
                table: "StateResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_StateResourceEntity_Values_MaxSize20",
                table: "StateResources");

            migrationBuilder.DropIndex(
                name: "IX_SkillResources_DescriptionId",
                table: "SkillResources");

            migrationBuilder.DropIndex(
                name: "IX_SkillResources_RequiredStateId",
                table: "SkillResources");

            migrationBuilder.DropIndex(
                name: "IX_SkillResources_StateId",
                table: "SkillResources");

            migrationBuilder.DropIndex(
                name: "IX_SkillResources_SummonId",
                table: "SkillResources");

            migrationBuilder.DropIndex(
                name: "IX_SkillResources_TextId",
                table: "SkillResources");

            migrationBuilder.DropIndex(
                name: "IX_SkillResources_TooltipId",
                table: "SkillResources");

            migrationBuilder.DropIndex(
                name: "IX_SkillResources_UpgradeIntoSkillId",
                table: "SkillResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SkillResourceEntity_Values_MaxSize20",
                table: "SkillResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SetItemEffectResources",
                table: "SetItemEffectResources");

            migrationBuilder.DropIndex(
                name: "IX_SetItemEffectResources_EffectId",
                table: "SetItemEffectResources");

            migrationBuilder.DropIndex(
                name: "IX_SetItemEffectResources_TextId",
                table: "SetItemEffectResources");

            migrationBuilder.DropIndex(
                name: "IX_SetItemEffectResources_TooltipId",
                table: "SetItemEffectResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SetItemEffectResourceEntity_BaseTypes_MaxSize4",
                table: "SetItemEffectResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SetItemEffectResourceEntity_BaseValues_MaxSize8",
                table: "SetItemEffectResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SetItemEffectResourceEntity_OptTypes_MaxSize4",
                table: "SetItemEffectResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SetItemEffectResourceEntity_OptValues_MaxSize8",
                table: "SetItemEffectResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LevelResourceEntity_JLvs_MaxSize4",
                table: "LevelResources");

            migrationBuilder.DropIndex(
                name: "IX_ItemResources_EffectId",
                table: "ItemResources");

            migrationBuilder.DropIndex(
                name: "IX_ItemResources_EnhanceResourceEntityId_EnhanceResourceEntity~",
                table: "ItemResources");

            migrationBuilder.DropIndex(
                name: "IX_ItemResources_NameId",
                table: "ItemResources");

            migrationBuilder.DropIndex(
                name: "IX_ItemResources_SkillId",
                table: "ItemResources");

            migrationBuilder.DropIndex(
                name: "IX_ItemResources_StateId",
                table: "ItemResources");

            migrationBuilder.DropIndex(
                name: "IX_ItemResources_SummonId",
                table: "ItemResources");

            migrationBuilder.DropIndex(
                name: "IX_ItemResources_TooltipId",
                table: "ItemResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemResourceEntity_BaseTypes_MaxSize4",
                table: "ItemResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemResourceEntity_BaseValues_MaxSize8",
                table: "ItemResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemResourceEntity_EnhanceIds_MaxSize2",
                table: "ItemResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemResourceEntity_EnhanceValues_MaxSize8",
                table: "ItemResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemResourceEntity_OptTypes_MaxSize4",
                table: "ItemResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemResourceEntity_OptValues_MaxSize8",
                table: "ItemResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemEffectResources",
                table: "ItemEffectResources");

            migrationBuilder.DropIndex(
                name: "IX_ItemEffectResources_TooltipId",
                table: "ItemEffectResources");

            migrationBuilder.DropIndex(
                name: "IX_EnhanceResources_RequiredItemId",
                table: "EnhanceResources");

            migrationBuilder.DropCheckConstraint(
                name: "CK_EnhanceResourceEntity_Percentage_MaxSize20",
                table: "EnhanceResources");

            migrationBuilder.DropIndex(
                name: "IX_EffectResources_ItemEffectId_ItemEffectOrdinalId",
                table: "EffectResources");

            migrationBuilder.DropIndex(
                name: "IX_EffectResources_ModelEffectResourceEntityId",
                table: "EffectResources");

            migrationBuilder.DropColumn(
                name: "EvolveTargetId",
                table: "SummonResources");

            migrationBuilder.DropColumn(
                name: "StringResourceEntityId",
                table: "SummonResources");

            migrationBuilder.DropColumn(
                name: "SummonId",
                table: "SkillResources");

            migrationBuilder.DropColumn(
                name: "UpgradeIntoSkillId",
                table: "SkillResources");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SetItemEffectResources");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "SetItemEffectResources");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "SetItemEffectResources");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "SetItemEffectResources");

            migrationBuilder.DropColumn(
                name: "EnhanceResourceEntityId",
                table: "ItemResources");

            migrationBuilder.DropColumn(
                name: "EnhanceResourceEntityLocalFlag",
                table: "ItemResources");

            migrationBuilder.DropColumn(
                name: "SetPart",
                table: "ItemResources");

            migrationBuilder.DropColumn(
                name: "EffectTrigger",
                table: "ItemEffectResources");

            migrationBuilder.DropColumn(
                name: "ItemEffectId",
                table: "EffectResources");

            migrationBuilder.DropColumn(
                name: "ItemEffectOrdinalId",
                table: "EffectResources");

            migrationBuilder.DropColumn(
                name: "ModelEffectResourceEntityId",
                table: "EffectResources");

            migrationBuilder.RenameColumn(
                name: "ModelName",
                table: "SummonResources",
                newName: "ScriptText");

            migrationBuilder.RenameColumn(
                name: "RequiredLevel",
                table: "SkillResources",
                newName: "SkillEnchantLinkId");

            migrationBuilder.RenameColumn(
                name: "Parts",
                table: "SetItemEffectResources",
                newName: "SetParts");

            migrationBuilder.AlterColumn<int>(
                name: "TextFeatureId",
                table: "SummonResources",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "StatId",
                table: "SummonResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int[]>(
                name: "SkillIds",
                table: "SummonResources",
                type: "integer[]",
                nullable: true,
                oldClrType: typeof(long?[]),
                oldType: "bigint[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NameId",
                table: "SummonResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MotionFileId",
                table: "SummonResources",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "ModelId",
                table: "SummonResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FaceId",
                table: "SummonResources",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "CardId",
                table: "SummonResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EvolveIntoSummonId",
                table: "SummonResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "SummonResources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "SkillTextIds",
                table: "SummonResources",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TooltipId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TextId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PosId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "IconId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "FxId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "CastFxId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CastFxPosId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CastSkillId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HitFxId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HitFxPosId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpecialOutputFxDelay",
                table: "StateResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateFxId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateFxPosId",
                table: "StateResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TooltipId",
                table: "SkillResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TextId",
                table: "SkillResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StateId",
                table: "SkillResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RequiredStateId",
                table: "SkillResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IconId",
                table: "SkillResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DescriptionId",
                table: "SkillResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequriedLevel",
                table: "SkillResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TooltipId",
                table: "SetItemEffectResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TextId",
                table: "SetItemEffectResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EffectId",
                table: "SetItemEffectResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetId",
                table: "SetItemEffectResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TooltipId",
                table: "ItemResources",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SummonId",
                table: "ItemResources",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StateId",
                table: "ItemResources",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SkillId",
                table: "ItemResources",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SetId",
                table: "ItemResources",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NameId",
                table: "ItemResources",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EffectId",
                table: "ItemResources",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetPartFlag",
                table: "ItemResources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "EffectId",
                table: "ItemEffectResources",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<int>(
                name: "RequiredItemId",
                table: "EnhanceResources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SetItemEffectResources",
                table: "SetItemEffectResources",
                columns: new[] { "SetId", "SetParts" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemEffectResources",
                table: "ItemEffectResources",
                column: "Id");
        }
    }
}
