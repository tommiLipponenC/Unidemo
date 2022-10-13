using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unidemo.Migrations
{
    public partial class StartDateAndGraduationDateColumnsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GraduationDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GraduationDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "AspNetUsers");
        }
    }
}
