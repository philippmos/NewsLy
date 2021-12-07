using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsLy.Api.Migrations
{
    public partial class ExtendBaseEntityWithCreateAndModificationDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "TrackingUrls",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "TrackingUrls",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Recipients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "Recipients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "MailRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "MailRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "MailingLists",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "MailingLists",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "TrackingUrls");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "TrackingUrls");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Recipients");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "Recipients");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "MailRequests");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "MailRequests");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "MailingLists");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "MailingLists");
        }
    }
}
