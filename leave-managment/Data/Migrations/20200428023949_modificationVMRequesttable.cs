using Microsoft.EntityFrameworkCore.Migrations;

namespace leave_managment.Data.Migrations
{
    public partial class modificationVMRequesttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestComments",
                table: "leaveRequestVMs",
                maxLength: 300,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestComments",
                table: "leaveRequestVMs");
        }
    }
}
