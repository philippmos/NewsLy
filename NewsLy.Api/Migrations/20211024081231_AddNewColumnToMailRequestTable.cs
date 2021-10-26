using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsLy.Api.Migrations
{
    public partial class AddNewColumnToMailRequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ToMailingListId",
                table: "ContactRequests",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToMailingListId",
                table: "ContactRequests");
        }
    }
}
