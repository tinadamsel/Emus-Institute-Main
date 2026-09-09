using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class addStudyCentersAndAssessmentRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentRegistrations_CbtTests_CbtTestId",
                table: "AssessmentRegistrations");

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentRegistrations_CbtTests_CbtTestId",
                table: "AssessmentRegistrations",
                column: "CbtTestId",
                principalTable: "CbtTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentRegistrations_CbtTests_CbtTestId",
                table: "AssessmentRegistrations");

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentRegistrations_CbtTests_CbtTestId",
                table: "AssessmentRegistrations",
                column: "CbtTestId",
                principalTable: "CbtTests",
                principalColumn: "Id");
        }
    }
}
