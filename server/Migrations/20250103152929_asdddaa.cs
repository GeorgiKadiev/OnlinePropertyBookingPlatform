using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlinePropertyBookingPlatform.Migrations
{
    /// <inheritdoc />
    public partial class asdddaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EstateId",
                table: "room",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CheckOutDate",
                table: "reservation",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CheckInDate",
                table: "reservation",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservation_room_RoomId",
                table: "reservation");

            migrationBuilder.DropIndex(
                name: "IX_reservation_RoomId",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "reservation");

            migrationBuilder.AlterColumn<int>(
                name: "EstateId",
                table: "room",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CheckOutDate",
                table: "reservation",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CheckInDate",
                table: "reservation",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
