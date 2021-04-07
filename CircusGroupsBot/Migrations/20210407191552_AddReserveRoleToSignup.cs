using Microsoft.EntityFrameworkCore.Migrations;

namespace CircusGroupsBot.Migrations
{
    public partial class AddReserveRoleToSignup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReserveRole",
                table: "Signup",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReserveRole",
                table: "Signup");
        }
    }
}
