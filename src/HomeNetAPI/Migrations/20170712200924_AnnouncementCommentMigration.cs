using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeNetAPI.Migrations
{
    public partial class AnnouncementCommentMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "AnnouncementComment");

            migrationBuilder.AddColumn<int>(
                name: "HouseMemberID",
                table: "AnnouncementComment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementComment_HouseMemberID",
                table: "AnnouncementComment",
                column: "HouseMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementComment_HouseMember_HouseMemberID",
                table: "AnnouncementComment",
                column: "HouseMemberID",
                principalTable: "HouseMember",
                principalColumn: "HouseMemberID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementComment_HouseMember_HouseMemberID",
                table: "AnnouncementComment");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementComment_HouseMemberID",
                table: "AnnouncementComment");

            migrationBuilder.DropColumn(
                name: "HouseMemberID",
                table: "AnnouncementComment");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AnnouncementComment",
                nullable: true);
        }
    }
}
