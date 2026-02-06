using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyPlanner.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyTasks_Categories_CategoryId",
                table: "StudyTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyTasks_Subjects_SubjectId",
                table: "StudyTasks");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyTasks_Categories_CategoryId",
                table: "StudyTasks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyTasks_Subjects_SubjectId",
                table: "StudyTasks",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyTasks_Categories_CategoryId",
                table: "StudyTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyTasks_Subjects_SubjectId",
                table: "StudyTasks");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyTasks_Categories_CategoryId",
                table: "StudyTasks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyTasks_Subjects_SubjectId",
                table: "StudyTasks",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
