using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SeiyuuMoe.Infrastructure.Migrations
{
    public partial class AddDefaultModificationDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "Seiyuus",
                type: "datetime",
                nullable: false,
                defaultValueSql: "current_timestamp()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "Animes",
                type: "datetime",
                nullable: false,
                defaultValueSql: "current_timestamp()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "AnimeCharacters",
                type: "datetime",
                nullable: false,
                defaultValueSql: "current_timestamp()");
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
