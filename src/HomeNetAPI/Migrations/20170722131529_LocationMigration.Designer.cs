using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using HomeNetAPI.Repository;

namespace HomeNetAPI.Migrations
{
    [DbContext(typeof(HomeNetContext))]
    [Migration("20170722131529_LocationMigration")]
    partial class LocationMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HomeNetAPI.Models.AnnouncementComment", b =>
                {
                    b.Property<int>("AnnouncementCommentID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comment")
                        .IsRequired();

                    b.Property<string>("DateAdded")
                        .IsRequired();

                    b.Property<int>("HouseAnnouncementID");

                    b.Property<int>("HouseMemberID");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("IsFlagged");

                    b.HasKey("AnnouncementCommentID");

                    b.HasIndex("HouseAnnouncementID");

                    b.HasIndex("HouseMemberID");

                    b.ToTable("AnnouncementComment");
                });

            modelBuilder.Entity("HomeNetAPI.Models.AnnouncementFlaggedComment", b =>
                {
                    b.Property<int>("FlaggedCommentID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateFlagged")
                        .IsRequired();

                    b.Property<int>("HouseMemberID");

                    b.Property<string>("Reason")
                        .IsRequired();

                    b.HasKey("FlaggedCommentID");

                    b.HasIndex("HouseMemberID");

                    b.ToTable("AnnouncementFlaggedComment");
                });

            modelBuilder.Entity("HomeNetAPI.Models.CallLogRecepients", b =>
                {
                    b.Property<int>("CallLogRecepientID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CallLogID");

                    b.Property<int>("UserID");

                    b.HasKey("CallLogRecepientID");

                    b.HasIndex("CallLogID");

                    b.HasIndex("UserID");

                    b.ToTable("CallLogRecepients");
                });

            modelBuilder.Entity("HomeNetAPI.Models.Category", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("CategoryID");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("HomeNetAPI.Models.Country", b =>
                {
                    b.Property<int>("CountryID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("CountryID");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("HomeNetAPI.Models.DialingCode", b =>
                {
                    b.Property<int>("DialingCodeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<int>("CountryID");

                    b.Property<int>("IsDeleted");

                    b.HasKey("DialingCodeID");

                    b.HasIndex("CountryID");

                    b.ToTable("DialingCode");
                });

            modelBuilder.Entity("HomeNetAPI.Models.FlaggedComment", b =>
                {
                    b.Property<int>("ReportID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AnnouncementCommentID");

                    b.Property<string>("DateReported")
                        .IsRequired();

                    b.Property<string>("DealtMessage");

                    b.Property<int>("IsDealtWith");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.HasKey("ReportID");

                    b.HasIndex("AnnouncementCommentID");

                    b.ToTable("FlaggedComment");
                });

            modelBuilder.Entity("HomeNetAPI.Models.GalleryImage", b =>
                {
                    b.Property<int>("GalleryImageID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateAdded")
                        .IsRequired();

                    b.Property<int>("HouseGalleryID");

                    b.Property<string>("ImageURL")
                        .IsRequired();

                    b.Property<string>("Keywords");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("GalleryImageID");

                    b.HasIndex("HouseGalleryID");

                    b.ToTable("GalleryImage");
                });

            modelBuilder.Entity("HomeNetAPI.Models.GalleryImageComment", b =>
                {
                    b.Property<int>("GalleryImageCommentID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comment")
                        .IsRequired();

                    b.Property<string>("DateAdded")
                        .IsRequired();

                    b.Property<int>("Dislikes");

                    b.Property<int>("GalleryImageID");

                    b.Property<int>("HouseMemberID");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("IsFlagged");

                    b.Property<int>("Likes");

                    b.HasKey("GalleryImageCommentID");

                    b.HasIndex("GalleryImageID");

                    b.HasIndex("HouseMemberID");

                    b.ToTable("GalleryImageComment");
                });

            modelBuilder.Entity("HomeNetAPI.Models.GalleryImageCommentFlag", b =>
                {
                    b.Property<int>("ReportID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateFlagged")
                        .IsRequired();

                    b.Property<int>("GalleryImageCommentID");

                    b.Property<int>("HouseMemberID");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.HasKey("ReportID");

                    b.HasIndex("GalleryImageCommentID");

                    b.HasIndex("HouseMemberID");

                    b.ToTable("GalleryImageCommentFlag");
                });

            modelBuilder.Entity("HomeNetAPI.Models.GalleryImageCommentRemoval", b =>
                {
                    b.Property<int>("GalleryImageCommentRemovalID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateRemoved")
                        .IsRequired();

                    b.Property<int>("GalleryImageCommentID");

                    b.Property<int>("UserID");

                    b.HasKey("GalleryImageCommentRemovalID");

                    b.HasIndex("GalleryImageCommentID");

                    b.HasIndex("UserID");

                    b.ToTable("GalleryImageCommentRemoval");
                });

            modelBuilder.Entity("HomeNetAPI.Models.House", b =>
                {
                    b.Property<int>("HouseID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateCreated");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("HouseImage");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("IsPrivate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("OneTimePin");

                    b.Property<int>("UserID");

                    b.HasKey("HouseID");

                    b.HasIndex("UserID");

                    b.ToTable("House");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HouseAnnouncement", b =>
                {
                    b.Property<int>("HouseAnnouncementID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateAdded")
                        .IsRequired();

                    b.Property<int>("HouseID");

                    b.Property<int>("HouseMemberID");

                    b.Property<int>("IsDeleted");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("HouseAnnouncementID");

                    b.HasIndex("HouseID");

                    b.HasIndex("HouseMemberID");

                    b.ToTable("HouseAnnouncement");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HouseGallery", b =>
                {
                    b.Property<int>("HouseGalleryID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateCreated")
                        .IsRequired();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<int>("HouseMemberID");

                    b.Property<int>("IsDeleted");

                    b.Property<string>("Location")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("HouseGalleryID");

                    b.HasIndex("HouseMemberID");

                    b.ToTable("HouseGallery");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HouseMember", b =>
                {
                    b.Property<int>("HouseMemberID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApprovalStatus");

                    b.Property<string>("DateApplied")
                        .IsRequired();

                    b.Property<string>("DateApproved");

                    b.Property<string>("DateLeft");

                    b.Property<int>("HouseID");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("UserID");

                    b.HasKey("HouseMemberID");

                    b.HasIndex("HouseID");

                    b.HasIndex("UserID");

                    b.ToTable("HouseMember");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HousePost", b =>
                {
                    b.Property<int>("HousePostID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DatePosted")
                        .IsRequired();

                    b.Property<int>("HouseMemberID");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("IsFlagged");

                    b.Property<string>("Location");

                    b.Property<string>("MediaResource");

                    b.Property<string>("PostText")
                        .IsRequired();

                    b.Property<string>("ResizedMediaResource");

                    b.Property<string>("Title");

                    b.HasKey("HousePostID");

                    b.HasIndex("HouseMemberID");

                    b.ToTable("HousePost");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HousePostFlag", b =>
                {
                    b.Property<int>("HousePostFlagID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateFlagged")
                        .IsRequired();

                    b.Property<int>("HouseMemberID");

                    b.Property<int>("HousePostID");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("IsFlagged");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<string>("ResponseMessage");

                    b.HasKey("HousePostFlagID");

                    b.HasIndex("HouseMemberID");

                    b.HasIndex("HousePostID");

                    b.ToTable("HousePostFlag");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HousePostMetaData", b =>
                {
                    b.Property<int>("HousePostMetaDataID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateAdded")
                        .IsRequired();

                    b.Property<int>("Disliked");

                    b.Property<int>("HousePostID");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("Liked");

                    b.Property<int>("UserID");

                    b.HasKey("HousePostMetaDataID");

                    b.HasIndex("HousePostID");

                    b.HasIndex("UserID");

                    b.ToTable("HousePostMetaData");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HouseProfileImage", b =>
                {
                    b.Property<int>("HouseProfileImageID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateAdded")
                        .IsRequired();

                    b.Property<int>("HouseID");

                    b.Property<string>("HouseImage")
                        .IsRequired();

                    b.Property<int>("IsDeleted");

                    b.HasKey("HouseProfileImageID");

                    b.HasIndex("HouseID");

                    b.ToTable("HouseProfileImage");
                });

            modelBuilder.Entity("HomeNetAPI.Models.InterHouseMail", b =>
                {
                    b.Property<int>("InterHouseMailID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateSent")
                        .IsRequired();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Document");

                    b.Property<int>("HouseID");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("SenderID");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int?>("UserId");

                    b.HasKey("InterHouseMailID");

                    b.HasIndex("HouseID");

                    b.HasIndex("UserId");

                    b.ToTable("InterHouseMail");
                });

            modelBuilder.Entity("HomeNetAPI.Models.InterHouseMessageReply", b =>
                {
                    b.Property<int>("InterHouseMessageReplyID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comment")
                        .IsRequired();

                    b.Property<string>("DateAdded")
                        .IsRequired();

                    b.Property<int>("InterHouseMailID");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("UserID");

                    b.HasKey("InterHouseMessageReplyID");

                    b.HasIndex("InterHouseMailID");

                    b.HasIndex("UserID");

                    b.ToTable("InterHouseMessageReply");
                });

            modelBuilder.Entity("HomeNetAPI.Models.Key", b =>
                {
                    b.Property<int>("KeyID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<int>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("KeyID");

                    b.ToTable("Key");
                });

            modelBuilder.Entity("HomeNetAPI.Models.MessageThread", b =>
                {
                    b.Property<int>("MessageThreadID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("HouseMemberID");

                    b.Property<int>("Priority");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("MessageThreadID");

                    b.HasIndex("HouseMemberID");

                    b.ToTable("MessageThread");
                });

            modelBuilder.Entity("HomeNetAPI.Models.MessageThreadMessage", b =>
                {
                    b.Property<int>("MessageThreadMessageID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateSent")
                        .IsRequired();

                    b.Property<int>("HouseMemberID");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<int>("MessageThreadID");

                    b.HasKey("MessageThreadMessageID");

                    b.HasIndex("HouseMemberID");

                    b.HasIndex("MessageThreadID");

                    b.ToTable("MessageThreadMessage");
                });

            modelBuilder.Entity("HomeNetAPI.Models.MessageThreadParticipant", b =>
                {
                    b.Property<int>("MessageThreadParticipantID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("HouseMemberID");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("MessageThreadID");

                    b.HasKey("MessageThreadParticipantID");

                    b.HasIndex("HouseMemberID");

                    b.HasIndex("MessageThreadID");

                    b.ToTable("MessageThreadParticipant");
                });

            modelBuilder.Entity("HomeNetAPI.Models.Organization", b =>
                {
                    b.Property<int>("OrganizationID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryID");

                    b.Property<string>("DateAdded")
                        .IsRequired();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("EmailAddress");

                    b.Property<string>("FacebookID");

                    b.Property<int>("IsDeleted");

                    b.Property<string>("Location")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("OrganizationPhoto");

                    b.Property<string>("SkypeID");

                    b.Property<string>("TelephoneNumber");

                    b.Property<string>("TwitterID");

                    b.Property<int>("UserID");

                    b.HasKey("OrganizationID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("UserID");

                    b.ToTable("Organization");
                });

            modelBuilder.Entity("HomeNetAPI.Models.OrganizationPost", b =>
                {
                    b.Property<int>("OrganizationPostID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateAdded")
                        .IsRequired();

                    b.Property<int>("IsDeleted");

                    b.Property<int>("OrganizationID");

                    b.Property<string>("PostImage");

                    b.Property<string>("PostText")
                        .IsRequired();

                    b.Property<string>("PostVoiceNote");

                    b.HasKey("OrganizationPostID");

                    b.HasIndex("OrganizationID");

                    b.ToTable("OrganizationPost");
                });

            modelBuilder.Entity("HomeNetAPI.Models.OrganizationPostMetaData", b =>
                {
                    b.Property<int>("OrganizationPostMetaDataID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateRecorded")
                        .IsRequired();

                    b.Property<int>("Dislikes");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("Likes");

                    b.Property<int>("OrganizationPostID");

                    b.Property<int>("UserID");

                    b.HasKey("OrganizationPostMetaDataID");

                    b.HasIndex("OrganizationPostID");

                    b.HasIndex("UserID");

                    b.ToTable("OrganizationPostMetaData");
                });

            modelBuilder.Entity("HomeNetAPI.Models.OrganizationSubscriber", b =>
                {
                    b.Property<int>("OrganizationSubscriberID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DateAdded")
                        .IsRequired();

                    b.Property<int>("IsDeleted");

                    b.Property<int>("OrganizationID");

                    b.Property<int>("UserID");

                    b.HasKey("OrganizationSubscriberID");

                    b.HasIndex("OrganizationID");

                    b.HasIndex("UserID");

                    b.ToTable("OrganizationSubscriber");
                });

            modelBuilder.Entity("HomeNetAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<int>("CountryID");

                    b.Property<string>("DateOfBirth")
                        .IsRequired();

                    b.Property<string>("DateRegistered")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FacebookID");

                    b.Property<string>("FirebaseMessagingToken");

                    b.Property<string>("Gender")
                        .IsRequired();

                    b.Property<int>("IsDeleted");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("ProfileImage");

                    b.Property<string>("SecurityAnswer");

                    b.Property<string>("SecurityQuestion");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("SkypeID");

                    b.Property<string>("Surname")
                        .IsRequired();

                    b.Property<string>("TwitterID");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("CountryID");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("User");
                });

            modelBuilder.Entity("HomeNetAPI.Models.UserCallLog", b =>
                {
                    b.Property<int>("CallLogID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CallEndTime")
                        .IsRequired();

                    b.Property<string>("CallStartTime")
                        .IsRequired();

                    b.Property<int>("UserID");

                    b.HasKey("CallLogID");

                    b.HasIndex("UserID");

                    b.ToTable("UserCallLog");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("HomeNetAPI.Models.AnnouncementComment", b =>
                {
                    b.HasOne("HomeNetAPI.Models.HouseAnnouncement", "HouseAnnouncement")
                        .WithMany()
                        .HasForeignKey("HouseAnnouncementID");

                    b.HasOne("HomeNetAPI.Models.HouseMember", "HouseMember")
                        .WithMany()
                        .HasForeignKey("HouseMemberID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.AnnouncementFlaggedComment", b =>
                {
                    b.HasOne("HomeNetAPI.Models.HouseMember", "HouseMember")
                        .WithMany()
                        .HasForeignKey("HouseMemberID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HomeNetAPI.Models.CallLogRecepients", b =>
                {
                    b.HasOne("HomeNetAPI.Models.UserCallLog", "UserCallLog")
                        .WithMany()
                        .HasForeignKey("CallLogID");

                    b.HasOne("HomeNetAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.DialingCode", b =>
                {
                    b.HasOne("HomeNetAPI.Models.Country", "Country")
                        .WithMany("DialingCodes")
                        .HasForeignKey("CountryID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.FlaggedComment", b =>
                {
                    b.HasOne("HomeNetAPI.Models.AnnouncementComment", "AnnouncementComment")
                        .WithMany()
                        .HasForeignKey("AnnouncementCommentID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.GalleryImage", b =>
                {
                    b.HasOne("HomeNetAPI.Models.HouseGallery", "HouseGallery")
                        .WithMany()
                        .HasForeignKey("HouseGalleryID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.GalleryImageComment", b =>
                {
                    b.HasOne("HomeNetAPI.Models.GalleryImage", "GalleryImage")
                        .WithMany()
                        .HasForeignKey("GalleryImageID");

                    b.HasOne("HomeNetAPI.Models.HouseMember", "HouseMember")
                        .WithMany()
                        .HasForeignKey("HouseMemberID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.GalleryImageCommentFlag", b =>
                {
                    b.HasOne("HomeNetAPI.Models.GalleryImageComment", "GalleryImageComment")
                        .WithMany()
                        .HasForeignKey("GalleryImageCommentID");

                    b.HasOne("HomeNetAPI.Models.HouseMember", "HouseMember")
                        .WithMany()
                        .HasForeignKey("HouseMemberID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.GalleryImageCommentRemoval", b =>
                {
                    b.HasOne("HomeNetAPI.Models.GalleryImageComment", "GalleryImageComment")
                        .WithMany()
                        .HasForeignKey("GalleryImageCommentID");

                    b.HasOne("HomeNetAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.House", b =>
                {
                    b.HasOne("HomeNetAPI.Models.User", "User")
                        .WithMany("Houses")
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HouseAnnouncement", b =>
                {
                    b.HasOne("HomeNetAPI.Models.House", "House")
                        .WithMany()
                        .HasForeignKey("HouseID");

                    b.HasOne("HomeNetAPI.Models.HouseMember", "HouseMember")
                        .WithMany()
                        .HasForeignKey("HouseMemberID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HouseGallery", b =>
                {
                    b.HasOne("HomeNetAPI.Models.HouseMember", "HouseMember")
                        .WithMany()
                        .HasForeignKey("HouseMemberID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HouseMember", b =>
                {
                    b.HasOne("HomeNetAPI.Models.House", "House")
                        .WithMany()
                        .HasForeignKey("HouseID");

                    b.HasOne("HomeNetAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HousePost", b =>
                {
                    b.HasOne("HomeNetAPI.Models.HouseMember", "HouseMember")
                        .WithMany()
                        .HasForeignKey("HouseMemberID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HousePostFlag", b =>
                {
                    b.HasOne("HomeNetAPI.Models.HouseMember", "HouseMember")
                        .WithMany()
                        .HasForeignKey("HouseMemberID");

                    b.HasOne("HomeNetAPI.Models.HousePost", "HousePost")
                        .WithMany()
                        .HasForeignKey("HousePostID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HousePostMetaData", b =>
                {
                    b.HasOne("HomeNetAPI.Models.HousePost", "HousePost")
                        .WithMany()
                        .HasForeignKey("HousePostID");

                    b.HasOne("HomeNetAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.HouseProfileImage", b =>
                {
                    b.HasOne("HomeNetAPI.Models.House", "House")
                        .WithMany("HouseProfileImages")
                        .HasForeignKey("HouseID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.InterHouseMail", b =>
                {
                    b.HasOne("HomeNetAPI.Models.House", "House")
                        .WithMany()
                        .HasForeignKey("HouseID");

                    b.HasOne("HomeNetAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("HomeNetAPI.Models.InterHouseMessageReply", b =>
                {
                    b.HasOne("HomeNetAPI.Models.InterHouseMail", "InterHouseMail")
                        .WithMany()
                        .HasForeignKey("InterHouseMailID");

                    b.HasOne("HomeNetAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.MessageThread", b =>
                {
                    b.HasOne("HomeNetAPI.Models.HouseMember", "HouseMember")
                        .WithMany()
                        .HasForeignKey("HouseMemberID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.MessageThreadMessage", b =>
                {
                    b.HasOne("HomeNetAPI.Models.HouseMember", "HouseMember")
                        .WithMany()
                        .HasForeignKey("HouseMemberID");

                    b.HasOne("HomeNetAPI.Models.MessageThread", "MessageThread")
                        .WithMany()
                        .HasForeignKey("MessageThreadID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.MessageThreadParticipant", b =>
                {
                    b.HasOne("HomeNetAPI.Models.HouseMember", "HouseMember")
                        .WithMany()
                        .HasForeignKey("HouseMemberID");

                    b.HasOne("HomeNetAPI.Models.MessageThread", "MessageThread")
                        .WithMany()
                        .HasForeignKey("MessageThreadID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.Organization", b =>
                {
                    b.HasOne("HomeNetAPI.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryID");

                    b.HasOne("HomeNetAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.OrganizationPost", b =>
                {
                    b.HasOne("HomeNetAPI.Models.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.OrganizationPostMetaData", b =>
                {
                    b.HasOne("HomeNetAPI.Models.OrganizationPost", "OrganizationPost")
                        .WithMany()
                        .HasForeignKey("OrganizationPostID");

                    b.HasOne("HomeNetAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.OrganizationSubscriber", b =>
                {
                    b.HasOne("HomeNetAPI.Models.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationID");

                    b.HasOne("HomeNetAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.User", b =>
                {
                    b.HasOne("HomeNetAPI.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryID");
                });

            modelBuilder.Entity("HomeNetAPI.Models.UserCallLog", b =>
                {
                    b.HasOne("HomeNetAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<int>")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("HomeNetAPI.Models.User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("HomeNetAPI.Models.User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<int>")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("HomeNetAPI.Models.User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId");
                });
        }
    }
}
