using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MinesweeperServer.Models;

public partial class TasksManagementDbContext : DbContext
{
    public TasksManagementDbContext()
    {
    }

    public TasksManagementDbContext(DbContextOptions<TasksManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DataList> DataLists { get; set; }

    public virtual DbSet<Difficulty> Difficulties { get; set; }

    public virtual DbSet<FinishedGame> FinishedGames { get; set; }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<GameReport> GameReports { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserReport> UserReports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB;Initial Catalog=MinesweeperDB;User ID=TaskAdminLogin;Password=joe123;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataList>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK__DataList__72E12F1A24573B87");
        });

        modelBuilder.Entity<Difficulty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Difficul__3213E83F708C61A2");
        });

        modelBuilder.Entity<FinishedGame>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Finished__3213E83FE6F89D59");

            entity.HasOne(d => d.User).WithMany(p => p.FinishedGames)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameIDToDifficultyID");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.FinishedGames)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameToPlayerID");
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Friends__3213E83FA7E40AAA");

            entity.HasOne(d => d.Status).WithMany(p => p.Friends)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RequestToStatus");

            entity.HasOne(d => d.UserRecieving).WithMany(p => p.FriendUserRecievings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FriendsToUser2ID");

            entity.HasOne(d => d.UserSending).WithMany(p => p.FriendUserSendings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FriendsToUser1ID");
        });

        modelBuilder.Entity<GameReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GameRepo__3213E83FE2BB7B61");

            entity.HasOne(d => d.Game).WithMany(p => p.GameReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReportToGameID");

            entity.HasOne(d => d.Status).WithMany(p => p.GameReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameReportToStatus");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Statuses__3213E83F0C35938E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F9779F460");
        });

        modelBuilder.Entity<UserReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRepo__3213E83FC7F1AD75");

            entity.HasOne(d => d.Status).WithMany(p => p.UserReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserReportToStatus");

            entity.HasOne(d => d.User).WithMany(p => p.UserReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReportToUserID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
