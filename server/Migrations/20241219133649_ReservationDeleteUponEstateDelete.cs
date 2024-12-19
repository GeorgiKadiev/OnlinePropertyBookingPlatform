using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlinePropertyBookingPlatform.Migrations
{
    /// <inheritdoc />
    public partial class ReservationDeleteUponEstateDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "reservation_ibfk_2",
                table: "reservation");

            migrationBuilder.AddForeignKey(
                name: "reservation_ibfk_2",
                table: "reservation",
                column: "EstateId",
                principalTable: "estate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "reservation_ibfk_2",
                table: "reservation");

            migrationBuilder.AddForeignKey(
                name: "reservation_ibfk_2",
                table: "reservation",
                column: "EstateId",
                principalTable: "estate",
                principalColumn: "Id");
        }
    }
}
