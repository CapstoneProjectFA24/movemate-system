using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.Models;

namespace MoveMate.Domain.DBContext;

public partial class MoveMateDbContext : DbContext
{
    public MoveMateDbContext()
    {
    }

    public MoveMateDbContext(DbContextOptions<MoveMateDbContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<BookingStaffDaily> BookingStaffDailies { get; set; }

    public virtual DbSet<BookingTracker> BookingTrackers { get; set; }



    public virtual DbSet<FeeDetail> FeeDetails { get; set; }

    public virtual DbSet<FeeSetting> FeeSettings { get; set; }



    public virtual DbSet<HolidaySetting> HolidaySettings { get; set; }

    public virtual DbSet<HouseType> HouseTypes { get; set; }

  

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PromotionCategory> PromotionCategories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ScheduleBooking> ScheduleBookings { get; set; }

    public virtual DbSet<ScheduleWorking> ScheduleWorkings { get; set; }

   

    public virtual DbSet<Service> Services { get; set; }

    

    public virtual DbSet<TrackerSource> TrackerSources { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Truck> Trucks { get; set; }

    public virtual DbSet<TruckCategory> TruckCategories { get; set; }

    public virtual DbSet<TruckImg> TruckImgs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("Assignment");

            entity.Property(e => e.AddressCurrent).HasMaxLength(255);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.FailedReason).HasMaxLength(255);
            entity.Property(e => e.StaffType).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(255);

            entity.HasOne(d => d.BookingDetails).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.BookingDetailsId)
                .HasConstraintName("FK_Assignment_BookingDetails");

            entity.HasOne(d => d.Booking).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Assignment_Booking");

