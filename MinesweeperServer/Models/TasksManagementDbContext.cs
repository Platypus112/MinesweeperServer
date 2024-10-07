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

    public virtual DbSet<GameReport> GameReports { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB;Initial Catalog=MinesweeperDB;User ID=TaskAdminLogin;Password=joe123;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataList>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK__DataList__72E12F1A8E451E2E");
        });

        modelBuilder.Entity<Difficulty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Difficul__3213E83FC4C36985");
        });

        modelBuilder.Entity<FinishedGame>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Finished__3213E83F3B972794");

            entity.HasOne(d => d.User).WithMany(p => p.FinishedGames)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameIDToDifficultyID");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.FinishedGames)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameToPlayerID");
        });

        modelBuilder.Entity<GameReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GameRepo__3213E83F6857504A");

            entity.HasOne(d => d.Game).WithMany(p => p.GameReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReportToGameID");

            entity.HasOne(d => d.Status).WithMany(p => p.GameReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReportToStatus");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Statuses__3213E83F6A7608EA");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F6B3058DE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
