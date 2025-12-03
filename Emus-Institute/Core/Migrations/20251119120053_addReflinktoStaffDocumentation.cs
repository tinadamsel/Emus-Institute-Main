using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class addReflinktoStaffDocumentation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPerStudent",
                table: "EvaluationDetails");

            migrationBuilder.DropColumn(
                name: "RefLink",
                table: "EvaluationDetails");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPerStudent",
                table: "StaffDocuments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefLink",
                table: "StaffDocuments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPerStudent",
                table: "StaffDocuments");

            migrationBuilder.DropColumn(
                name: "RefLink",
                table: "StaffDocuments");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPerStudent",
                table: "EvaluationDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefLink",
                table: "EvaluationDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
