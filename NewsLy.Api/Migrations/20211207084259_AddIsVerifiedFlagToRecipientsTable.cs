using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsLy.Api.Migrations
{
    public partial class AddIsVerifiedFlagToRecipientsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Recipients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Recipients");
        }
    }
}
