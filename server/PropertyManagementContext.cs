using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using OnlinePropertyBookingPlatform.Models;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace OnlinePropertyBookingPlatform;

public partial class PropertyManagementContext : DbContext
{
    public PropertyManagementContext()
    {
    }

    public PropertyManagementContext(DbContextOptions<PropertyManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<Estate> Estates { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => new { e.EstateId, e.AmenityName })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("amenities");

            entity.HasOne(d => d.Estate).WithMany(p => p.Amenities)
                .HasForeignKey(d => d.EstateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("amenities_ibfk_1");
        });

        modelBuilder.Entity<Estate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("estate");

            entity.HasIndex(e => e.EstateOwnerId, "EstateOwnerId");

            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Description);
            
            entity.HasOne(d => d.EstateOwner).WithMany(p => p.Estates)
                .HasForeignKey(d => d.EstateOwnerId)
                .HasConstraintName("estate_ibfk_1");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("payment");

            entity.HasIndex(e => e.ReservationId, "ReservationId");

            entity.Property(e => e.Method).HasMaxLength(50);

            entity.HasOne(d => d.Reservation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("payment_ibfk_1");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("reservation");

            entity.HasIndex(e => e.CustomerId, "CustomerId");

            entity.HasIndex(e => e.EstateId, "EstateId");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("reservation_ibfk_1");

            entity.HasOne(d => d.Estate).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.EstateId)
                .HasConstraintName("reservation_ibfk_2").OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.RoomId);
           
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("review");

            entity.HasIndex(e => e.AuthorId, "AuthorId");

            entity.HasIndex(e => e.EstateId, "EstateId");

            entity.Property(e => e.Comment).HasColumnType("text");

            entity.HasOne(d => d.Author).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("review_ibfk_2");

            entity.HasOne(d => d.Estate).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.EstateId)
                .HasConstraintName("review_ibfk_1");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("room");

            entity.HasIndex(e => e.EstateId, "EstateId");

            entity.Property(e => e.RoomType).HasMaxLength(50);

            entity.HasOne(d => d.Estate).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.EstateId)
                .HasConstraintName("room_ibfk_1").OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Description);
            entity.Property(e => e.Name);
            
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "Email").IsUnique();
            entity.Property(entity => entity.ResetPasswordToken);
            entity.Property(e => e.IsEmailVerified);
            entity.Property(e => e.EmailVerificationToken);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasColumnType("enum('Customer','EstateOwner','Admin')");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
