using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PartTimeJobManagement.API.Models;

public partial class PartTimeJobManagementContext : DbContext
{
    public PartTimeJobManagementContext()
    {
    }

    public PartTimeJobManagementContext(DbContextOptions<PartTimeJobManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<ApplicationHistory> ApplicationHistories { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Employer> Employers { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobCategory> JobCategories { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:MyCnn");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasIndex(e => e.ApplicationDate, "IX_Applications_Date").IsDescending();

            entity.HasIndex(e => e.JobId, "IX_Applications_JobID");

            entity.HasIndex(e => e.Status, "IX_Applications_Status");

            entity.HasIndex(e => e.StudentId, "IX_Applications_StudentID");

            entity.HasIndex(e => new { e.StudentId, e.Status }, "IX_Applications_Student_Status");

            entity.HasIndex(e => new { e.StudentId, e.JobId }, "UQ_Applications_StudentJob").IsUnique();

            entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
            entity.Property(e => e.ApplicationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Job).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applications_Jobs");

            entity.HasOne(d => d.Student).WithMany(p => p.Applications)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applications_Students");
        });

        modelBuilder.Entity<ApplicationHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId);

            entity.ToTable("ApplicationHistory");

            entity.HasIndex(e => e.ApplicationId, "IX_History_ApplicationID");

            entity.Property(e => e.HistoryId).HasColumnName("HistoryID");
            entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Application).WithMany(p => p.ApplicationHistories)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_History_Applications");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasIndex(e => e.ApplicationId, "IX_Contracts_ApplicationID");

            entity.HasIndex(e => new { e.Status, e.StartDate, e.EndDate }, "IX_Contracts_Status_Dates");

            entity.HasIndex(e => e.ApplicationId, "UQ_Contracts_Application").IsUnique();

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
            entity.Property(e => e.ContractFile).HasMaxLength(256);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SalaryAgreed).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Application).WithOne(p => p.Contract)
                .HasForeignKey<Contract>(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contracts_Applications");
        });

        modelBuilder.Entity<Employer>(entity =>
        {
            entity.HasIndex(e => e.IsVerified, "IX_Employers_IsVerified");

            entity.Property(e => e.EmployerId)
                .ValueGeneratedNever()
                .HasColumnName("EmployerID");
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.CompanyName).HasMaxLength(150);
            entity.Property(e => e.ContactName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TaxCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.EmployerNavigation).WithOne(p => p.Employer)
                .HasForeignKey<Employer>(d => d.EmployerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employers_Users");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasIndex(e => e.CategoryId, "IX_Jobs_CategoryID");

            entity.HasIndex(e => e.EmployerId, "IX_Jobs_EmployerID");

            entity.HasIndex(e => e.ExpirationDate, "IX_Jobs_ExpirationDate").HasFilter("([ExpirationDate] IS NOT NULL)");

            entity.HasIndex(e => e.Location, "IX_Jobs_Location");

            entity.HasIndex(e => e.PostedDate, "IX_Jobs_PostedDate").IsDescending();

            entity.HasIndex(e => e.Status, "IX_Jobs_Status");

            entity.HasIndex(e => new { e.Status, e.CategoryId }, "IX_Jobs_Status_Category");

            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.EmployerId).HasColumnName("EmployerID");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.PostedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Open");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Jobs_Categories");

            entity.HasOne(d => d.Employer).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.EmployerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jobs_Employers");
        });

        modelBuilder.Entity<JobCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.HasIndex(e => e.CategoryName, "UQ_JobCategories_Name").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.ContractId, "IX_Payments_ContractID");

            entity.HasIndex(e => new { e.ContractId, e.Status }, "IX_Payments_Contract_Status");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Contract).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Contracts");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.RoleName, "UQ_Roles_RoleName").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.StudentId)
                .ValueGeneratedNever()
                .HasColumnName("StudentID");
            entity.Property(e => e.Cvfile)
                .HasMaxLength(256)
                .HasColumnName("CVFile");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Major).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.StudentNavigation).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.IsActive, "IX_Users_IsActive").HasFilter("([IsActive]=(1))");

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleID");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
