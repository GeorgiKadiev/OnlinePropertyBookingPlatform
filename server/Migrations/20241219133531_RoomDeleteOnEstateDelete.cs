using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlinePropertyBookingPlatform.Migrations
{
    /// <inheritdoc />
    public partial class RoomDeleteOnEstateDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "room_ibfk_1",
                table: "room");

            migrationBuilder.AddForeignKey(
                name: "room_ibfk_1",
                table: "room",
                column: "EstateId",
                principalTable: "estate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "room_ibfk_1",
                table: "room");

            migrationBuilder.AddForeignKey(
                name: "room_ibfk_1",
                table: "room",
                column: "EstateId",
                principalTable: "estate",
                principalColumn: "Id");
        }
    }
}
