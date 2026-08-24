using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace day03.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    SubjectId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    SubjectId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    { "SUB01", "C#" },
                    { "SUB02", "ASP.NET Core" },
                    { "SUB03", "SQL Server" }
                });

            migrationBuilder.InsertData(
                table: "StudentScore",
                columns: new[] { "ScoreId", "Score", "StudentId", "StudentName", "SubjectId" },
                values: new object[,]
                {
                    { 1, 8.5m, "SV001", "Nguyễn Văn A", "SUB01" },
                    { 2, 9.0m, "SV002", "Trần Thị B", "SUB02" },
                    { 3, 7.5m, "SV003", "Lê Văn C", "SUB03" }
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
