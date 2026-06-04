using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BandBoostAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExamStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Exams_ExamId",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "SkillCategory",
                table: "Questions",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ResourceUrl",
                table: "Questions",
                newName: "Explanation");

            migrationBuilder.RenameColumn(
                name: "QuestionType",
                table: "Questions",
                newName: "QuestionNumber");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Questions",
                newName: "ContentJson");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExamId",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ExamSectionId",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ExamSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SharedContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamSections_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ExamSectionId",
                table: "Questions",
                column: "ExamSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSections_ExamId",
                table: "ExamSections",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_ExamSections_ExamSectionId",
                table: "Questions",
                column: "ExamSectionId",
                principalTable: "ExamSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Exams_ExamId",
                table: "Questions",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ExamSections_ExamSectionId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Exams_ExamId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "ExamSections");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ExamSectionId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ExamSectionId",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Questions",
                newName: "SkillCategory");

            migrationBuilder.RenameColumn(
                name: "QuestionNumber",
                table: "Questions",
                newName: "QuestionType");

            migrationBuilder.RenameColumn(
                name: "Explanation",
                table: "Questions",
                newName: "ResourceUrl");

            migrationBuilder.RenameColumn(
                name: "ContentJson",
                table: "Questions",
                newName: "Content");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExamId",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Exams_ExamId",
                table: "Questions",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
