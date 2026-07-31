using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza_API.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderHistoryIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_orders_status_created_at_id",
                table: "orders",
                columns: new[] { "status", "created_at", "id" },
                descending: new[] { false, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_orders_status_created_at_id",
                table: "orders");
        }
    }
}
