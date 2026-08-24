using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace day03.Migrations
{
    /// <inheritdoc />
    public partial class day03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    SubjectId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SubjectName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.SubjectId);
                });

            migrationBuilder.CreateTable(
                name: "StudentScore",
                columns: table => new
                {
                    ScoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Score = table.Column<decimal>(type: "decimal(14,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentScore", x => x.ScoreId);
                    table.ForeignKey(
                        name: "FK_StudentScore_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId");
                });

            migrationBuilder.InsertData(
                table: "Subject",
                columns: new[] { "SubjectId", "SubjectName" },
                values: new object[,]
                {
                    { "SUB001", "C# Programming" },
                    { "SUB002", "SQL Server Database" },
                    { "SUB003", "ASP.NET Core MVC Web App" }
                });

            migrationBuilder.InsertData(
                table: "StudentScore",
                columns: new[] { "ScoreId", "Score", "StudentId", "StudentName", "SubjectId" },
                values: new object[,]
                {
                    { 1, 8.00m, "STD001", "Alex Tran", "SUB001" },
                    { 2, 7.50m, "STD001", "Alex Tran", "SUB002" },
                    { 3, 9.00m, "STD002", "Tai Pham", "SUB001" },
                    { 4, 4.00m, "STD003", "Alice Nguyen", "SUB003" },
                    { 5, 7.00m, "STD003", "Alice Nguyen", "SUB002" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentScore_SubjectId",
                table: "StudentScore",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentScore");

            migrationBuilder.DropTable(
                name: "Subject");
        }
    }
}
