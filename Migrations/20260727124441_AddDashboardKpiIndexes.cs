using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza_API.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardKpiIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_orders_completed_at",
                table: "orders",
                column: "completed_at",
                filter: "completed_at IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_orders_created_at",
                table: "orders",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_orders_ready_at",
                table: "orders",
                column: "ready_at",
                filter: "ready_at IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_orders_status",
                table: "orders",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_orders_completed_at",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_orders_created_at",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_orders_ready_at",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_orders_status",
                table: "orders");
        }
    }
}
