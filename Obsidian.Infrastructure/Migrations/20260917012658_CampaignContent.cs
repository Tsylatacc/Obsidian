using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Obsidian.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CampaignContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipient_Campaigns_campaign_id",
                table: "Recipient");

            migrationBuilder.DropColumn(
                name: "Media_Caption",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Media_FileName",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Media_MimeType",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Media_Type",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Media_Url",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Campaigns");

            migrationBuilder.RenameColumn(
                name: "campaign_id",
                table: "Recipient",
                newName: "CampaignId");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "Recipient",
                newName: "PhoneNumber_Value");

            migrationBuilder.RenameIndex(
                name: "IX_Recipient_campaign_id",
                table: "Recipient",
                newName: "IX_Recipient_CampaignId");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "Channels",
                newName: "PhoneNumber_Value");

            migrationBuilder.CreateTable(
                name: "CampaignContent",
                columns: table => new
                {
                    Position = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    MediaType = table.Column<string>(type: "text", nullable: true),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Caption = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignContent", x => x.Position);
                    table.ForeignKey(
                        name: "FK_CampaignContent_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignContent_CampaignId",
                table: "CampaignContent",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipient_Campaigns_CampaignId",
                table: "Recipient",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipient_Campaigns_CampaignId",
                table: "Recipient");

            migrationBuilder.DropTable(
                name: "CampaignContent");

            migrationBuilder.RenameColumn(
                name: "CampaignId",
                table: "Recipient",
                newName: "campaign_id");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber_Value",
                table: "Recipient",
                newName: "phone_number");

            migrationBuilder.RenameIndex(
                name: "IX_Recipient_CampaignId",
                table: "Recipient",
                newName: "IX_Recipient_campaign_id");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber_Value",
                table: "Channels",
                newName: "phone_number");

            migrationBuilder.AddColumn<string>(
                name: "Media_Caption",
                table: "Campaigns",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_FileName",
                table: "Campaigns",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_MimeType",
                table: "Campaigns",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Type",
                table: "Campaigns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Media_Url",
                table: "Campaigns",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Campaigns",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipient_Campaigns_campaign_id",
                table: "Recipient",
                column: "campaign_id",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
