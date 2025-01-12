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
    [Migration("20250111203410_UpdateReservationDates")]
    partial class UpdateReservationDates
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

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("EstateId", "AmenityName")
                        .HasName("PRIMARY")
                        .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                    b.ToTable("amenities", (string)null);

                    b.HasData(
                        new
                        {
                            EstateId = 1,
                            AmenityName = "Air Conditioning",
                            Id = 1
                        },
                        new
                        {
                            EstateId = 1,
                            AmenityName = "Wi-Fi",
                            Id = 2
                        },
                        new
                        {
                            EstateId = 2,
                            AmenityName = "Parking",
                            Id = 3
                        },
                        new
                        {
                            EstateId = 3,
                            AmenityName = "Swimming Pool",
                            Id = 4
                        },
                        new
                        {
                            EstateId = 4,
                            AmenityName = "Eco-Friendly",
                            Id = 5
                        },
                        new
                        {
                            EstateId = 5,
                            AmenityName = "DigitalNomad-Friendly",
                            Id = 6
                        },
                        new
                        {
                            EstateId = 6,
                            AmenityName = "Hair Dryer",
                            Id = 7
                        },
                        new
                        {
                            EstateId = 7,
                            AmenityName = "Fridge",
                            Id = 8
                        },
                        new
                        {
                            EstateId = 8,
                            AmenityName = "Smoker-Friendly",
                            Id = 9
                        },
                        new
                        {
                            EstateId = 9,
                            AmenityName = "Fitness Centre",
                            Id = 10
                        });
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.Estate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("EstateOwnerId")
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

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.EstatePhoto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EstateId")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("estatephotos", (string)null);
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

                    b.Property<DateOnly>("CheckInDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("CheckOutDate")
                        .HasColumnType("date");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("int");

                    b.Property<int?>("EstateId")
                        .HasColumnType("int");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.Property<bool?>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<double?>("TotalPrice")
                        .HasColumnType("double");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex("RoomId");

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

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int>("EstateId")
                        .HasColumnType("int");

                    b.Property<int?>("MaxGuests")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("RoomType")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "EstateId" }, "EstateId")
                        .HasDatabaseName("EstateId2");

                    b.ToTable("room", (string)null);
                });

            modelBuilder.Entity("OnlinePropertyBookingPlatform.Models.RoomPhoto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("roomphotos", (string)null);
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

                    b.Property<string>("EmailVerificationToken")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<short>("PhoneNumber")
                        .HasColumnType("smallint");

                    b.Property<string>("ResetPasswordToken")
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("enum('Customer','EstateOwner','Admin')");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

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
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
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
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("reservation_ibfk_2");

                    b.HasOne("OnlinePropertyBookingPlatform.Models.Room", "room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Estate");

                    b.Navigation("room");
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
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
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
