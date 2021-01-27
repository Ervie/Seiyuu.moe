using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SeiyuuMoe.Infrastructure.Migrations
{
    public partial class AddVnTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<long>(
            //    name: "VndbId",
            //    table: "Seiyuus",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "VisualNovelCharacters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(uuid())"),
                    Name = table.Column<string>(nullable: false),
                    VndbId = table.Column<long>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    KanjiName = table.Column<string>(nullable: true),
                    About = table.Column<string>(nullable: true),
                    Nicknames = table.Column<string>(nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisualNovelCharacters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisualNovelRoleTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisualNovelRoleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisualNovels",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(uuid())"),
                    VndbId = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    TitleOriginal = table.Column<string>(nullable: true),
                    Alias = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    About = table.Column<string>(nullable: true),
                    Popularity = table.Column<int>(nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisualNovels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisualNovelRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(uuid())"),
                    VisualNovelId = table.Column<Guid>(nullable: true),
                    RoleTypeId = table.Column<long>(nullable: true),
                    CharacterId = table.Column<Guid>(nullable: true),
                    SeiyuuId = table.Column<Guid>(nullable: true),
                    LanguageId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisualNovelRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisualNovelRoles_VisualNovelCharacters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "VisualNovelCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisualNovelRoles_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisualNovelRoles_VisualNovelRoleTypes_RoleTypeId",
                        column: x => x.RoleTypeId,
                        principalTable: "VisualNovelRoleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisualNovelRoles_Seiyuus_SeiyuuId",
                        column: x => x.SeiyuuId,
                        principalTable: "Seiyuus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisualNovelRoles_VisualNovels_VisualNovelId",
                        column: x => x.VisualNovelId,
                        principalTable: "VisualNovels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seiyuus_VndbId",
                table: "Seiyuus",
                column: "VndbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisualNovelCharacters_VndbId",
                table: "VisualNovelCharacters",
                column: "VndbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisualNovelRoles_CharacterId",
                table: "VisualNovelRoles",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualNovelRoles_LanguageId",
                table: "VisualNovelRoles",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualNovelRoles_RoleTypeId",
                table: "VisualNovelRoles",
                column: "RoleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualNovelRoles_SeiyuuId",
                table: "VisualNovelRoles",
                column: "SeiyuuId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualNovelRoles_VisualNovelId",
                table: "VisualNovelRoles",
                column: "VisualNovelId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualNovels_VndbId",
                table: "VisualNovels",
                column: "VndbId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisualNovelRoles");

            migrationBuilder.DropTable(
                name: "VisualNovelCharacters");

            migrationBuilder.DropTable(
                name: "VisualNovelRoleTypes");

            migrationBuilder.DropTable(
                name: "VisualNovels");

            migrationBuilder.DropIndex(
                name: "IX_Seiyuus_VndbId",
                table: "Seiyuus");

            migrationBuilder.DropColumn(
                name: "VndbId",
                table: "Seiyuus");
        }
    }
}
