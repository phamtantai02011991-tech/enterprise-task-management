using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PretestWDA.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbEmployee",
                columns: table => new
                {
                    EmpID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpDoB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmpName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmpAddress = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmpEmail = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbEmployee", x => x.EmpID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbEmployee");
        }
    }
}
