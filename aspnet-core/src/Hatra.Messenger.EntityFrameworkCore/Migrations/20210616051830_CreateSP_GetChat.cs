using Microsoft.EntityFrameworkCore.Migrations;

namespace Hatra.Messenger.Migrations
{
    public partial class CreateSP_GetChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var q = @"CREATE PROCEDURE [dbo].[GetChat]
	@userId bigint,
	@receiverId bigint
AS
BEGIN
select top(1) cp.ChatId from
ChatParticipants cp
join Chats c on c.id = cp.ChatId
join ChatParticipants cp1 on cp1.ChatId = c.Id and cp1.UserId = @receiverId
where cp.UserId = @userId and c.IsDeleted = 0 and c.ChatType = 0
order by c.CreationTime
END";
            migrationBuilder.Sql(q);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
