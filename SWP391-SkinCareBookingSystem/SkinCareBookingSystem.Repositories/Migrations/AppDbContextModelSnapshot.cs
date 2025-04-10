﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkinCareBookingSystem.Repositories.Data;

#nullable disable

namespace SkinCareBookingSystem.Repositories.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProductTransaction", b =>
                {
                    b.Property<int>("ProductsId")
                        .HasColumnType("int");

                    b.Property<int>("TransactionsId")
                        .HasColumnType("int");

                    b.HasKey("ProductsId", "TransactionsId");

                    b.HasIndex("TransactionsId");

                    b.ToTable("ProductTransaction");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.BookingServiceSchedule", b =>
                {
                    b.Property<int>("ScheduleLogId")
                        .HasColumnType("int");

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.HasKey("ScheduleLogId", "BookingId", "ServiceId");

                    b.HasIndex("BookingId");

                    b.HasIndex("ServiceId");

                    b.ToTable("BookingServiceSchedules");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Content", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ContentOfPost")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ImageId")
                        .HasColumnType("int");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ImageId");

                    b.HasIndex("PostId");

                    b.ToTable("Contents");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("Bytes")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Size")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.OptionSkinTypePoints", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("OptionId")
                        .HasColumnType("int");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<string>("SkinTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OptionId");

                    b.ToTable("OptionSkinTypePoints");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DatePost")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ImageId")
                        .HasColumnType("int");

                    b.Property<int>("PostStatus")
                        .HasColumnType("int");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ImageId");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ImageId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ImageId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.RecommendedService", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<int>("SurveyResultId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.HasIndex("SurveyResultId");

                    b.ToTable("RecommendedServices");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Schedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateWork")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.ScheduleLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsCancel")
                        .HasColumnType("bit");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStartShift")
                        .HasColumnType("datetime2");

                    b.Property<int>("WorkingTime")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.ToTable("ScheduleLogs");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.ServicesDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<int?>("ImageId")
                        .HasColumnType("int");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ImageId");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServicesDetails");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SkincareService", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Benefits")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("ImageId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ServiceDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WorkTime")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ImageId");

                    b.ToTable("SkincareServices");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveyOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("OptionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("SurveyOptions");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveyQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("QuestionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SurveyQuestions");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveyResponse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("OptionId")
                        .HasColumnType("int");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ResponseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("SessionId")
                        .HasColumnType("int");

                    b.Property<string>("SkinTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SurveyOptionId")
                        .HasColumnType("int");

                    b.Property<int?>("SurveyQuestionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OptionId");

                    b.HasIndex("QuestionId");

                    b.HasIndex("SessionId");

                    b.HasIndex("SurveyOptionId");

                    b.HasIndex("SurveyQuestionId");

                    b.ToTable("SurveyResponses");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveyResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RecommendationText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResultId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResultText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SkinType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SurveyResults");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveySession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CompletedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<string>("SelectedQuestionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SurveyResultId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SurveyResultId");

                    b.HasIndex("UserId");

                    b.ToTable("SurveySessions");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BookingId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ImageId")
                        .HasColumnType("int");

                    b.Property<long>("OrderCode")
                        .HasColumnType("bigint");

                    b.Property<string>("QrCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalMoney")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TranctionStatus")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ImageId");

                    b.HasIndex("UserId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ImageId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PaymentNumber")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("VerifyToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("YearOfBirth")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ImageId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.UserSkinTypeScore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<int>("SessionId")
                        .HasColumnType("int");

                    b.Property<string>("SkinTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SessionId");

                    b.ToTable("UserSkinTypeScores");
                });

            modelBuilder.Entity("ProductTransaction", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Transaction", null)
                        .WithMany()
                        .HasForeignKey("TransactionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Booking", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.User", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.BookingServiceSchedule", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Booking", "Booking")
                        .WithMany("BookingServiceSchedules")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.ScheduleLog", "ScheduleLog")
                        .WithMany("BookingServiceSchedules")
                        .HasForeignKey("ScheduleLogId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SkincareService", "Service")
                        .WithMany("BookingServiceSchedules")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("ScheduleLog");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Content", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Post", "Post")
                        .WithMany("Contents")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.OptionSkinTypePoints", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SurveyOption", "Option")
                        .WithMany("SkinTypePoints")
                        .HasForeignKey("OptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Option");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Post", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Category", "Category")
                        .WithMany("Posts")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Image");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Product", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.Navigation("Category");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.RecommendedService", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SkincareService", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SurveyResult", "SurveyResult")
                        .WithMany("RecommendedServices")
                        .HasForeignKey("SurveyResultId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");

                    b.Navigation("SurveyResult");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Schedule", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.User", "User")
                        .WithMany("Schedules")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.ScheduleLog", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Schedule", "Schedule")
                        .WithMany("ScheduleLogs")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.ServicesDetail", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SkincareService", "SkincareService")
                        .WithMany("ServicesDetails")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("SkincareService");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SkincareService", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Category", "Category")
                        .WithMany("skincareServices")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveyOption", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SurveyQuestion", "Question")
                        .WithMany("Options")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveyResponse", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SurveyOption", "Option")
                        .WithMany()
                        .HasForeignKey("OptionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SurveyQuestion", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SurveySession", "Session")
                        .WithMany("Responses")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SurveyOption", null)
                        .WithMany("Responses")
                        .HasForeignKey("SurveyOptionId");

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SurveyQuestion", null)
                        .WithMany("Responses")
                        .HasForeignKey("SurveyQuestionId");

                    b.Navigation("Option");

                    b.Navigation("Question");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveySession", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SurveyResult", "SurveyResult")
                        .WithMany("Sessions")
                        .HasForeignKey("SurveyResultId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("SurveyResult");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Transaction", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.User", "User")
                        .WithMany("Transactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.User", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Category", "Category")
                        .WithMany("Users")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.Navigation("Category");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.UserSkinTypeScore", b =>
                {
                    b.HasOne("SkinCareBookingSystem.BusinessObject.Entity.SurveySession", "Session")
                        .WithMany("SkinTypeScores")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Session");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Booking", b =>
                {
                    b.Navigation("BookingServiceSchedules");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Category", b =>
                {
                    b.Navigation("Posts");

                    b.Navigation("Products");

                    b.Navigation("Users");

                    b.Navigation("skincareServices");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Post", b =>
                {
                    b.Navigation("Contents");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.Schedule", b =>
                {
                    b.Navigation("ScheduleLogs");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.ScheduleLog", b =>
                {
                    b.Navigation("BookingServiceSchedules");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SkincareService", b =>
                {
                    b.Navigation("BookingServiceSchedules");

                    b.Navigation("ServicesDetails");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveyOption", b =>
                {
                    b.Navigation("Responses");

                    b.Navigation("SkinTypePoints");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveyQuestion", b =>
                {
                    b.Navigation("Options");

                    b.Navigation("Responses");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveyResult", b =>
                {
                    b.Navigation("RecommendedServices");

                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.SurveySession", b =>
                {
                    b.Navigation("Responses");

                    b.Navigation("SkinTypeScores");
                });

            modelBuilder.Entity("SkinCareBookingSystem.BusinessObject.Entity.User", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("Posts");

                    b.Navigation("Schedules");

                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
