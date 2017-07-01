using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HomeNetAPI.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsDeleted = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    CountryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsDeleted = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.CountryID);
                });

            migrationBuilder.CreateTable(
                name: "Key",
                columns: table => new
                {
                    KeyID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Key", x => x.KeyID);
                });

            migrationBuilder.CreateTable(
                name: "MessageThreadMessage",
                columns: table => new
                {
                    MessageThreadMessageID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateSent = table.Column<string>(nullable: false),
                    HouseMemberID = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: false),
                    MessageThreadID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageThreadMessage", x => x.MessageThreadMessageID);
                });

            migrationBuilder.CreateTable(
                name: "MessageThreadParticipant",
                columns: table => new
                {
                    MessageThreadParticipantID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HouseMemberID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    MessageThreadID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageThreadParticipant", x => x.MessageThreadParticipantID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    CountryID = table.Column<int>(nullable: false),
                    DateOfBirth = table.Column<string>(nullable: false),
                    DateRegistered = table.Column<string>(nullable: false),
                    Email = table.Column<string>(maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    FacebookID = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Password = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    PasswordSalt = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    ProfileImage = table.Column<string>(nullable: true),
                    SecurityAnswer = table.Column<string>(nullable: true),
                    SecurityQuestion = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    SkypeID = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: false),
                    TwitterID = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "House",
                columns: table => new
                {
                    HouseID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    HouseImage = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    OwnerID = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_House", x => x.HouseID);
                    table.ForeignKey(
                        name: "FK_House_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    OrganizationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryID = table.Column<int>(nullable: false),
                    DateAdded = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: true),
                    FacebookID = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    OrganizationPhoto = table.Column<string>(nullable: true),
                    SkypeID = table.Column<string>(nullable: true),
                    TelephoneNumber = table.Column<string>(nullable: true),
                    TwitterID = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.OrganizationID);
                    table.ForeignKey(
                        name: "FK_Organization_Category_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organization_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCallLog",
                columns: table => new
                {
                    CallLogID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CallEndTime = table.Column<string>(nullable: false),
                    CallStartTime = table.Column<string>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCallLog", x => x.CallLogID);
                    table.ForeignKey(
                        name: "FK_UserCallLog_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HouseMember",
                columns: table => new
                {
                    HouseMemberID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApprovalStatus = table.Column<int>(nullable: false),
                    DateApplied = table.Column<string>(nullable: false),
                    DateApproved = table.Column<string>(nullable: true),
                    DateLeft = table.Column<string>(nullable: true),
                    HouseID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseMember", x => x.HouseMemberID);
                    table.ForeignKey(
                        name: "FK_HouseMember_House_HouseID",
                        column: x => x.HouseID,
                        principalTable: "House",
                        principalColumn: "HouseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseMember_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterHouseMail",
                columns: table => new
                {
                    InterHouseMailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateSent = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Document = table.Column<string>(nullable: true),
                    HouseID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    SenderID = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterHouseMail", x => x.InterHouseMailID);
                    table.ForeignKey(
                        name: "FK_InterHouseMail_House_HouseID",
                        column: x => x.HouseID,
                        principalTable: "House",
                        principalColumn: "HouseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterHouseMail_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPost",
                columns: table => new
                {
                    OrganizationPostID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateAdded = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    OrganizationID = table.Column<int>(nullable: false),
                    PostImage = table.Column<string>(nullable: true),
                    PostText = table.Column<string>(nullable: false),
                    PostVoiceNote = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPost", x => x.OrganizationPostID);
                    table.ForeignKey(
                        name: "FK_OrganizationPost_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationSubscriber",
                columns: table => new
                {
                    OrganizationSubscriberID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateAdded = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    OrganizationID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationSubscriber", x => x.OrganizationSubscriberID);
                    table.ForeignKey(
                        name: "FK_OrganizationSubscriber_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationSubscriber_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CallLogRecepients",
                columns: table => new
                {
                    CallLogRecepientID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CallLogID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallLogRecepients", x => x.CallLogRecepientID);
                    table.ForeignKey(
                        name: "FK_CallLogRecepients_UserCallLog_CallLogID",
                        column: x => x.CallLogID,
                        principalTable: "UserCallLog",
                        principalColumn: "CallLogID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CallLogRecepients_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnnouncementFlaggedComment",
                columns: table => new
                {
                    FlaggedCommentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateFlagged = table.Column<string>(nullable: false),
                    HouseMemberID = table.Column<int>(nullable: false),
                    Reason = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementFlaggedComment", x => x.FlaggedCommentID);
                    table.ForeignKey(
                        name: "FK_AnnouncementFlaggedComment_HouseMember_HouseMemberID",
                        column: x => x.HouseMemberID,
                        principalTable: "HouseMember",
                        principalColumn: "HouseMemberID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HouseAnnouncement",
                columns: table => new
                {
                    HouseAnnouncementID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateAdded = table.Column<string>(nullable: false),
                    HouseID = table.Column<int>(nullable: false),
                    HouseMemberID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseAnnouncement", x => x.HouseAnnouncementID);
                    table.ForeignKey(
                        name: "FK_HouseAnnouncement_House_HouseID",
                        column: x => x.HouseID,
                        principalTable: "House",
                        principalColumn: "HouseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HouseAnnouncement_HouseMember_HouseMemberID",
                        column: x => x.HouseMemberID,
                        principalTable: "HouseMember",
                        principalColumn: "HouseMemberID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HouseGallery",
                columns: table => new
                {
                    HouseGalleryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    HouseMemberID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseGallery", x => x.HouseGalleryID);
                    table.ForeignKey(
                        name: "FK_HouseGallery_HouseMember_HouseMemberID",
                        column: x => x.HouseMemberID,
                        principalTable: "HouseMember",
                        principalColumn: "HouseMemberID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HousePost",
                columns: table => new
                {
                    HousePostID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatePosted = table.Column<string>(nullable: false),
                    HouseMemberID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: false),
                    PostText = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousePost", x => x.HousePostID);
                    table.ForeignKey(
                        name: "FK_HousePost_HouseMember_HouseMemberID",
                        column: x => x.HouseMemberID,
                        principalTable: "HouseMember",
                        principalColumn: "HouseMemberID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageThread",
                columns: table => new
                {
                    MessageThreadID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HouseMemberID = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageThread", x => x.MessageThreadID);
                    table.ForeignKey(
                        name: "FK_MessageThread_HouseMember_HouseMemberID",
                        column: x => x.HouseMemberID,
                        principalTable: "HouseMember",
                        principalColumn: "HouseMemberID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterHouseMessageReply",
                columns: table => new
                {
                    InterHouseMessageReplyID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: false),
                    DateAdded = table.Column<string>(nullable: false),
                    InterHouseMailID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterHouseMessageReply", x => x.InterHouseMessageReplyID);
                    table.ForeignKey(
                        name: "FK_InterHouseMessageReply_InterHouseMail_InterHouseMailID",
                        column: x => x.InterHouseMailID,
                        principalTable: "InterHouseMail",
                        principalColumn: "InterHouseMailID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterHouseMessageReply_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPostMetaData",
                columns: table => new
                {
                    OrganizationPostMetaDataID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateRecorded = table.Column<string>(nullable: false),
                    Dislikes = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    Likes = table.Column<int>(nullable: false),
                    OrganizationPostID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPostMetaData", x => x.OrganizationPostMetaDataID);
                    table.ForeignKey(
                        name: "FK_OrganizationPostMetaData_OrganizationPost_OrganizationPostID",
                        column: x => x.OrganizationPostID,
                        principalTable: "OrganizationPost",
                        principalColumn: "OrganizationPostID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPostMetaData_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnnouncementComment",
                columns: table => new
                {
                    AnnouncementCommentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: false),
                    DateAdded = table.Column<string>(nullable: false),
                    HouseAnnouncementID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    IsFlagged = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementComment", x => x.AnnouncementCommentID);
                    table.ForeignKey(
                        name: "FK_AnnouncementComment_HouseAnnouncement_HouseAnnouncementID",
                        column: x => x.HouseAnnouncementID,
                        principalTable: "HouseAnnouncement",
                        principalColumn: "HouseAnnouncementID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GalleryImage",
                columns: table => new
                {
                    GalleryImageID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateAdded = table.Column<string>(nullable: false),
                    HouseGalleryID = table.Column<int>(nullable: false),
                    ImageURL = table.Column<string>(nullable: false),
                    Keywords = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryImage", x => x.GalleryImageID);
                    table.ForeignKey(
                        name: "FK_GalleryImage_HouseGallery_HouseGalleryID",
                        column: x => x.HouseGalleryID,
                        principalTable: "HouseGallery",
                        principalColumn: "HouseGalleryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HousePostFlag",
                columns: table => new
                {
                    HousePostFlagID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateFlagged = table.Column<string>(nullable: false),
                    HouseMemberID = table.Column<int>(nullable: false),
                    HousePostID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousePostFlag", x => x.HousePostFlagID);
                    table.ForeignKey(
                        name: "FK_HousePostFlag_HouseMember_HouseMemberID",
                        column: x => x.HouseMemberID,
                        principalTable: "HouseMember",
                        principalColumn: "HouseMemberID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HousePostFlag_HousePost_HousePostID",
                        column: x => x.HousePostID,
                        principalTable: "HousePost",
                        principalColumn: "HousePostID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HousePostMetaData",
                columns: table => new
                {
                    HousePostMetaDataID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateAdded = table.Column<string>(nullable: false),
                    Disliked = table.Column<int>(nullable: false),
                    HousePostID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    Liked = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousePostMetaData", x => x.HousePostMetaDataID);
                    table.ForeignKey(
                        name: "FK_HousePostMetaData_HousePost_HousePostID",
                        column: x => x.HousePostID,
                        principalTable: "HousePost",
                        principalColumn: "HousePostID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HousePostMetaData_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FlaggedComment",
                columns: table => new
                {
                    ReportID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnnouncementCommentID = table.Column<int>(nullable: false),
                    DateReported = table.Column<string>(nullable: false),
                    DealtMessage = table.Column<string>(nullable: true),
                    IsDealtWith = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlaggedComment", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK_FlaggedComment_AnnouncementComment_AnnouncementCommentID",
                        column: x => x.AnnouncementCommentID,
                        principalTable: "AnnouncementComment",
                        principalColumn: "AnnouncementCommentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GalleryImageComment",
                columns: table => new
                {
                    GalleryImageCommentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: false),
                    DateAdded = table.Column<string>(nullable: false),
                    Dislikes = table.Column<int>(nullable: false),
                    GalleryImageID = table.Column<int>(nullable: false),
                    HouseMemberID = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    IsFlagged = table.Column<int>(nullable: false),
                    Likes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryImageComment", x => x.GalleryImageCommentID);
                    table.ForeignKey(
                        name: "FK_GalleryImageComment_GalleryImage_GalleryImageID",
                        column: x => x.GalleryImageID,
                        principalTable: "GalleryImage",
                        principalColumn: "GalleryImageID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GalleryImageComment_HouseMember_HouseMemberID",
                        column: x => x.HouseMemberID,
                        principalTable: "HouseMember",
                        principalColumn: "HouseMemberID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GalleryImageCommentFlag",
                columns: table => new
                {
                    ReportID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateFlagged = table.Column<string>(nullable: false),
                    GalleryImageCommentID = table.Column<int>(nullable: false),
                    HouseMemberID = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryImageCommentFlag", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK_GalleryImageCommentFlag_GalleryImageComment_GalleryImageCommentID",
                        column: x => x.GalleryImageCommentID,
                        principalTable: "GalleryImageComment",
                        principalColumn: "GalleryImageCommentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GalleryImageCommentFlag_HouseMember_HouseMemberID",
                        column: x => x.HouseMemberID,
                        principalTable: "HouseMember",
                        principalColumn: "HouseMemberID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GalleryImageCommentRemoval",
                columns: table => new
                {
                    GalleryImageCommentRemovalID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateRemoved = table.Column<string>(nullable: false),
                    GalleryImageCommentID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryImageCommentRemoval", x => x.GalleryImageCommentRemovalID);
                    table.ForeignKey(
                        name: "FK_GalleryImageCommentRemoval_GalleryImageComment_GalleryImageCommentID",
                        column: x => x.GalleryImageCommentID,
                        principalTable: "GalleryImageComment",
                        principalColumn: "GalleryImageCommentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GalleryImageCommentRemoval_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementComment_HouseAnnouncementID",
                table: "AnnouncementComment",
                column: "HouseAnnouncementID");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementFlaggedComment_HouseMemberID",
                table: "AnnouncementFlaggedComment",
                column: "HouseMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_CallLogRecepients_CallLogID",
                table: "CallLogRecepients",
                column: "CallLogID");

            migrationBuilder.CreateIndex(
                name: "IX_CallLogRecepients_UserID",
                table: "CallLogRecepients",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_FlaggedComment_AnnouncementCommentID",
                table: "FlaggedComment",
                column: "AnnouncementCommentID");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImage_HouseGalleryID",
                table: "GalleryImage",
                column: "HouseGalleryID");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageComment_GalleryImageID",
                table: "GalleryImageComment",
                column: "GalleryImageID");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageComment_HouseMemberID",
                table: "GalleryImageComment",
                column: "HouseMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageCommentFlag_GalleryImageCommentID",
                table: "GalleryImageCommentFlag",
                column: "GalleryImageCommentID");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageCommentFlag_HouseMemberID",
                table: "GalleryImageCommentFlag",
                column: "HouseMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageCommentRemoval_GalleryImageCommentID",
                table: "GalleryImageCommentRemoval",
                column: "GalleryImageCommentID");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageCommentRemoval_UserID",
                table: "GalleryImageCommentRemoval",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_House_UserId",
                table: "House",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseAnnouncement_HouseID",
                table: "HouseAnnouncement",
                column: "HouseID");

            migrationBuilder.CreateIndex(
                name: "IX_HouseAnnouncement_HouseMemberID",
                table: "HouseAnnouncement",
                column: "HouseMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_HouseGallery_HouseMemberID",
                table: "HouseGallery",
                column: "HouseMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_HouseMember_HouseID",
                table: "HouseMember",
                column: "HouseID");

            migrationBuilder.CreateIndex(
                name: "IX_HouseMember_UserID",
                table: "HouseMember",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_HousePost_HouseMemberID",
                table: "HousePost",
                column: "HouseMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_HousePostFlag_HouseMemberID",
                table: "HousePostFlag",
                column: "HouseMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_HousePostFlag_HousePostID",
                table: "HousePostFlag",
                column: "HousePostID");

            migrationBuilder.CreateIndex(
                name: "IX_HousePostMetaData_HousePostID",
                table: "HousePostMetaData",
                column: "HousePostID");

            migrationBuilder.CreateIndex(
                name: "IX_HousePostMetaData_UserID",
                table: "HousePostMetaData",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_InterHouseMail_HouseID",
                table: "InterHouseMail",
                column: "HouseID");

            migrationBuilder.CreateIndex(
                name: "IX_InterHouseMail_UserId",
                table: "InterHouseMail",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterHouseMessageReply_InterHouseMailID",
                table: "InterHouseMessageReply",
                column: "InterHouseMailID");

            migrationBuilder.CreateIndex(
                name: "IX_InterHouseMessageReply_UserID",
                table: "InterHouseMessageReply",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageThread_HouseMemberID",
                table: "MessageThread",
                column: "HouseMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_CategoryID",
                table: "Organization",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_UserID",
                table: "Organization",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPost_OrganizationID",
                table: "OrganizationPost",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPostMetaData_OrganizationPostID",
                table: "OrganizationPostMetaData",
                column: "OrganizationPostID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPostMetaData_UserID",
                table: "OrganizationPostMetaData",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSubscriber_OrganizationID",
                table: "OrganizationSubscriber",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSubscriber_UserID",
                table: "OrganizationSubscriber",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCallLog_UserID",
                table: "UserCallLog",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnouncementFlaggedComment");

            migrationBuilder.DropTable(
                name: "CallLogRecepients");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "FlaggedComment");

            migrationBuilder.DropTable(
                name: "GalleryImageCommentFlag");

            migrationBuilder.DropTable(
                name: "GalleryImageCommentRemoval");

            migrationBuilder.DropTable(
                name: "HousePostFlag");

            migrationBuilder.DropTable(
                name: "HousePostMetaData");

            migrationBuilder.DropTable(
                name: "InterHouseMessageReply");

            migrationBuilder.DropTable(
                name: "Key");

            migrationBuilder.DropTable(
                name: "MessageThread");

            migrationBuilder.DropTable(
                name: "MessageThreadMessage");

            migrationBuilder.DropTable(
                name: "MessageThreadParticipant");

            migrationBuilder.DropTable(
                name: "OrganizationPostMetaData");

            migrationBuilder.DropTable(
                name: "OrganizationSubscriber");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "UserCallLog");

            migrationBuilder.DropTable(
                name: "AnnouncementComment");

            migrationBuilder.DropTable(
                name: "GalleryImageComment");

            migrationBuilder.DropTable(
                name: "HousePost");

            migrationBuilder.DropTable(
                name: "InterHouseMail");

            migrationBuilder.DropTable(
                name: "OrganizationPost");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "HouseAnnouncement");

            migrationBuilder.DropTable(
                name: "GalleryImage");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "HouseGallery");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "HouseMember");

            migrationBuilder.DropTable(
                name: "House");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
