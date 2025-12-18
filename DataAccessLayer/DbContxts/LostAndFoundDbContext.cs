using System;
using System.Collections.Generic;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DbContxts;

public partial class LostAndFoundDbContext : DbContext
{
    public LostAndFoundDbContext()
    {
    }

    public LostAndFoundDbContext(DbContextOptions<LostAndFoundDbContext> options)
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Database=LostAndFoundDb;User Id=sa;Password=12345;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__campus__3213E83F138A2398");

            entity.ToTable("campus", tb => tb.HasTrigger("trg_campus_update"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.Datecreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("datecreate");
            entity.Property(e => e.Dateupdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("dateupdate");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83FD871D602");

            entity.ToTable("categories", tb => tb.HasTrigger("trg_categories_update"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Datecreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("datecreate");
            entity.Property(e => e.Dateupdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("dateupdate");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__item__3213E83F72A91B6C");

            entity.ToTable("item");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Context).HasColumnName("context");
            entity.Property(e => e.CurrentLocationId).HasColumnName("current_location_id");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FoundDate)
                .HasColumnType("datetime")
                .HasColumnName("found_date");
            entity.Property(e => e.FoundLocation)
                .HasMaxLength(500)
                .HasColumnName("found_location");
            entity.Property(e => e.Img)
                .HasMaxLength(500)
                .HasColumnName("img");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__item__category_i__59063A47");

            entity.HasOne(d => d.CurrentLocation).WithMany(p => p.Items)
                .HasForeignKey(d => d.CurrentLocationId)
                .HasConstraintName("FK__item__current_lo__59FA5E80");

            entity.HasOne(d => d.User).WithMany(p => p.Items)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__item__user_id__5AEE82B9");
        });

        modelBuilder.Entity<ReturnRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__return_r__3213E83FA32B5A53");

            entity.ToTable("return_record", tb => tb.HasTrigger("trg_return_record_update"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ConfirmImg)
                .HasMaxLength(500)
                .HasColumnName("confirm_img");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date_created");
            entity.Property(e => e.DateUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date_update");
            entity.Property(e => e.EvidenceImg)
                .HasMaxLength(500)
                .HasColumnName("evidence_img");
            entity.Property(e => e.ImgCccdBack)
                .HasMaxLength(500)
                .HasColumnName("img_cccd_back");
            entity.Property(e => e.ImgCccdFont)
                .HasMaxLength(500)
                .HasColumnName("img_cccd_font");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VerifyNotes).HasColumnName("verify_notes");

            entity.HasOne(d => d.Item).WithMany(p => p.ReturnRecords)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__return_re__item___6B24EA82");

            entity.HasOne(d => d.Staff).WithMany(p => p.ReturnRecordStaffs)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__return_re__staff__6C190EBB");

            entity.HasOne(d => d.User).WithMany(p => p.ReturnRecordUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__return_re__user___6D0D32F4");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles__3213E83F292C1C30");

            entity.ToTable("roles", tb => tb.HasTrigger("trg_roles_update"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Datecreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("datecreate");
            entity.Property(e => e.Dateupdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("dateupdate");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
        });

        modelBuilder.Entity<ServiceLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__service___3213E83FE75B32F9");

            entity.ToTable("service_location", tb => tb.HasTrigger("trg_service_location_update"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.Datecreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("datecreate");
            entity.Property(e => e.Dateupdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("dateupdate");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");

            entity.HasOne(d => d.Campus).WithMany(p => p.ServiceLocations)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK__service_l__campu__46E78A0C");
        });

        modelBuilder.Entity<Upload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__upload__3213E83FB79CD4F2");

            entity.ToTable("upload");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.DateCreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date_create");
            entity.Property(e => e.DateUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date_update");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Img)
                .HasMaxLength(500)
                .HasColumnName("img");
            entity.Property(e => e.LostDate)
                .HasColumnType("datetime")
                .HasColumnName("lost_date");
            entity.Property(e => e.LostLocation)
                .HasMaxLength(500)
                .HasColumnName("lost_location");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.NoteCreate)
                .HasColumnType("datetime")
                .HasColumnName("note_create");
            entity.Property(e => e.NoteUpdate)
                .HasColumnType("datetime")
                .HasColumnName("note_update");
            entity.Property(e => e.Staffid).HasColumnName("staffid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Campus).WithMany(p => p.Uploads)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK__upload__campus_i__6477ECF3");

            entity.HasOne(d => d.Category).WithMany(p => p.Uploads)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__upload__category__6383C8BA");

            entity.HasOne(d => d.Staff).WithMany(p => p.UploadStaffs)
                .HasForeignKey(d => d.Staffid)
                .HasConstraintName("FK__upload__staffid__628FA481");

            entity.HasOne(d => d.User).WithMany(p => p.UploadUsers)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("FK__upload__userid__619B8048");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user__3213E83F31FF5236");

            entity.ToTable("user");

            entity.HasIndex(e => e.Gmail, "UQ__user__493D0C0AE88DBB60").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__user__F3DBC57240A437F5").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.Ext)
                .HasMaxLength(50)
                .HasColumnName("ext");
            entity.Property(e => e.Gmail)
                .HasMaxLength(255)
                .HasColumnName("gmail");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("ACTIVE")
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__user__role_id__534D60F1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
