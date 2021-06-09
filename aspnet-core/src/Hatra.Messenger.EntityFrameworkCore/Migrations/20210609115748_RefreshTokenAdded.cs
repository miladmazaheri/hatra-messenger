using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hatra.Messenger.Migrations
{
    public partial class RefreshTokenAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMedias_AbpUsers_CreatorUserId",
                table: "ChatMedias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMedias",
                table: "ChatMedias");

            migrationBuilder.RenameTable(
                name: "ChatMedias",
                newName: "ChatMedia");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMedias_CreatorUserId",
                table: "ChatMedia",
                newName: "IX_ChatMedia_CreatorUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMedia",
                table: "ChatMedia",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Device = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token_Device_Expires",
                table: "RefreshToken",
                columns: new[] { "Token", "Device", "Expires" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMedia_AbpUsers_CreatorUserId",
                table: "ChatMedia",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMedia_AbpUsers_CreatorUserId",
                table: "ChatMedia");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMedia",
                table: "ChatMedia");

            migrationBuilder.RenameTable(
                name: "ChatMedia",
                newName: "ChatMedias");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMedia_CreatorUserId",
                table: "ChatMedias",
                newName: "IX_ChatMedias_CreatorUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMedias",
                table: "ChatMedias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMedias_AbpUsers_CreatorUserId",
                table: "ChatMedias",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
