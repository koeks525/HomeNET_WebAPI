using Microsoft.EntityFrameworkCore;
using HomeNetAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HomeNetAPI.Repository
{
    public class HomeNetContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public HomeNetContext(DbContextOptions<HomeNetContext> options) : base(options)
        {

        }
        //All users of our system
        //public DbSet<User> Users { get; set; }
        //Keep track of all announcement comments
        public DbSet<AnnouncementComment> AnnouncementComments { get; set; }
        public DbSet<Country> Countries { get; set; }
        //Keep track of all the flagged comments
        public DbSet<FlaggedComment> FlaggedComments { get; set; }
        //All images that belong to some photo gallery
        public DbSet<GalleryImage> GalleryImage { get; set; }
        public DbSet<GalleryImageComment> GalleryImageComments { get; set; }
        //Keep track of all the gallery image comments that get flagged
        public DbSet<GalleryImageCommentFlag> GalleryImageCommentFlags { get; set; }
        //Keep track of all the images that were removed and the reasons for them being removed
        public DbSet<GalleryImageCommentRemoval> GalleryImageCommentRemovals { get; set; }
        //Keep track of all houses created by the users
        public DbSet<House> Houses { get; set; }
        //Keep track of all the house announcements
        public DbSet<HouseAnnouncement> HouseAnnouncements { get; set; }
        //Keep track of all the photo galleries in the house
        public DbSet<HouseGallery> HouseGalleries { get; set; }
        //Keep track of all the house members (belonging to a house
        public DbSet<HouseMember> HouseMembers { get; set; }
        //Keep track of all the posts a user makes under a specific house (posts = pictures, voice notes, or text)
        public DbSet<HousePost> HousePosts { get; set; }
        //Keep track track of all flagged posts
        public DbSet<HousePostFlag> FlaggedHousePosts { get; set; }
        //Keep track of all the message "threads" a user of a house creates - a user can have many threads, and a thread can have many people participating
        public DbSet<MessageThread> MessageThreads { get; set; }
        //Keep track of all the users participating in a message thread
        public DbSet<MessageThreadParticipant> MessageThreadParticipants { get; set; }
        //Keep track of all messages in a thread (who wrote the message, etc)
        public DbSet<MessageThreadMessage> MessageThreadMessages { get; set; }
        //Keep track of all categories for businesses (organizations)
        public DbSet<Category> Categories { get; set; }
        //Keep track of all subscribers to many organizations
        public DbSet<OrganizationSubscriber> OrganizationSubscribers { get; set; }
        //Keep track of all the calls a user has made (basic start, end, and the people called)
        public DbSet<UserCallLog> UserCallLogs { get; set; }
        //Keep track of all the recepients in a call - this table isnt really needed for now (will really be useful should we allow calls to have multiple participants)
        public DbSet<CallLogRecepients> CallLogRecepients { get; set; }
        //Keep track of all messages sent to other houses
        public DbSet<InterHouseMail> InterHouseMails { get; set; }
        //Keep track of all the Inter house message replies
        public DbSet<InterHouseMessageReply> InterHouseMessageReplies { get; set; }
        //Keep track of all the posts an organization makes to its subscribers
        public DbSet<OrganizationPost> OrganizationPosts { get; set; }
        //Keep track of all the post meda data (likes, dislikes, who made that meta, etc.)
        public DbSet<OrganizationPostMetaData> OrganizationPostMedaDatas { get; set; }
        //Keep track of all the users that like and dislike a post
        public DbSet<HousePostMetaData> HousePostMetaData { get; set; }
        //keep track of all the keys in the system
        public DbSet<Key> Keys { get; set; }
        //Keep track of past profile images on the houses
        public DbSet<HouseProfileImage> HouseProfileImages { get; set; }
        //Keep track of all organizations on the system
        public DbSet<Organization> Organizations { get; set; }
        //Keep track of all dialing codes for each country
        public DbSet<DialingCode> DialingCodes { get; set; }
        //Keep track of comments
        public DbSet<HousePostComment> HousePostComments { get; set; }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Source: http://stackoverflow.com/questions/34768976/specifying-on-delete-no-action-in-entity-framework-7
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<AnnouncementComment>().ToTable("AnnouncementComment");
            modelBuilder.Entity<AnnouncementFlaggedComment>().ToTable("AnnouncementFlaggedComment");
            modelBuilder.Entity<Country>().ToTable("Country");
            modelBuilder.Entity<FlaggedComment>().ToTable("FlaggedComment");
            modelBuilder.Entity<GalleryImage>().ToTable("GalleryImage");
            modelBuilder.Entity<GalleryImageComment>().ToTable("GalleryImageComment");
            modelBuilder.Entity<GalleryImageCommentFlag>().ToTable("GalleryImageCommentFlag");
            modelBuilder.Entity<GalleryImageCommentRemoval>().ToTable("GalleryImageCommentRemoval");
            modelBuilder.Entity<House>().ToTable("House");
            modelBuilder.Entity<HouseAnnouncement>().ToTable("HouseAnnouncement");
            modelBuilder.Entity<HouseGallery>().ToTable("HouseGallery");
            modelBuilder.Entity<HouseMember>().ToTable("HouseMember");
            modelBuilder.Entity<HousePost>().ToTable("HousePost");
            modelBuilder.Entity<HousePostFlag>().ToTable("HousePostFlag");
            modelBuilder.Entity<MessageThread>().ToTable("MessageThread");
            modelBuilder.Entity<MessageThreadMessage>().ToTable("MessageThreadMessage");
            modelBuilder.Entity<MessageThreadParticipant>().ToTable("MessageThreadParticipant");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Organization>().ToTable("Organization");
            modelBuilder.Entity<OrganizationSubscriber>().ToTable("OrganizationSubscriber");
            //Source: http://stackoverflow.com/questions/41442513/how-can-i-change-default-asp-net-identity-table-names-in-net-core 
            modelBuilder.Entity<User>(entity => { entity.ToTable("User"); });
            modelBuilder.Entity<UserCallLog>().ToTable("UserCallLog");
            modelBuilder.Entity<CallLogRecepients>().ToTable("CallLogRecepients");
            modelBuilder.Entity<InterHouseMail>().ToTable("InterHouseMail");
            modelBuilder.Entity<InterHouseMessageReply>().ToTable("InterHouseMessageReply");
            modelBuilder.Entity<OrganizationPost>().ToTable("OrganizationPost");
            modelBuilder.Entity<OrganizationPostMetaData>().ToTable("OrganizationPostMetaData");
            modelBuilder.Entity<HousePostMetaData>().ToTable("HousePostMetaData");
            modelBuilder.Entity<Key>().ToTable("Key");
            modelBuilder.Entity<HouseProfileImage>().ToTable("HouseProfileImage");
            modelBuilder.Entity<DialingCode>().ToTable("DialingCode");
            modelBuilder.Entity<HousePostComment>().ToTable("HousePostComment");
        }
    }
}
