﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlinePropertyBookingPlatform;

#nullable disable

namespace OnlinePropertyBookingPlatform.Migrations
{
    [DbContext(typeof(PropertyManagementContext))]
    [Migration("20241211141046_ResetPasswordTokenToDb")]
    partial class ResetPasswordTokenToDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4");
            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Amenity", b =>
                {
                    b.Property<int>("EstateId")
                        .HasColumnType("int");

                    b.Property<string>("AmenityName")
                        .HasColumnType("varchar(255)");

                    b.HasKey("EstateId", "AmenityName")
                        .HasName("PRIMARY")
                        .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                    b.ToTable("amenities", (string)null);
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Estate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("EstateOwnerId")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<double>("PricePerNight")
                        .HasColumnType("double");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "EstateOwnerId" }, "EstateOwnerId");

                    b.ToTable("estate", (string)null);
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Amount")
                        .HasColumnType("double");

                    b.Property<DateOnly?>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Method")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("ReservationId")
                        .HasColumnType("int");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "ReservationId" }, "ReservationId");

                    b.ToTable("payment", (string)null);
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Reservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly?>("CheckInDate")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("CheckOutDate")
                        .HasColumnType("date");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("int");

                    b.Property<int?>("EstateId")
                        .HasColumnType("int");

                    b.Property<bool?>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<double?>("TotalPrice")
                        .HasColumnType("double");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "CustomerId" }, "CustomerId");

                    b.HasIndex(new[] { "EstateId" }, "EstateId");

                    b.ToTable("reservation", (string)null);
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AuthorId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<DateOnly?>("Date")
                        .HasColumnType("date");

                    b.Property<int?>("EstateId")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "AuthorId" }, "AuthorId");

                    b.HasIndex(new[] { "EstateId" }, "EstateId")
                        .HasDatabaseName("EstateId1");

                    b.ToTable("review", (string)null);
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BedCount")
                        .HasColumnType("int");

                    b.Property<int?>("EstateId")
                        .HasColumnType("int");

                    b.Property<int?>("MaxGuests")
                        .HasColumnType("int");

                    b.Property<string>("RoomType")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "EstateId" }, "EstateId")
                        .HasDatabaseName("EstateId2");

                    b.ToTable("room", (string)null);
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ResetPasswordToken")
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("enum('Customer','EstateOwner','Admin')");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "Email" }, "Email")
                        .IsUnique();

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Amenity", b =>
                {
                    b.HasOne("OnlinePropertyBookingPlatform.Models.Estate", "Estate")
                        .WithMany("Amenities")
                        .HasForeignKey("EstateId")
                        .IsRequired()
                        .HasConstraintName("amenities_ibfk_1");

                    b.Navigation("Estate");
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Estate", b =>
                {
                    b.HasOne("OnlinePropertyBookingPlatform.Models.User", "EstateOwner")
                        .WithMany("Estates")
                        .HasForeignKey("EstateOwnerId")
                        .HasConstraintName("estate_ibfk_1");

                    b.Navigation("EstateOwner");
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Payment", b =>
                {
                    b.HasOne("OnlinePropertyBookingPlatform.Models.Reservation", "Reservation")
                        .WithMany("Payments")
                        .HasForeignKey("ReservationId")
                        .HasConstraintName("payment_ibfk_1");

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Reservation", b =>
                {
                    b.HasOne("OnlinePropertyBookingPlatform.Models.User", "Customer")
                        .WithMany("Reservations")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("reservation_ibfk_1");

                    b.HasOne("OnlinePropertyBookingPlatform.Models.Estate", "Estate")
                        .WithMany("Reservations")
                        .HasForeignKey("EstateId")
                        .HasConstraintName("reservation_ibfk_2");

                    b.Navigation("Customer");

                    b.Navigation("Estate");
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Review", b =>
                {
                    b.HasOne("OnlinePropertyBookingPlatform.Models.User", "Author")
                        .WithMany("Reviews")
                        .HasForeignKey("AuthorId")
                        .HasConstraintName("review_ibfk_2");

                    b.HasOne("OnlinePropertyBookingPlatform.Models.Estate", "Estate")
                        .WithMany("Reviews")
                        .HasForeignKey("EstateId")
                        .HasConstraintName("review_ibfk_1");

                    b.Navigation("Author");

                    b.Navigation("Estate");
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Room", b =>
                {
                    b.HasOne("OnlinePropertyBookingPlatform.Models.Estate", "Estate")
                        .WithMany("Rooms")
                        .HasForeignKey("EstateId")
                        .HasConstraintName("room_ibfk_1");

                    b.Navigation("Estate");
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Estate", b =>
                {
                    b.Navigation("Amenities");

                    b.Navigation("Reservations");

                    b.Navigation("Reviews");

                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Reservation", b =>
                {
                    b.Navigation("Payments");
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.User", b =>
                {
                    b.Navigation("Estates");

                    b.Navigation("Reservations");

                    b.Navigation("Reviews");
                });
#pragma warning restore 612, 618
        }
    }
}
