using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Common.Migrations
{
    public partial class AddSubscriberIntoEventTracker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subscriber",
                table: "EventTracker",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subscriber",
                table: "EventTracker");
        }
    }
}
