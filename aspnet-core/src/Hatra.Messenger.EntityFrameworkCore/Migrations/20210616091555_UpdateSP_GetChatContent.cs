using Microsoft.EntityFrameworkCore.Migrations;

namespace Hatra.Messenger.Migrations
{
    public partial class UpdateSP_GetChatContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var q = @"alter PROCEDURE [dbo].[GetChatContent]
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
cc.ReplyOfId as ReplyOfId,
cc.CreationTime as CreationTime
from ChatContents cc
where cc.ChatId = @chatId and cc.CreationTime < @baseDateTime
order by cc.CreationTime desc
END

";
            migrationBuilder.Sql(q);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
