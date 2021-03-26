using Microsoft.EntityFrameworkCore.Migrations;

namespace CircusGroupsBot.Migrations
{
    public partial class AddedCommandMessageIdToEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "CommandMessageId",
                table: "Events",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommandMessageId",
                table: "Events");
        }
    }
}
