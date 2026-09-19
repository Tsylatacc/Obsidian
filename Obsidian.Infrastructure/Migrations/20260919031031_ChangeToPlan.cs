using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Obsidian.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "MessagedUsed",
                table: "SubscriptionUsage",
                newName: "MessagesUsed");

            migrationBuilder.RenameColumn(
                name: "SubscriptionPlanId",
                table: "Subscriptions",
                newName: "PlanId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_SubscriptionPlanId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_PlanId");

            migrationBuilder.AddColumn<int>(
                name: "CampaignsUsed",
                table: "SubscriptionUsage",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SubscriptionId1",
                table: "SubscriptionUsage",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "CampaignLimit",
                table: "SubscriptionPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecipientPerCampaignLimit",
                table: "SubscriptionPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionUsage_SubscriptionId1",
                table: "SubscriptionUsage",
                column: "SubscriptionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_SubscriptionPlans_PlanId",
                table: "Subscriptions",
                column: "PlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionUsage_Subscriptions_SubscriptionId1",
                table: "SubscriptionUsage",
                column: "SubscriptionId1",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_SubscriptionPlans_PlanId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionUsage_Subscriptions_SubscriptionId1",
                table: "SubscriptionUsage");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionUsage_SubscriptionId1",
                table: "SubscriptionUsage");

            migrationBuilder.DropColumn(
                name: "CampaignsUsed",
                table: "SubscriptionUsage");

            migrationBuilder.DropColumn(
                name: "SubscriptionId1",
                table: "SubscriptionUsage");

            migrationBuilder.DropColumn(
                name: "CampaignLimit",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "RecipientPerCampaignLimit",
                table: "SubscriptionPlans");

            migrationBuilder.RenameColumn(
                name: "MessagesUsed",
                table: "SubscriptionUsage",
                newName: "MessagedUsed");

            migrationBuilder.RenameColumn(
                name: "PlanId",
                table: "Subscriptions",
                newName: "SubscriptionPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_PlanId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_SubscriptionPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "Subscriptions",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
