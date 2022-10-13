using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unidemo.Migrations
{
    public partial class RefreshTokenPropNameEdited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Refreshtoken",
                table: "AspNetUsers",
                newName: "RefreshToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "AspNetUsers",
                newName: "Refreshtoken");
        }
    }
}
