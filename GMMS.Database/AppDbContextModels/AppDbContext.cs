using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GMMS.Database.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblMember> TblMembers { get; set; }

    public virtual DbSet<TblMembership> TblMemberships { get; set; }

    public virtual DbSet<TblMembershipPlan> TblMembershipPlans { get; set; }

    public virtual DbSet<TblPayment> TblPayments { get; set; }

    public virtual DbSet<TblPaymentMethod> TblPaymentMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblMember>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Tbl_Memb__0CF04B1805BB98F3");

            entity.ToTable("Tbl_Member");

            entity.HasIndex(e => e.MemberCode, "UQ__Tbl_Memb__84CA637700FA42E5").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MemberCode).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<TblMembership>(entity =>
        {
            entity.HasKey(e => e.MembershipId).HasName("PK__Tbl_Memb__92A786791A0BD763");

            entity.ToTable("Tbl_Membership");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Member).WithMany(p => p.TblMemberships)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Membership_Member");

            entity.HasOne(d => d.MembershipPlan).WithMany(p => p.TblMemberships)
                .HasForeignKey(d => d.MembershipPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Membership_MembershipPlan");
        });

        modelBuilder.Entity<TblMembershipPlan>(entity =>
        {
            entity.HasKey(e => e.MembershipPlanId).HasName("PK__Tbl_Memb__8E444BB63E2A6ABC");

            entity.ToTable("Tbl_MembershipPlan");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<TblPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Tbl_Paym__9B556A38D39F22DB");

            entity.ToTable("Tbl_Payment");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Sspath)
                .HasMaxLength(500)
                .HasColumnName("SSPath");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Membership).WithMany(p => p.TblPayments)
                .HasForeignKey(d => d.MembershipId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Membership");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.TblPayments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_PaymentMethod");
        });

        modelBuilder.Entity<TblPaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__Tbl_Paym__DC31C1D3D827C608");

            entity.ToTable("Tbl_PaymentMethod");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
