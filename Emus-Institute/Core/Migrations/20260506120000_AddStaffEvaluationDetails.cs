using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffEvaluationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffEvaluationDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Passport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolTranscript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HighSchoolCompletionCertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherCertificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WAECScratchCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    DateApproved = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffEvaluationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffEvaluationDetails_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffEvaluationDetails_UserId",
                table: "StaffEvaluationDetails",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffEvaluationDetails");
        }
    }
}
