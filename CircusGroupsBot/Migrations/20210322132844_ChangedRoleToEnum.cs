using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CircusGroupsBot.Migrations
{
    public partial class ChangedRoleToEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Signup_Roles_RoleId",
                table: "Signup");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Signup_RoleId",
                table: "Signup");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Signup");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Signup",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Signup");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Signup",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Name" },
                values: new object[,]
                {
                    { 1, "Tank" },
                    { 2, "Healer" },
                    { 3, "DD" },
                    { 4, "Runner" },
                    { 5, "Maybe" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Signup_RoleId",
                table: "Signup",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Signup_Roles_RoleId",
                table: "Signup",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
