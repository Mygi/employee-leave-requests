using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vypex.CodingChallenge.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AccumulatedAnnualLeaveDays = table.Column<int>(type: "INT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", maxLength: 30, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });
            migrationBuilder.CreateTable(
               name: "EmployeeLeave",
               columns: table => new
               {
                   Id = table.Column<long>(type: "INT", nullable: false),
                   EmployeeId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                   CalculatedLeaveDays = table.Column<int>(type: "INT", nullable: false),
                   LeaveType = table.Column<int>(type: "INT", nullable: false),
                   LeaveStartDate = table.Column<DateTime>(type: "TEXT", maxLength: 30, nullable: false),
                   LeaveEndDate = table.Column<DateTime>(type: "TEXT", maxLength: 30, nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_EmployeeLeave", x => x.Id);
                   table.ForeignKey(name:"FK_Employee_EmployeerLeave", 
                                    principalTable: "Employees",
                                    column: x => x.EmployeeId,
                                    principalColumn: "Id");
               });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "EmployeeLeave");
        }
    }
}
