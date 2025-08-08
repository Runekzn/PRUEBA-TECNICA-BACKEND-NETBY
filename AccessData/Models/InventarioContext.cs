using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AccessData.Models;

public partial class InventarioContext : DbContext
{
    public InventarioContext()
    {
    }

    public InventarioContext(DbContextOptions<InventarioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC071C9AFB3D");

            entity.Property(e => e.CatNombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07C00E6E50");

            entity.Property(e => e.ProDescripcion)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.ProImagen).IsUnicode(false);
            entity.Property(e => e.ProNombre)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.ProPrecio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ProCategoriaNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_Categorias");

            entity.HasOne(d => d.ProCreatedByNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProCreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07612B66ED");

            entity.Property(e => e.RolName).HasMaxLength(100);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC073792E145");

            entity.Property(e => e.TraDescripcion)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TraFechaRealizada).HasColumnType("datetime");
            entity.Property(e => e.TraPrecioTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TraPrecioUnitario).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TraTipoTransaccion)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.TraProductoNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TraProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_Transacciones");

            entity.HasOne(d => d.TraUserExecutedNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TraUserExecuted)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaccion_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07239346E0");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserEmail).HasMaxLength(100);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC071342018B");

            entity.ToTable("UserRole");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
