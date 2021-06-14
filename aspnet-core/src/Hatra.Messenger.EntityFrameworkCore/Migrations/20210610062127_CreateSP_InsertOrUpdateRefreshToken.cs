using Microsoft.EntityFrameworkCore.Migrations;

namespace Hatra.Messenger.Migrations
{
    public partial class CreateSP_InsertOrUpdateRefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var q = @"CREATE PROCEDURE InsertOrUpdateRefreshToken
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
            migrationBuilder.Sql(q);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
