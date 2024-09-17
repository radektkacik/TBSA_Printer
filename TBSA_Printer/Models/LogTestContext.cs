using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TBSA_Printer.Models;

public partial class LogTestContext : DbContext
{
    public LogTestContext()
    {
    }

    public LogTestContext(DbContextOptions<LogTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Eftest> Eftests { get; set; }

    public virtual DbSet<LogEvent> LogEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.130.31;Database=LogTest;User Id=sa;Password=Taurid1*;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Eftest>(entity =>
        {
            entity.ToTable("EFTest");

            entity.Property(e => e.DateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Text).HasMaxLength(50);
        });

        modelBuilder.Entity<LogEvent>(entity =>
        {
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
