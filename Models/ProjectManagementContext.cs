using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProjectmanagementAPI.Models;

namespace ProjectmanagementAPI.Models;

public partial class ProjectManagementContext : DbContext
{
    public ProjectManagementContext()
    {
    }

    public ProjectManagementContext(DbContextOptions<ProjectManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<PasswordOtp> Passwordotps { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=ProjectManagementDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__COMMENTS__C3B4DFAA123C5F48");

            entity.ToTable("COMMENTS");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.Comment1)
                .IsUnicode(false)
                .HasColumnName("Comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Task).WithMany(p => p.Comments)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_Comments_Tasks");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<PasswordOtp>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("PASSWORD_OTPS");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OtpCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__PROJECTS__761ABED0B44677AA");

            entity.ToTable("PROJECTS");

            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.OwnerId).HasColumnName("ownerID");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Owner).WithMany(p => p.Projects)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Projects_Users");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__TASKS__7C6949D165347DEF");

            entity.ToTable("TASKS");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TaskName)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Tasks_Users");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_Tasks_Projects");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__TEAMS__123AE7B99CBE9322");

            entity.ToTable("TEAMS");

            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.TeamName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.TeamMemberId).HasName("PK__TEAM_MEM__C7C092851648DD66");

            entity.ToTable("TEAM_MEMBERS");

            entity.Property(e => e.TeamMemberId).HasColumnName("TeamMemberID");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_TeamMembers_Teams");

            entity.HasOne(d => d.User).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_TeamMembers_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USERS__1788CCACC88CB00D");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.Email, "UQ__USERS__A9D1053418126D80").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}