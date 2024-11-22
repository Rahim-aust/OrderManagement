using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OrderManagement.Models;

public partial class ProductManageDbContext : DbContext
{
    public ProductManageDbContext()
    {
    }

    public ProductManageDbContext(DbContextOptions<ProductManageDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblOrder> TblOrders { get; set; }

    public virtual DbSet<TblProduct> TblProducts { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
        {

        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblOrder>(entity =>
        {
            entity.HasKey(e => e.IntOrderId).HasName("PK__tblOrder__447BBCA46794E5C1");

            entity.ToTable("tblOrders");

            entity.Property(e => e.IntOrderId).HasColumnName("intOrderId");
            entity.Property(e => e.DtOrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("dtOrderDate");
            entity.Property(e => e.IntProductId).HasColumnName("intProductId");
            entity.Property(e => e.NumQuantity)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("numQuantity");
            entity.Property(e => e.StrCustomerName)
                .HasMaxLength(100)
                .HasColumnName("strCustomerName");

            entity.HasOne(d => d.IntProduct).WithMany(p => p.TblOrders)
                .HasForeignKey(d => d.IntProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblOrders__intPr__398D8EEE");
        });

        modelBuilder.Entity<TblProduct>(entity =>
        {
            entity.HasKey(e => e.IntProductId).HasName("PK__tblProdu__06E80BE3D0BF88A0");

            entity.ToTable("tblProducts");

            entity.Property(e => e.IntProductId).HasColumnName("intProductId");
            entity.Property(e => e.NumStock)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("numStock");
            entity.Property(e => e.NumUnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("numUnitPrice");
            entity.Property(e => e.StrProductName)
                .HasMaxLength(100)
                .HasColumnName("strProductName");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.IntUserId).HasName("PK__tblUsers__AE995DCE176A63C5");

            entity.ToTable("tblUsers");

            entity.HasIndex(e => e.StrUsername, "UQ__tblUsers__2C1C10550CE886B3").IsUnique();

            entity.Property(e => e.IntUserId).HasColumnName("intUserId");
            entity.Property(e => e.IntFailedLoginAttempts).HasColumnName("intFailedLoginAttempts");
            entity.Property(e => e.IsLocked).HasColumnName("isLocked");
            entity.Property(e => e.StrPasswordHash)
                .HasMaxLength(255)
                .HasColumnName("strPasswordHash");
            entity.Property(e => e.StrUsername)
                .HasMaxLength(100)
                .HasColumnName("strUsername");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
