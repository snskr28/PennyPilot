using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PennyPilot.Backend.Domain.Entities;
using PennyPilot.Backend.Infrastructure;

namespace PennyPilot.Backend.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Income> Incomes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Usercategory> Usercategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("pk_categories");

            entity.ToTable("categories", tb => tb.HasComment("Stores income and expense categories"));

            entity.HasIndex(e => new { e.Isenabled, e.Isdeleted }, "idx_categories_enabled_deleted");

            entity.HasIndex(e => e.Name, "uq_categories_name").IsUnique();

            entity.Property(e => e.Categoryid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("categoryid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Isenabled)
                .HasDefaultValue(true)
                .HasColumnName("isenabled");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Expenseid).HasName("pk_expenses");

            entity.ToTable("expenses", tb => tb.HasComment("Stores expense transactions"));

            entity.HasIndex(e => e.Categoryid, "idx_expenses_categoryid");

            entity.HasIndex(e => e.Date, "idx_expenses_date");

            entity.HasIndex(e => new { e.Isenabled, e.Isdeleted }, "idx_expenses_enabled_deleted");

            entity.HasIndex(e => e.Userid, "idx_expenses_userid");

            entity.Property(e => e.Expenseid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("expenseid");
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Isenabled)
                .HasDefaultValue(true)
                .HasColumnName("isenabled");
            entity.Property(e => e.Paidby)
                .HasMaxLength(100)
                .HasColumnName("paidby");
            entity.Property(e => e.Paymentmode)
                .HasMaxLength(50)
                .HasColumnName("paymentmode");
            entity.Property(e => e.Receiptimage).HasColumnName("receiptimage");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Updatedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Category).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_expenses_categories");

            entity.HasOne(d => d.User).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_expenses_users");
        });

        modelBuilder.Entity<Income>(entity =>
        {
            entity.HasKey(e => e.Incomeid).HasName("pk_income");

            entity.ToTable("income", tb => tb.HasComment("Stores income transactions"));

            entity.HasIndex(e => e.Categoryid, "idx_income_categoryid");

            entity.HasIndex(e => e.Date, "idx_income_date");

            entity.HasIndex(e => new { e.Isenabled, e.Isdeleted }, "idx_income_enabled_deleted");

            entity.HasIndex(e => e.Userid, "idx_income_userid");

            entity.Property(e => e.Incomeid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("incomeid");
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Isenabled)
                .HasDefaultValue(true)
                .HasColumnName("isenabled");
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .HasColumnName("source");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Updatedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Category).WithMany(p => p.Incomes)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_income_categories");

            entity.HasOne(d => d.User).WithMany(p => p.Incomes)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_income_users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("pk_users");

            entity.ToTable("users", tb => tb.HasComment("Stores user account information"));

            entity.HasIndex(e => new { e.Isenabled, e.Isdeleted }, "idx_users_enabled_deleted");

            entity.HasIndex(e => e.Email, "uq_users_email").IsUnique();

            entity.HasIndex(e => e.Username, "uq_users_username").IsUnique();

            entity.Property(e => e.Userid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("userid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Dob)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Isenabled)
                .HasDefaultValue(true)
                .HasColumnName("isenabled");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Middlename)
                .HasMaxLength(50)
                .HasColumnName("middlename");
            entity.Property(e => e.Passwordhash).HasColumnName("passwordhash");
            entity.Property(e => e.Passwordresettoken)
                .HasMaxLength(255)
                .HasColumnName("passwordresettoken");
            entity.Property(e => e.Passwordresettokenexpiry)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("passwordresettokenexpiry");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Usercategory>(entity =>
        {
            entity.HasKey(e => e.Usercategoryid).HasName("pk_usercategories");

            entity.ToTable("usercategories", tb => tb.HasComment("Junction table linking users to their custom categories"));

            entity.HasIndex(e => e.Categoryid, "idx_usercategories_categoryid");

            entity.HasIndex(e => e.Userid, "idx_usercategories_userid");

            entity.HasIndex(e => new { e.Userid, e.Categoryid }, "uq_usercategory").IsUnique();

            entity.Property(e => e.Usercategoryid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("usercategoryid");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Category).WithMany(p => p.Usercategories)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usercategories_categories");

            entity.HasOne(d => d.User).WithMany(p => p.Usercategories)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usercategories_users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
