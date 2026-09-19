using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Obsidian.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSubscriptionUsageRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionUsage_Subscriptions_SubscriptionId1",
                table: "SubscriptionUsage");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionUsage_SubscriptionId1",
                table: "SubscriptionUsage");

            migrationBuilder.DropColumn(
                name: "SubscriptionId1",
                table: "SubscriptionUsage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubscriptionId1",
                table: "SubscriptionUsage",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionUsage_SubscriptionId1",
                table: "SubscriptionUsage",
                column: "SubscriptionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionUsage_Subscriptions_SubscriptionId1",
                table: "SubscriptionUsage",
                column: "SubscriptionId1",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
