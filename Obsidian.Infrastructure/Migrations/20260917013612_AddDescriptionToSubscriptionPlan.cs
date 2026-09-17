using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Obsidian.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToSubscriptionPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SubscriptionPlans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Campaigns",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Campaigns");
        }
    }
}
