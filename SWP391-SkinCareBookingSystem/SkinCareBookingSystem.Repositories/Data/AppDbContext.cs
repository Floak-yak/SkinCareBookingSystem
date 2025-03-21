﻿using Microsoft.EntityFrameworkCore;
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
        }
    }
}
