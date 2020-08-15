using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SeiyuuMoe.Infrastructure.Migrations
{
    public partial class AddAutoGenerateIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Seiyuus",
                nullable: false,
                defaultValueSql: "(uuid())",
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Blacklists",
                nullable: false,
                defaultValueSql: "(uuid())",
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Animes",
                nullable: false,
                defaultValueSql: "(uuid())",
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AnimeRoles",
                nullable: false,
                defaultValueSql: "(uuid())",
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AnimeCharacters",
                nullable: false,
                defaultValueSql: "(uuid())",
                oldClrType: typeof(string),
                oldType: "char(36)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Seiyuus",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldDefaultValueSql: "(uuid())");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Blacklists",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldDefaultValueSql: "(uuid())");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Animes",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldDefaultValueSql: "(uuid())");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AnimeRoles",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldDefaultValueSql: "(uuid())");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AnimeCharacters",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldDefaultValueSql: "(uuid())");
        }
    }
}
