using Microsoft.EntityFrameworkCore.Migrations;

namespace Hatra.Messenger.Migrations
{
    public partial class UpdateSP_GetChatHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var q = @"ALTER PROCEDURE [dbo].[GetChatHistory]
	@userId bigint
AS
BEGIN
select 
c.Id as ChatId,
cpOther.UserId as UserId,
(isnull(c.Title,cp.Title)) as Title,
(isnull(c.LogoAddress,cp.LogoAddress)) as LogoAddress,
0 as UnreadCount,
cc.id as LastContent_Id ,     
cc.UserId as LastContent_UserId  ,
cc.Text as LastContent_Text    ,
cc.CreationTime as LastContent_DateTime

from  ChatParticipants cp
join Chats c on c.Id = cp.ChatId
join ChatParticipants cpOther on cpOther.ChatId = c.Id and cpOther.UserId <> @userId --Nedd To Change For Group Chats
left join ChatContents cc on cc.ChatId = c.Id
left outer join ChatContents cc1 on cc1.ChatId = c.Id and (cc.CreationTime < cc1.CreationTime)--greatest-n-per-group on chat contents

where 
cp.UserId = @userId
and c.IsDeleted = 0
and cc1.Id is null
END

";
            migrationBuilder.Sql(q);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
