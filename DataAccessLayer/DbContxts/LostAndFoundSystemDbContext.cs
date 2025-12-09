using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.DbContxts;

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

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ReturnRecord> ReturnRecords { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServiceLocation> ServiceLocations { get; set; }

    public virtual DbSet<Upload> Uploads { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__campus__3213E83F307D0F6A");

            entity.ToTable("campus");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Datecreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("datecreate");
            entity.Property(e => e.Dateupdate).HasColumnName("dateupdate");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83F2F0C8777");

            entity.ToTable("categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datecreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("datecreate");
            entity.Property(e => e.Dateupdate).HasColumnName("dateupdate");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__item__3213E83FF11D1293");

            entity.ToTable("item");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CurrentLocationId).HasColumnName("current_location_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FoundDate).HasColumnName("found_date");
            entity.Property(e => e.FoundLocation)
                .HasMaxLength(255)
                .HasColumnName("found_location");
            entity.Property(e => e.Img)
                .HasMaxLength(255)
                .HasColumnName("img");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__item__category_i__4D94879B");

            entity.HasOne(d => d.CurrentLocation).WithMany(p => p.Items)
                .HasForeignKey(d => d.CurrentLocationId)
                .HasConstraintName("FK__item__current_lo__4E88ABD4");

            entity.HasOne(d => d.User).WithMany(p => p.Items)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__item__user_id__4F7CD00D");
        });

        modelBuilder.Entity<ReturnRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__return_r__3213E83F2C0DC57D");

            entity.ToTable("return_record");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConfirmImg)
                .HasMaxLength(255)
                .HasColumnName("confirm_img");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_created");
            entity.Property(e => e.DateUpdate).HasColumnName("date_update");
            entity.Property(e => e.EvidenceImg)
                .HasMaxLength(255)
                .HasColumnName("evidence_img");
            entity.Property(e => e.ImgCccdBack)
                .HasMaxLength(255)
                .HasColumnName("img_cccd_back");
            entity.Property(e => e.ImgCccdFront)
                .HasMaxLength(255)
                .HasColumnName("img_cccd_front");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Mssv)
                .HasMaxLength(50)
                .HasColumnName("mssv");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.VerifyNotes).HasColumnName("verify_notes");

            entity.HasOne(d => d.Item).WithMany(p => p.ReturnRecords)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__return_re__item___5441852A");

            entity.HasOne(d => d.Staff).WithMany(p => p.ReturnRecords)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__return_re__staff__5535A963");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles__3213E83F75CA083A");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datecreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("datecreate");
            entity.Property(e => e.Dateupdate).HasColumnName("dateupdate");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
        });

        modelBuilder.Entity<ServiceLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__service___3213E83FA1331CE2");

            entity.ToTable("service_location");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.Datecreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("datecreate");
            entity.Property(e => e.Dateupdate).HasColumnName("dateupdate");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");

            entity.HasOne(d => d.Campus).WithMany(p => p.ServiceLocations)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__service_l__campu__45F365D3");
        });

        modelBuilder.Entity<Upload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__upload__3213E83F3BE998BE");

            entity.ToTable("upload");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.DateAccept).HasColumnName("date_accept");
            entity.Property(e => e.DateCreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("date_create");
            entity.Property(e => e.IdItem).HasColumnName("idItem");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.StaffId).HasColumnName("staffId");
            entity.Property(e => e.StaffIdAccept).HasColumnName("staffId_accept");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.StatusAccept).HasColumnName("status_accept");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.Uploads)
                .HasForeignKey(d => d.IdItem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__upload__idItem__59FA5E80");

            entity.HasOne(d => d.Staff).WithMany(p => p.UploadStaffs)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK__upload__staffId__5BE2A6F2");

            entity.HasOne(d => d.StaffIdAcceptNavigation).WithMany(p => p.UploadStaffIdAcceptNavigations)
                .HasForeignKey(d => d.StaffIdAccept)
                .HasConstraintName("FK__upload__staffId___5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.UploadUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__upload__userId__5AEE82B9");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user__3213E83FB66860F9");

            entity.ToTable("user");

            entity.HasIndex(e => e.Username, "UQ__user__F3DBC572C10FFAEE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Avt)
                .HasMaxLength(255)
                .HasColumnName("avt");
            entity.Property(e => e.Gmail)
                .HasMaxLength(150)
                .HasColumnName("gmail");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user__role_id__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
