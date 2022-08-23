using Microsoft.EntityFrameworkCore.Migrations;

namespace SLBFEMS.Migrations
{
    public partial class cvvalidation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IsParsportValidated",
                table: "JobSeekerData",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IsBirthCertificateValidated",
                table: "JobSeekerData",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "IsCvValidated",
                table: "JobSeekerData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCvValidated",
                table: "JobSeekerData");

            migrationBuilder.AlterColumn<bool>(
                name: "IsParsportValidated",
                table: "JobSeekerData",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBirthCertificateValidated",
                table: "JobSeekerData",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
