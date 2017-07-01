using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeNetAPI.Migrations
{
    public partial class HomeNetMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResponseMessage",
                table: "HousePostFlag",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "isFlagged",
                table: "HousePostFlag",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "DateCreated",
                table: "House",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseMessage",
                table: "HousePostFlag");

            migrationBuilder.DropColumn(
                name: "isFlagged",
                table: "HousePostFlag");

            migrationBuilder.AlterColumn<string>(
                name: "DateCreated",
                table: "House",
                nullable: false);
        }
    }
}
