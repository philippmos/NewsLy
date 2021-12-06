using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsLy.Api.Migrations
{
    public partial class ExtendRecipientWithConfirmationAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmationMailSentDate",
                table: "Recipients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UserConfirmationDate",
                table: "Recipients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationMailSentDate",
                table: "Recipients");

            migrationBuilder.DropColumn(
                name: "UserConfirmationDate",
                table: "Recipients");
        }
    }
}
