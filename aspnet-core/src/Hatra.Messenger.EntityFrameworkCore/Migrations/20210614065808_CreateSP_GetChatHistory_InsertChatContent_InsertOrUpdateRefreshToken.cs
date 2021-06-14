using Microsoft.EntityFrameworkCore.Migrations;

namespace Hatra.Messenger.Migrations
{
    public partial class CreateSP_GetChatHistory_InsertChatContent_InsertOrUpdateRefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var q_InsertOrUpdateRefreshToken = @"CREATE PROCEDURE InsertOrUpdateRefreshToken
	@userId bigint,
	@token nvarchar(max),
	@device nvarchar(50),
	@ip nvarchar(20),
	@expires datetime
AS
BEGIN
	begin tran
if exists (select id from RefreshTokens with (updlock,serializable) where UserId = @userID and Device= @device and CreatedByIp = @ip)
begin
   update RefreshTokens set Token = @token,CreationTime=GetDate(),Expires =@expires
   where UserId = @userID and Device= @device and CreatedByIp = @ip
end
else
begin
   insert into RefreshTokens (UserId,Token,Expires,CreationTime,CreatedByIp,Device) values
   (@userId,@token,@expires,GetDate(),@ip,@device)
end
commit tran
END";
            migrationBuilder.Sql(q_InsertOrUpdateRefreshToken);


            var q_GetChatHistory = @"CREATE PROCEDURE [dbo].[GetChatHistory]
	@userId bigint
AS
BEGIN
select 
c.Id as ChatId,
(isnull(c.Title,cp.Title)) as Title,
(isnull(c.LogoAddress,cp.LogoAddress)) as LogoAddress,
0 as UnreadCount,
cc.id as LastContent_Id ,     
cc.UserId as LastContent_UserId  ,
cc.Text as LastContent_Text    ,
cc.CreationTime as LastContent_DateTime

from  ChatParticipants cp
join Chats c on c.Id = cp.ChatId
left join ChatContents cc on cc.ChatId = c.Id
left outer join ChatContents cc1 on cc1.ChatId = c.Id and (cc.CreationTime < cc1.CreationTime)--greatest-n-per-group on chat contents

where 
cp.UserId = @userId
and c.IsDeleted = 0
and cc1.Id is null
END
";
            migrationBuilder.Sql(q_GetChatHistory);

            var q_InsertChatContent = @"CREATE PROCEDURE [dbo].[InsertChatContent]
	@id uniqueidentifier,
	@userId bigint,
	@chatId uniqueidentifier,
	@replyOfId uniqueidentifier,
	@text nvarchar(MAX),
	@mediaAddress nvarchar(MAX),
	@thumbnailAddress nvarchar(MAX)
AS
BEGIN
	begin tran
INSERT INTO [dbo].[ChatContents]
           ([Id]
           ,[ChatId]
           ,[UserId]
           ,[Text]
           ,[ReplyOfId]
           ,[ViewCount]
           ,[ReceiveCount]
           ,[ThumbnailAddress]
           ,[CreationTime]
           ,[MediaAddress])
     VALUES
           (@id
           ,@chatId
           ,@userId
           ,@text
           ,@replyOfId
           ,0
           ,0
           ,@thumbnailAddress
           ,getdate()
           ,@mediaAddress)
commit tran
END
";
            migrationBuilder.Sql(q_InsertChatContent);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
