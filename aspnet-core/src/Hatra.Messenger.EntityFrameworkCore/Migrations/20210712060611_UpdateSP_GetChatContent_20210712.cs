using Microsoft.EntityFrameworkCore.Migrations;

namespace Hatra.Messenger.Migrations
{
    public partial class UpdateSP_GetChatContent_20210712 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var q = @"ALTER PROCEDURE [dbo].[GetChatHistory]
	@userId bigint
AS
BEGIN
select 
temp.ChatId,
temp.UserId,
temp.Title,
temp.LogoAddress,
temp.LastContent_Id ,     
temp.LastContent_UserId ,
temp.LastContent_Text,
temp.LastContent_DateTime,
temp.LastContent_ViewCount,
temp.LastContent_ReceiveCount,
ISNULL(temp2.UnreadCount,0) as UnreadCount
 from 
(select 
c.Id as ChatId,
cpOther.UserId as UserId,
(ISNULL(c.Title,cp.Title)) as Title,
(ISNULL(c.LogoAddress,cp.LogoAddress)) as LogoAddress,
cc.id as LastContent_Id ,     
cc.UserId as LastContent_UserId  ,
cc.Text as LastContent_Text    ,
cc.CreationTime as LastContent_DateTime,
cc.ViewCount as LastContent_ViewCount,
cc.ReceiveCount as LastContent_ReceiveCount
from  ChatParticipants cp
join Chats c on c.Id = cp.ChatId
join ChatParticipants cpOther on cpOther.ChatId = c.Id and cpOther.UserId <> @userId --Need To Change For Group Chats
left join ChatContents cc on cc.ChatId = c.Id
left outer join ChatContents cc1 on cc1.ChatId = c.Id and (cc.CreationTime < cc1.CreationTime)--greatest-n-per-group on chat contents
where 
cp.UserId = @userId
and c.IsDeleted = 0
and cc1.Id is null) as temp
left join 
--to find unread count
(select cp.ChatId,COUNT(cc.Id) as UnreadCount from
ChatParticipants cp 
join ChatContents cc on cp.ChatId = cc.ChatId and cc.UserId <>@userId and cc.ViewCount =0
where cp.UserId = @userId and cp.IsDeleted = 0 
group by cp.ChatId) as temp2 
on temp.ChatId = temp2.ChatId
END";
            migrationBuilder.Sql(q);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
