using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeNetAPI.Migrations
{
    public partial class JuneMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MessageThreadParticipant_HouseMemberID",
                table: "MessageThreadParticipant",
                column: "HouseMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageThreadParticipant_MessageThreadID",
                table: "MessageThreadParticipant",
                column: "MessageThreadID");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageThreadParticipant_HouseMember_HouseMemberID",
                table: "MessageThreadParticipant",
                column: "HouseMemberID",
                principalTable: "HouseMember",
                principalColumn: "HouseMemberID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageThreadParticipant_MessageThread_MessageThreadID",
                table: "MessageThreadParticipant",
                column: "MessageThreadID",
                principalTable: "MessageThread",
                principalColumn: "MessageThreadID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageThreadParticipant_HouseMember_HouseMemberID",
                table: "MessageThreadParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageThreadParticipant_MessageThread_MessageThreadID",
                table: "MessageThreadParticipant");

            migrationBuilder.DropIndex(
                name: "IX_MessageThreadParticipant_HouseMemberID",
                table: "MessageThreadParticipant");

            migrationBuilder.DropIndex(
                name: "IX_MessageThreadParticipant_MessageThreadID",
                table: "MessageThreadParticipant");
        }
    }
}
