using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        #region DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<SkincareService> SkincareServices { get; set; }
        public DbSet<TestInformation> TestInformations { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ScheduleLog> ScheduleLogs { get; set; }
        public DbSet<BookingServiceSchedule> BookingServiceSchedules { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ServicesDetail> ServicesDetails { get; set; }

        // Survey-related entities
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<SurveyOption> SurveyOptions { get; set; }
        public DbSet<SurveyResult> SurveyResults { get; set; }
        public DbSet<SurveySession> SurveySessions { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<RecommendedService> RecommendedServices { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Users)
                .WithOne(u => u.Category)
                .HasForeignKey(u => u.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Content>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Contents)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TestInformation>()
                .HasOne(ti => ti.User)
                .WithMany(u => u.TestInformationHistory)
                .HasForeignKey(ti => ti.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.User)
                .WithMany(u => u.Schedules)
                .HasForeignKey(ti => ti.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Category)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScheduleLog>()
                .HasOne(sl => sl.Schedule)
                .WithMany(s => s.ScheduleLogs)
                .HasForeignKey(sl => sl.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServicesDetail>()
                .HasOne(sd => sd.SkincareService)
                .WithMany(s => s.ServicesDetails)
                .HasForeignKey(sd => sd.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingServiceSchedule>()
                .HasKey(bss => new
                {
                    bss.ScheduleLogId,
                    bss.BookingId,
                    bss.ServiceId
                });

            modelBuilder.Entity<BookingServiceSchedule>()
                .HasOne(bss => bss.Booking)
                .WithMany(b => b.BookingServiceSchedules)
                .HasForeignKey(bss => bss.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingServiceSchedule>()
                .HasOne(bss => bss.Service)
                .WithMany(s => s.BookingServiceSchedules)
                .HasForeignKey(bss => bss.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingServiceSchedule>()
                .HasOne(bss => bss.ScheduleLog)
                .WithMany(sl => sl.BookingServiceSchedules)
                .HasForeignKey(bss => bss.ScheduleLogId)
                .OnDelete(DeleteBehavior.Restrict);

            // Survey relationships
            modelBuilder.Entity<SurveyQuestion>()
                .HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SurveySession>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SurveySession>()
                .HasOne(s => s.SurveyResult)
                .WithMany(r => r.Sessions)
                .HasForeignKey(s => s.SurveyResultId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SurveySession>()
                .HasMany(s => s.Responses)
                .WithOne(r => r.Session)
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SurveyResponse>()
                .HasOne(r => r.Question)
                .WithMany()
                .HasForeignKey(r => r.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SurveyResponse>()
                .HasOne(r => r.Option)
                .WithMany()
                .HasForeignKey(r => r.OptionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecommendedService>()
                .HasOne(rs => rs.SurveyResult)
                .WithMany(r => r.RecommendedServices)
                .HasForeignKey(rs => rs.SurveyResultId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecommendedService>()
                .HasOne(rs => rs.Service)
                .WithMany()
                .HasForeignKey(rs => rs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
