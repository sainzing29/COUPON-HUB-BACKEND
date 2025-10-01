using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CouponHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceCenterIdToCoupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceCenterId",
                table: "Coupons",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_ServiceCenterId",
                table: "Coupons",
                column: "ServiceCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_ServiceCenters_ServiceCenterId",
                table: "Coupons",
                column: "ServiceCenterId",
                principalTable: "ServiceCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_ServiceCenters_ServiceCenterId",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_ServiceCenterId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "ServiceCenterId",
                table: "Coupons");
        }
    }
}