            entity.HasOne(d => d.ScheduleBooking).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.ScheduleBookingId)
                .HasConstraintName("FK_Assignment_ScheduleBooking");

            entity.HasOne(d => d.Truck).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.TruckId)
                .HasConstraintName("FK_Assignment_Truck");

            entity.HasOne(d => d.User).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Assignment_User");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Booking");

            entity.Property(e => e.BookingAt).HasColumnType("datetime");
            entity.Property(e => e.CancelReason).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.DeliveryAddress).HasMaxLength(255);
            entity.Property(e => e.DeliveryPoint).HasMaxLength(255);
            entity.Property(e => e.EstimatedDistance).HasMaxLength(255);
            entity.Property(e => e.EstimatedEndTime).HasColumnType("datetime");
            entity.Property(e => e.FloorsNumber).HasMaxLength(255);
            entity.Property(e => e.IsManyItems).HasMaxLength(255);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.PickupAddress).HasMaxLength(255);
            entity.Property(e => e.PickupPoint).HasMaxLength(255);
            entity.Property(e => e.ReportedReason).HasMaxLength(255);
            entity.Property(e => e.Review).HasMaxLength(255);
            entity.Property(e => e.ReviewAt).HasColumnType("datetime");
            entity.Property(e => e.RoomNumber).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.TypeBooking).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.HouseType).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.HouseTypeId)
                .HasConstraintName("FK_Booking_HouseType");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Booking_User");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_BookingDetails_Booking");

            entity.HasOne(d => d.Service).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_BookingDetails_Service");
        });

        modelBuilder.Entity<BookingStaffDaily>(entity =>
        {
            entity.ToTable("BookingStaffDaily");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ScheduleWorking).WithMany(p => p.BookingStaffDailies)
                .HasForeignKey(d => d.ScheduleWorkingId)
                .HasConstraintName("FK_BookingStaffDaily_ScheduleWorking");

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
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.FeeDetails)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_FeeDetails_Booking1");

            entity.HasOne(d => d.FeeSetting).WithMany(p => p.FeeDetails)
                .HasForeignKey(d => d.FeeSettingId)
                .HasConstraintName("FK_FeeDetails_FeeSetting");
        });

        modelBuilder.Entity<FeeSetting>(entity =>
        {
            entity.ToTable("FeeSetting");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DiscountRate).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.Unit).HasMaxLength(255);

            entity.HasOne(d => d.HouseType).WithMany(p => p.FeeSettings)
                .HasForeignKey(d => d.HouseTypeId)
                .HasConstraintName("FK_FeeSetting_HouseType");

            entity.HasOne(d => d.Service).WithMany(p => p.FeeSettings)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_FeeSetting_Service");
        });

        
        modelBuilder.Entity<HolidaySetting>(entity =>
        {
            entity.ToTable("HolidaySetting");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<HouseType>(entity =>
        {
            entity.ToTable("HouseType");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
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
            entity.Property(e => e.Date).HasColumnType("datetime");
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

            entity.HasOne(d => d.Service).WithMany(p => p.PromotionCategories)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_PromotionCategory_Service");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<ScheduleBooking>(entity =>
        {
            entity.ToTable("ScheduleBooking");

            entity.Property(e => e.Shard).HasMaxLength(255);
        });

        modelBuilder.Entity<ScheduleWorking>(entity =>
        {
            entity.ToTable("ScheduleWorking");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

      

        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Service");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);

            entity.HasOne(d => d.ParentService).WithMany(p => p.InverseParentService)
                .HasForeignKey(d => d.ParentServiceId)
                .HasConstraintName("FK_Service_ParentService");

            entity.HasOne(d => d.TruckCategory).WithMany(p => p.Services)
                .HasForeignKey(d => d.TruckCategoryId)
                .HasConstraintName("FK_Service_TruckCategory");
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
            entity.Property(e => e.PaymentMethod).HasMaxLength(255);
            entity.Property(e => e.Resource).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Substance).HasMaxLength(255);
            entity.Property(e => e.TransactionCode).HasMaxLength(255);
            entity.Property(e => e.TransactionType).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);

            entity.HasOne(d => d.Payment).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_Transaction_Payment");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("FK_Transaction_Wallet");
        });

        modelBuilder.Entity<Truck>(entity =>
        {
            entity.ToTable("Truck");

            entity.HasIndex(e => e.UserId, "UQ__Truck__1788CC4D11DE5635").IsUnique();

            entity.Property(e => e.Brand).HasMaxLength(255);
            entity.Property(e => e.Color).HasMaxLength(255);
            entity.Property(e => e.Model).HasMaxLength(255);
            entity.Property(e => e.NumberPlate).HasMaxLength(255);

            entity.HasOne(d => d.TruckCategory).WithMany(p => p.Trucks)
                .HasForeignKey(d => d.TruckCategoryId)
                .HasConstraintName("FK_Truck_TruckCategory");

            entity.HasOne(d => d.User).WithOne(p => p.Truck)
                .HasForeignKey<Truck>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Truck_User");
        });

        modelBuilder.Entity<TruckCategory>(entity =>
        {
            entity.ToTable("TruckCategory");

            entity.Property(e => e.CategoryName).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EstimatedHeight).HasMaxLength(255);
            entity.Property(e => e.EstimatedLenght).HasMaxLength(255);
            entity.Property(e => e.EstimatedWidth).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Summarize).HasMaxLength(255);
        });

        modelBuilder.Entity<TruckImg>(entity =>
        {
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
            entity.Property(e => e.Dob).HasColumnType("datetime");
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

            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.Value).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.UserInfos)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserInfo_User");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.ToTable("Voucher");

            entity.Property(e => e.Code).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.Vouchers)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Voucher_Booking");

            entity.HasOne(d => d.PromotionCategory).WithMany(p => p.Vouchers)
                .HasForeignKey(d => d.PromotionCategoryId)
                .HasConstraintName("FK_Voucher_PromotionCategory");

            entity.HasOne(d => d.User).WithMany(p => p.Vouchers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Voucher_User");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.ToTable("Wallet");

            entity.HasIndex(e => e.UserId, "UQ_Wallet_UserId").IsUnique();

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.LockReason).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithOne(p => p.Wallet)
                .HasForeignKey<Wallet>(d => d.UserId)
                .HasConstraintName("FK_Wallet_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
