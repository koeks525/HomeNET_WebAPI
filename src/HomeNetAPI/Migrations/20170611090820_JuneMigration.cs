using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeNetAPI.Migrations
{
    public partial class JuneMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_CountryID",
                table: "User",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageThreadMessage_HouseMemberID",
                table: "MessageThreadMessage",
                column: "HouseMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageThreadMessage_MessageThreadID",
                table: "MessageThreadMessage",
                column: "MessageThreadID");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageThreadMessage_HouseMember_HouseMemberID",
                table: "MessageThreadMessage",
                column: "HouseMemberID",
                principalTable: "HouseMember",
                principalColumn: "HouseMemberID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageThreadMessage_MessageThread_MessageThreadID",
                table: "MessageThreadMessage",
                column: "MessageThreadID",
                principalTable: "MessageThread",
                principalColumn: "MessageThreadID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Country_CountryID",
                table: "User",
                column: "CountryID",
                principalTable: "Country",
                principalColumn: "CountryID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageThreadMessage_HouseMember_HouseMemberID",
                table: "MessageThreadMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageThreadMessage_MessageThread_MessageThreadID",
                table: "MessageThreadMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Country_CountryID",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_CountryID",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_MessageThreadMessage_HouseMemberID",
                table: "MessageThreadMessage");

            migrationBuilder.DropIndex(
                name: "IX_MessageThreadMessage_MessageThreadID",
                table: "MessageThreadMessage");

            migrationBuilder.AddColumn<string>(
                name: "PasswordSalt",
                table: "User",
                nullable: true);
        }
    }
}
