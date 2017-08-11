using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HomeNetAPI.Migrations
{
    public partial class AberdareMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HousePostComment",
                columns: table => new
                {
                    HousePostCommentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: false),
                    DatePosted = table.Column<string>(nullable: false),
                    HouseMemberID = table.Column<int>(nullable: false),
                    HousePostID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    IsFlagged = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousePostComment", x => x.HousePostCommentID);
                    table.ForeignKey(
                        name: "FK_HousePostComment_HouseMember_HouseMemberID",
                        column: x => x.HouseMemberID,
                        principalTable: "HouseMember",
                        principalColumn: "HouseMemberID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HousePostComment_HousePost_HousePostID",
                        column: x => x.HousePostID,
                        principalTable: "HousePost",
                        principalColumn: "HousePostID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HousePostComment_HouseMemberID",
                table: "HousePostComment",
                column: "HouseMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_HousePostComment_HousePostID",
                table: "HousePostComment",
                column: "HousePostID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HousePostComment");
        }
    }
}
