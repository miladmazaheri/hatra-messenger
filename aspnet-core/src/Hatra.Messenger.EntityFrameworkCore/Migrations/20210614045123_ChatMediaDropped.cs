using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hatra.Messenger.Migrations
{
    public partial class ChatMediaDropped : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AbpUsers_UserId",
                table: "RefreshToken");

            migrationBuilder.DropTable(
                name: "ChatMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "LogoMediaId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LogoMediaId",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "ChatContents");

            migrationBuilder.DropColumn(
                name: "AvatarMediaId",
                table: "AbpUsers");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                newName: "RefreshTokens");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshToken_Token_Device_Expires",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_Token_Device_Expires");

            migrationBuilder.AddColumn<string>(
                name: "LogoAddress",
                table: "Chats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoAddress",
                table: "ChatParticipants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MediaAddress",
                table: "ChatContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarAddress",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AbpUsers_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AbpUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "LogoAddress",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LogoAddress",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "MediaAddress",
                table: "ChatContents");

            migrationBuilder.DropColumn(
                name: "AvatarAddress",
                table: "AbpUsers");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "RefreshToken");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshToken",
                newName: "IX_RefreshToken_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_Token_Device_Expires",
                table: "RefreshToken",
                newName: "IX_RefreshToken_Token_Device_Expires");

            migrationBuilder.AddColumn<Guid>(
                name: "LogoMediaId",
                table: "Chats",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LogoMediaId",
                table: "ChatParticipants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MediaId",
                table: "ChatContents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AvatarMediaId",
                table: "AbpUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ChatMedia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMedia_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMedia_CreatorUserId",
                table: "ChatMedia",
                column: "CreatorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AbpUsers_UserId",
                table: "RefreshToken",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
