using System;
using System.Collections.Generic;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DbContexts;

public partial class LostAndFoundSystemDbContext : DbContext
{
    public LostAndFoundSystemDbContext()
    {
    }

    public LostAndFoundSystemDbContext(DbContextOptions<LostAndFoundSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Campus> Campuses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ImageStorage> ImageStorages { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemHistory> ItemHistories { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Receipt> Receipts { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReturnRecord> ReturnRecords { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServiceLocation> ServiceLocations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Database=LostAndFoundSystemDB;User Id=sa;Password=1234567890;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campus>(entity =>
        {
            entity.ToTable("campus");

            entity.HasIndex(e => e.Name, "UQ_campus_name").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "UQ_categories_name").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
        });

        modelBuilder.Entity<ImageStorage>(entity =>
        {
            entity.ToTable("image_storage");

            entity.HasIndex(e => new { e.EntityType, e.EntityId, e.Status }, "IX_image_storage_entity");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.ImageOrder)
                .HasDefaultValue(0)
                .HasColumnName("image_order");
            entity.Property(e => e.ImageType)
                .HasMaxLength(50)
                .HasColumnName("image_type");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.MimeType)
                .HasMaxLength(50)
                .HasColumnName("mime_type");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.ImageStorages)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_image_storage_user");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("item", tb => tb.HasTrigger("trg_item_audit"));

            entity.HasIndex(e => new { e.CategoryId, e.FoundCampusId }, "IX_item_search");

            entity.HasIndex(e => new { e.Status, e.DateCreated }, "IX_item_status").IsDescending(false, true);

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CurrentLocationId).HasColumnName("current_location_id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.DateUpdated).HasColumnName("date_updated");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FoundCampusId).HasColumnName("found_campus_id");
            entity.Property(e => e.FoundDate).HasColumnName("found_date");
            entity.Property(e => e.FoundLocation).HasColumnName("found_location");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_item_category");

            entity.HasOne(d => d.CurrentLocation).WithMany(p => p.Items)
                .HasForeignKey(d => d.CurrentLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_item_location");

            entity.HasOne(d => d.FoundCampus).WithMany(p => p.Items)
                .HasForeignKey(d => d.FoundCampusId)
                .HasConstraintName("FK_item_found_campus");

            entity.HasOne(d => d.User).WithMany(p => p.Items)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_item_user");
        });

        modelBuilder.Entity<ItemHistory>(entity =>
        {
            entity.ToTable("item_history");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OldValue).HasColumnName("old_value");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.ItemHistories)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_item_history_user");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemHistories)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_item_history_item");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notifications");

            entity.HasIndex(e => new { e.UserId, e.IsRead, e.DateCreated }, "IX_notifications_user").IsDescending(false, false, true);

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.ReferenceId).HasColumnName("reference_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_notifications_user");
        });

        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.ToTable("receipt");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.DateUpdate).HasColumnName("date_update");
            entity.Property(e => e.DateVerified).HasColumnName("date_verified");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VerifyNotes).HasColumnName("verify_notes");

            entity.HasOne(d => d.Item).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_receipt_item");

            entity.HasOne(d => d.Staff).WithMany(p => p.ReceiptStaffs)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK_receipt_staff");

            entity.HasOne(d => d.User).WithMany(p => p.ReceiptUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_receipt_user");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IsRevoked)
                .HasDefaultValue(false)
                .HasColumnName("is_revoked");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_refresh_tokens_user");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("report");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ActionDate).HasColumnName("action_date");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("OPEN")
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Item).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK_report_item");

            entity.HasOne(d => d.User).WithMany(p => p.Reports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_report_user");
        });

        modelBuilder.Entity<ReturnRecord>(entity =>
        {
            entity.ToTable("return_record");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ConfirmImg).HasColumnName("confirm_img");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.DateUpdate).HasColumnName("date_update");
            entity.Property(e => e.EvidenceImg).HasColumnName("evidence_img");
            entity.Property(e => e.ImgCccdBack).HasColumnName("img_cccd_back");
            entity.Property(e => e.ImgCccdFront).HasColumnName("img_cccd_front");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("COMPLETED")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VerifyNotes).HasColumnName("verify_notes");

            entity.HasOne(d => d.Item).WithMany(p => p.ReturnRecords)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_return_record_item");

            entity.HasOne(d => d.Staff).WithMany(p => p.ReturnRecordStaffs)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_return_record_staff");

            entity.HasOne(d => d.User).WithMany(p => p.ReturnRecordUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_return_record_user");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "UQ_roles_name").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
        });

        modelBuilder.Entity<ServiceLocation>(entity =>
        {
            entity.ToTable("service_location");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.DateUpdate).HasColumnName("date_update");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");

            entity.HasOne(d => d.Campus).WithMany(p => p.ServiceLocations)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_service_location_campus");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.HasIndex(e => e.CampusId, "IX_user_campus");

            entity.HasIndex(e => new { e.RoleId, e.Status }, "IX_user_role");

            entity.HasIndex(e => e.Gmail, "UQ_user_gmail").IsUnique();

            entity.HasIndex(e => e.Mssv, "UQ_user_mssv").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ_user_username").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Gmail)
                .HasMaxLength(100)
                .HasColumnName("gmail");
            entity.Property(e => e.Mssv)
                .HasMaxLength(20)
                .HasColumnName("mssv");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("user_name");

            entity.HasOne(d => d.Campus).WithMany(p => p.Users)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK_user_campus");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
