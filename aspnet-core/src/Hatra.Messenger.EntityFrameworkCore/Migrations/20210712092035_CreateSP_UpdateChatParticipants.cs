using Microsoft.EntityFrameworkCore.Migrations;

namespace Hatra.Messenger.Migrations
{
    public partial class CreateSP_UpdateChatParticipants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var q = @"Create PROCEDURE [dbo].[UpdateChatParticipants]
	@userId bigint,
	@logoAddress nvarchar(MAX),
	@title nvarchar(50)
AS
BEGIN
	begin tran
update cc1 set cc1.Title = @title,cc1.LogoAddress =@logoAddress from ChatParticipants cc 
join ChatParticipants cc1 on cc.ChatId = cc1.ChatId and cc1.UserId <> @userId
where cc.UserId  = @userId
commit tran
END
";
            migrationBuilder.Sql(q);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
