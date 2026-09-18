using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Obsidian.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RecipientUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveredAt",
                table: "Recipient",
                newName: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Recipient",
                newName: "DeliveredAt");
        }
    }
}
