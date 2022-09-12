using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilleSpace.Infrastructure.Migrations
{
    public partial class ParkingZone_can_be_null : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_ParkingZones_ParkingZoneId",
                table: "Reservations");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParkingZoneId",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ParkingSpace",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_ParkingZones_ParkingZoneId",
                table: "Reservations",
                column: "ParkingZoneId",
                principalTable: "ParkingZones",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_ParkingZones_ParkingZoneId",
                table: "Reservations");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParkingZoneId",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParkingSpace",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_ParkingZones_ParkingZoneId",
                table: "Reservations",
                column: "ParkingZoneId",
                principalTable: "ParkingZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
