using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class addIsAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Departments_CourseId",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Assignments",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_CourseId",
                table: "Assignments",
                newName: "IX_Assignments_DepartmentId");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Departments_DepartmentId",
                table: "Assignments",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Departments_DepartmentId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Assignments",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_DepartmentId",
                table: "Assignments",
                newName: "IX_Assignments_CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Departments_CourseId",
                table: "Assignments",
                column: "CourseId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
