using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsLy.Api.Migrations
{
    public partial class SetRecipientsConfirmationDatesToNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserConfirmationDate",
                table: "Recipients");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ConfirmationMailSentDate",
                table: "Recipients",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmationDate",
                table: "Recipients",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationDate",
                table: "Recipients");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ConfirmationMailSentDate",
                table: "Recipients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserConfirmationDate",
                table: "Recipients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
