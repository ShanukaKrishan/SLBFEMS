using Microsoft.EntityFrameworkCore.Migrations;

namespace SLBFEMS.Migrations
{
    public partial class primarykeychanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JobSeekerData",
                table: "JobSeekerData");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "JobSeekerData");

            migrationBuilder.AlterColumn<string>(
                name: "PassportFileName",
                table: "JobSeekerData",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BirthCertificateFileName",
                table: "JobSeekerData",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "NIC",
                table: "JobSeekerData",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobSeekerData",
                table: "JobSeekerData",
                column: "NIC");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JobSeekerData",
                table: "JobSeekerData");

            migrationBuilder.DropColumn(
                name: "NIC",
                table: "JobSeekerData");

            migrationBuilder.AlterColumn<string>(
                name: "PassportFileName",
                table: "JobSeekerData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BirthCertificateFileName",
                table: "JobSeekerData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "JobSeekerData",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobSeekerData",
                table: "JobSeekerData",
                column: "Id");
        }
    }
}
