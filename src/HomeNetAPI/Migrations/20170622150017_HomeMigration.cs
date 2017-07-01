using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeNetAPI.Migrations
{
    public partial class HomeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isFlagged",
                table: "HousePostFlag",
                newName: "IsFlagged");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "HousePost",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsFlagged",
                table: "HousePost",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MediaResource",
                table: "HousePost",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResizedMediaResource",
                table: "HousePost",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "House",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "HousePost");

            migrationBuilder.DropColumn(
                name: "MediaResource",
                table: "HousePost");

            migrationBuilder.DropColumn(
                name: "ResizedMediaResource",
                table: "HousePost");

            migrationBuilder.RenameColumn(
                name: "IsFlagged",
                table: "HousePostFlag",
                newName: "isFlagged");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "HousePost",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "House",
                nullable: false);
        }
    }
}
