using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class addPublicAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "CbtTests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublicAssessment",
                table: "CbtTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "StudentUserId",
                table: "CbtAttempts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "AssessmentRegistrationId",
                table: "CbtAttempts",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Payments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "AssessmentRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramType = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScholarshipType = table.Column<int>(type: "int", nullable: false),
                    CbtTestId = table.Column<int>(type: "int", nullable: false),
                    AccessToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    HasOpenedQuiz = table.Column<bool>(type: "bit", nullable: false),
                    AttemptId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentRegistrations_CbtTests_CbtTestId",
                        column: x => x.CbtTestId,
                        principalTable: "CbtTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_AssessmentRegistrations_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentRegistrations_CbtTestId",
                table: "AssessmentRegistrations",
                column: "CbtTestId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentRegistrations_PaymentId",
                table: "AssessmentRegistrations",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CbtAttempts_AssessmentRegistrationId",
                table: "CbtAttempts",
                column: "AssessmentRegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CbtAttempts_AssessmentRegistrations_AssessmentRegistrationId",
                table: "CbtAttempts",
                column: "AssessmentRegistrationId",
                principalTable: "AssessmentRegistrations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CbtAttempts_AssessmentRegistrations_AssessmentRegistrationId",
                table: "CbtAttempts");

            migrationBuilder.DropTable(
                name: "AssessmentRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_CbtAttempts_AssessmentRegistrationId",
                table: "CbtAttempts");

            migrationBuilder.DropColumn(
                name: "AssessmentRegistrationId",
                table: "CbtAttempts");

            migrationBuilder.DropColumn(
                name: "IsPublicAssessment",
                table: "CbtTests");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "CbtTests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StudentUserId",
                table: "CbtAttempts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
