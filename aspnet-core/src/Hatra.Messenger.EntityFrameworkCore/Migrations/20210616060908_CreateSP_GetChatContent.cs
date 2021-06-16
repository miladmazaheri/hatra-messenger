using Microsoft.EntityFrameworkCore.Migrations;

namespace Hatra.Messenger.Migrations
{
    public partial class CreateSP_GetChatContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatContents_ChatId",
                table: "ChatContents");

            migrationBuilder.CreateIndex(
                name: "IX_ChatContents_ChatId_CreationTime",
                table: "ChatContents",
                columns: new[] { "ChatId", "CreationTime" });

            var q = @"CREATE PROCEDURE [dbo].[GetChatContent]
	@count int,
	@chatId uniqueidentifier,
	@baseDateTime datetime
AS
BEGIN
select top(@count)
cc.Id as Id, 
cc.UserId as UserId, 
cc.ChatId as ChatId, 
cc.MediaAddress as MediaAddress, 
cc.ThumbnailAddress as ThumbnailAddress, 
cc.[Text] as [Text], 
cc.ReplyOfId as ReplyOfId
from ChatContents cc
where cc.ChatId = @chatId and cc.CreationTime < @baseDateTime
order by cc.CreationTime desc
END";
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatContents_ChatId_CreationTime",
                table: "ChatContents");

            migrationBuilder.CreateIndex(
                name: "IX_ChatContents_ChatId",
                table: "ChatContents",
                column: "ChatId");
        }
    }
}
