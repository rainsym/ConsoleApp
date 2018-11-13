using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Common.Migrations
{
    public partial class AddSubscriberToRawRabbitEventModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RawRabbitEvent",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Subscriber",
                table: "RawRabbitEvent",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RawRabbitEvent");

            migrationBuilder.DropColumn(
                name: "Subscriber",
                table: "RawRabbitEvent");
        }
    }
}
