using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SeiyuuMoe.Infrastructure.Migrations
{
    public partial class AddModificationDateColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "Seiyuus",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "Animes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "AnimeCharacters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "Seiyuus");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "Animes");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "AnimeCharacters");
        }
    }
}
