using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSharpQuizBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddExamAndExamQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExamQuestions",
                columns: table => new
                {
                    ExamQuestionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExamId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestions", x => x.ExamQuestionId);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    ExamId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.ExamId);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionText = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Option1 = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Option2 = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Option3 = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Option4 = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CorrectAnswer = table.Column<int>(type: "INTEGER", nullable: true),
                    CorrectTextAnswer = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Passage = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeLimit = table.Column<int>(type: "INTEGER", nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Explanation = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamQuestions");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
