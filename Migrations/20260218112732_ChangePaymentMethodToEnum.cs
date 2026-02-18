using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza_API.Migrations
{
    /// <inheritdoc />
    public partial class ChangePaymentMethodToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payment_method",
                table: "orders");

            migrationBuilder.AddColumn<int>(
                name: "payment_method",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropColumn(
                name: "payment_method",
                table: "orders");

            
            migrationBuilder.AddColumn<string>(
                name: "payment_method",
                table: "orders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
