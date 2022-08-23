using Microsoft.EntityFrameworkCore.Migrations;

namespace SLBFEMS.Migrations
{
    public partial class jobseekersdatamodelupgrades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Qualifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBirthCertificateValidated",
                table: "JobSeekerData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "JobSeekerData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsParsportValidated",
                table: "JobSeekerData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "AffiliationData",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Qualifications");

            migrationBuilder.DropColumn(
                name: "IsBirthCertificateValidated",
                table: "JobSeekerData");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "JobSeekerData");

            migrationBuilder.DropColumn(
                name: "IsParsportValidated",
                table: "JobSeekerData");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "AffiliationData");
        }
    }
}
