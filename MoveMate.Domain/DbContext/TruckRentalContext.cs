﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MoveMate.Domain.Models;

public partial class TruckRentalContext : DbContext
{
    public TruckRentalContext()
    {
    }

    public TruckRentalContext(DbContextOptions<TruckRentalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<AchievementDetail> AchievementDetails { get; set; }

    public virtual DbSet<AchievementSetting> AchievementSettings { get; set; }

   

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<BookingItem> BookingItems { get; set; }

    public virtual DbSet<BookingStaffDaily> BookingStaffDailies { get; set; }

    public virtual DbSet<BookingTracker> BookingTrackers { get; set; }

   
    public virtual DbSet<FeeDetail> FeeDetails { get; set; }

    public virtual DbSet<FeeSetting> FeeSettings { get; set; }

   

    public virtual DbSet<HouseType> HouseTypes { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemCategory> ItemCategories { get; set; }

   

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PromotionCategory> PromotionCategories { get; set; }

    public virtual DbSet<PromotionDetail> PromotionDetails { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<ScheduleDetail> ScheduleDetails { get; set; }

   

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceBooking> ServiceBookings { get; set; }

    

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<TrackerSource> TrackerSources { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TripAccuracy> TripAccuracies { get; set; }

    public virtual DbSet<Truck> Trucks { get; set; }

    public virtual DbSet<TruckCategory> TruckCategories { get; set; }

    public virtual DbSet<TruckImg> TruckImgs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.ToTable("Achievement");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Achievements)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Achievement_User");
        });

        modelBuilder.Entity<AchievementDetail>(entity =>
        {
            entity.HasOne(d => d.Achievement).WithMany(p => p.AchievementDetails)
                .HasForeignKey(d => d.AchievementId)
                .HasConstraintName("FK_AchievementDetails_Achievement");

            entity.HasOne(d => d.Booking).WithMany(p => p.AchievementDetails)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_AchievementDetails_Booking");
        });

        modelBuilder.Entity<AchievementSetting>(entity =>
        {
            entity.ToTable("AchievementSetting");

            entity.Property(e => e.AwardWinningHook).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Booking");

            entity.Property(e => e.Bonus).HasMaxLength(255);
            entity.Property(e => e.BoxType).HasMaxLength(255);
            entity.Property(e => e.CancelReason).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.DeliveryAddress).HasMaxLength(255);
            entity.Property(e => e.DeliveryPoint).HasMaxLength(255);
            entity.Property(e => e.EstimatedAcreage).HasMaxLength(255);
            entity.Property(e => e.EstimatedDeliveryTime).HasMaxLength(255);
            entity.Property(e => e.EstimatedDistance).HasMaxLength(255);
            entity.Property(e => e.EstimatedHeight).HasMaxLength(255);
            entity.Property(e => e.EstimatedLength).HasMaxLength(255);
            entity.Property(e => e.EstimatedTotalWeight).HasMaxLength(255);
            entity.Property(e => e.EstimatedVolume).HasMaxLength(255);
            entity.Property(e => e.EstimatedWeight).HasMaxLength(255);
            entity.Property(e => e.EstimatedWidth).HasMaxLength(255);
            entity.Property(e => e.FeeInfo).HasMaxLength(255);
            entity.Property(e => e.FloorsNumber).HasMaxLength(255);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.PickupAddress).HasMaxLength(255);
            entity.Property(e => e.PickupPoint).HasMaxLength(255);
            entity.Property(e => e.ReportedReason).HasMaxLength(255);
            entity.Property(e => e.Review).HasMaxLength(255);
            entity.Property(e => e.RoomNumber).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.TypeBooking).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Booking_User");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.Property(e => e.StaffType).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_BookingDetails_Booking");

            entity.HasOne(d => d.User).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_BookingDetails_User");
        });

        modelBuilder.Entity<BookingItem>(entity =>
        {
            entity.ToTable("BookingItem");

            entity.Property(e => e.EstimatedHeight).HasMaxLength(255);
            entity.Property(e => e.EstimatedLenght).HasMaxLength(255);
            entity.Property(e => e.EstimatedVolume).HasMaxLength(255);
            entity.Property(e => e.EstimatedWeight).HasMaxLength(255);
            entity.Property(e => e.EstimatedWidth).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingItems)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_BookingItem_Booking");

            entity.HasOne(d => d.Item).WithMany(p => p.BookingItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK_BookingItem_Item");
        });

        modelBuilder.Entity<BookingStaffDaily>(entity =>
        {
            entity.ToTable("BookingStaffDaily");

            entity.Property(e => e.AddressCurrent).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.BookingStaffDailies)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_BookingStaffDaily_User");
        });

        modelBuilder.Entity<BookingTracker>(entity =>
        {
            entity.ToTable("BookingTracker");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Point).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Time).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingTrackers)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_BookingTracker_Booking");
        });

       

        modelBuilder.Entity<FeeDetail>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany()
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_FeeDetails_Booking");

            entity.HasOne(d => d.FeeSetting).WithMany()
                .HasForeignKey(d => d.FeeSettingId)
                .HasConstraintName("FK_FeeDetails_FeeSetting");
        });

        modelBuilder.Entity<FeeSetting>(entity =>
        {
            entity.ToTable("FeeSetting");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
        });

       

        modelBuilder.Entity<HouseType>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HouseType");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany()
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_HouseType_Booking");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("Item");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ImgUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.ItemCategory).WithMany(p => p.Items)
                .HasForeignKey(d => d.ItemCategoryId)
                .HasConstraintName("FK_Item_ItemCategory");
        });

        modelBuilder.Entity<ItemCategory>(entity =>
        {
            entity.ToTable("ItemCategory");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ImgUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
        });

        

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DeviceId).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Receive).HasMaxLength(255);
            entity.Property(e => e.SentFrom).HasMaxLength(255);
            entity.Property(e => e.Topic).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.Property(e => e.BankCode).HasMaxLength(255);
            entity.Property(e => e.BankTransNo).HasMaxLength(255);
            entity.Property(e => e.CardType).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.ResponseCode).HasMaxLength(255);
            entity.Property(e => e.Token).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Payment_Booking");
        });

        modelBuilder.Entity<PromotionCategory>(entity =>
        {
            entity.ToTable("PromotionCategory");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EndBookingTime).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.StartBookingTime).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(255);
        });

        modelBuilder.Entity<PromotionDetail>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.PromotionDetails)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_PromotionDetails_Booking");

            entity.HasOne(d => d.PromotionCategory).WithMany(p => p.PromotionDetails)
                .HasForeignKey(d => d.PromotionCategoryId)
                .HasConstraintName("FK_PromotionDetails_PromotionCategory");

            entity.HasOne(d => d.User).WithMany(p => p.PromotionDetails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_PromotionDetails_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("Schedule");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<ScheduleDetail>(entity =>
        {
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Schedule).WithMany(p => p.ScheduleDetails)
                .HasForeignKey(d => d.ScheduleId)
                .HasConstraintName("FK_ScheduleDetails_Schedule");

            entity.HasOne(d => d.User).WithMany(p => p.ScheduleDetails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ScheduleDetails_User");
        });

      

        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Service");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<ServiceBooking>(entity =>
        {
            entity.ToTable("ServiceBooking");

            entity.HasOne(d => d.Booking).WithMany(p => p.ServiceBookings)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_ServiceBooking_Booking");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceBookings)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_ServiceBooking_Service");
        });

       
        modelBuilder.Entity<Token>(entity =>
        {
            entity.ToTable("Token");

            entity.Property(e => e.RefreshToken).HasMaxLength(255);
            entity.Property(e => e.Token1)
                .HasMaxLength(255)
                .HasColumnName("Token");
            entity.Property(e => e.TokenType).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Token_User");
        });

        modelBuilder.Entity<TrackerSource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TrackerImg");

            entity.ToTable("TrackerSource");

            entity.Property(e => e.ResourceCode).HasMaxLength(255);
            entity.Property(e => e.ResourceUrl).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);

            entity.HasOne(d => d.BookingTracker).WithMany(p => p.TrackerSources)
                .HasForeignKey(d => d.BookingTrackerId)
                .HasConstraintName("FK_TrackerSource_BookingTracker");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.FailedReason).HasMaxLength(255);
            entity.Property(e => e.PaymentMethod).HasMaxLength(255);
            entity.Property(e => e.Resource).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Substance).HasMaxLength(255);
            entity.Property(e => e.TransactionCode).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Payment).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_Transaction_Payment");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("FK_Transaction_Wallet");
        });

        modelBuilder.Entity<TripAccuracy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripAccu__3214EC0738E9E07D");

            entity.ToTable("TripAccuracy");

            entity.Property(e => e.Shard).HasMaxLength(255);
            entity.Property(e => e.TotalApprovedTrip).HasColumnName("TotalApproved_trip");

            entity.HasOne(d => d.User).WithMany(p => p.TripAccuracies)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_TripAccuracy_User");
        });

        modelBuilder.Entity<Truck>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Vehicle");

            entity.ToTable("Truck");

            entity.Property(e => e.Brand).HasMaxLength(255);
            entity.Property(e => e.Color).HasMaxLength(255);
            entity.Property(e => e.Model).HasMaxLength(255);
            entity.Property(e => e.NumberPlate).HasMaxLength(255);

            entity.HasOne(d => d.TruckCategory).WithMany(p => p.Trucks)
                .HasForeignKey(d => d.TruckCategoryId)
                .HasConstraintName("FK_Truck_TruckCategory");

            entity.HasOne(d => d.User).WithMany(p => p.Trucks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Truck_User");
        });

        modelBuilder.Entity<TruckCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_VehicleCategory");

            entity.ToTable("TruckCategory");

            entity.Property(e => e.CategoryName).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EstimatedHeight).HasMaxLength(255);
            entity.Property(e => e.EstimatedLength).HasMaxLength(255);
            entity.Property(e => e.EstimatedWidth).HasMaxLength(255);
            entity.Property(e => e.ImgUrl).HasMaxLength(255);
            entity.Property(e => e.Summarize).HasMaxLength(255);
        });

        modelBuilder.Entity<TruckImg>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_VehicleImg");

            entity.ToTable("TruckImg");

            entity.Property(e => e.ImageCode).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Truck).WithMany(p => p.TruckImgs)
                .HasForeignKey(d => d.TruckId)
                .HasConstraintName("FK_TruckImg_Truck");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.CodeIntroduce).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.NumberIntroduce).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.ToTable("UserInfo");

            entity.Property(e => e.Cavet).HasMaxLength(255);
            entity.Property(e => e.CitizenIdentification).HasMaxLength(255);
            entity.Property(e => e.Code).HasMaxLength(255);
            entity.Property(e => e.CurriculumVitae).HasMaxLength(255);
            entity.Property(e => e.HealthCertificate).HasMaxLength(255);
            entity.Property(e => e.HealthInsurance).HasMaxLength(255);
            entity.Property(e => e.ImgUrl).HasMaxLength(255);
            entity.Property(e => e.License).HasMaxLength(255);
            entity.Property(e => e.PermanentAddress).HasMaxLength(255);
            entity.Property(e => e.TemporaryResidenceAddress).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.UserInfos)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserInfo_User");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.ToTable("Wallet");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.LockReason).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Wallet_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
