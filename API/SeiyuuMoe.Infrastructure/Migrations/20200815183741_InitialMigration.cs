using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SeiyuuMoe.Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimeCharacters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    MalId = table.Column<long>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    Popularity = table.Column<long>(nullable: true),
                    KanjiName = table.Column<string>(nullable: true),
                    About = table.Column<string>(nullable: true),
                    Nicknames = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeCharacters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnimeRoleTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeRoleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnimeSeasons",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Year = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeSeasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnimeStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnimeTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blacklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MalId = table.Column<long>(nullable: false),
                    EntityType = table.Column<string>(nullable: false),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blacklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seiyuus",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    MalId = table.Column<long>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    Popularity = table.Column<long>(nullable: true),
                    KanjiName = table.Column<string>(nullable: true),
                    About = table.Column<string>(nullable: true),
                    Birthday = table.Column<DateTime>(type: "DATE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seiyuus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Animes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    MalId = table.Column<long>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    Popularity = table.Column<long>(nullable: true),
                    EnglishTitle = table.Column<string>(nullable: true),
                    KanjiTitle = table.Column<string>(nullable: true),
                    TitleSynonyms = table.Column<string>(nullable: true),
                    About = table.Column<string>(nullable: true),
                    AiringDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    StatusId = table.Column<long>(nullable: true),
                    TypeId = table.Column<long>(nullable: true),
                    SeasonId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animes_AnimeSeasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "AnimeSeasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Animes_AnimeStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "AnimeStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Animes_AnimeTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "AnimeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AnimeRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AnimeId = table.Column<Guid>(nullable: true),
                    RoleTypeId = table.Column<long>(nullable: true),
                    CharacterId = table.Column<Guid>(nullable: true),
                    SeiyuuId = table.Column<Guid>(nullable: true),
                    LanguageId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimeRoles_Animes_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "Animes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimeRoles_AnimeCharacters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "AnimeCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimeRoles_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnimeRoles_AnimeRoleTypes_RoleTypeId",
                        column: x => x.RoleTypeId,
                        principalTable: "AnimeRoleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimeRoles_Seiyuus_SeiyuuId",
                        column: x => x.SeiyuuId,
                        principalTable: "Seiyuus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimeCharacters_MalId",
                table: "AnimeCharacters",
                column: "MalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnimeRoles_AnimeId",
                table: "AnimeRoles",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeRoles_CharacterId",
                table: "AnimeRoles",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeRoles_LanguageId",
                table: "AnimeRoles",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeRoles_RoleTypeId",
                table: "AnimeRoles",
                column: "RoleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeRoles_SeiyuuId",
                table: "AnimeRoles",
                column: "SeiyuuId");

            migrationBuilder.CreateIndex(
                name: "IX_Animes_MalId",
                table: "Animes",
                column: "MalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Animes_SeasonId",
                table: "Animes",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Animes_StatusId",
                table: "Animes",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Animes_TypeId",
                table: "Animes",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Seiyuus_MalId",
                table: "Seiyuus",
                column: "MalId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimeRoles");

            migrationBuilder.DropTable(
                name: "Blacklists");

            migrationBuilder.DropTable(
                name: "Animes");

            migrationBuilder.DropTable(
                name: "AnimeCharacters");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "AnimeRoleTypes");

            migrationBuilder.DropTable(
                name: "Seiyuus");

            migrationBuilder.DropTable(
                name: "AnimeSeasons");

            migrationBuilder.DropTable(
                name: "AnimeStatuses");

            migrationBuilder.DropTable(
                name: "AnimeTypes");
        }
    }
}
