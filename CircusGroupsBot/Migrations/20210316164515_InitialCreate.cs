using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CircusGroupsBot.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeaderUserID = table.Column<ulong>(nullable: false),
                    EventName = table.Column<string>(nullable: false),
                    DateAndTime = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Tanks = table.Column<int>(nullable: false),
                    Healers = table.Column<int>(nullable: false),
                    Dds = table.Column<int>(nullable: false),
                    Runners = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
