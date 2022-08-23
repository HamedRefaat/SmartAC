using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Theoremone.SmartAc.Infrastructure.Data.Migrations.Sqlite
{
    public partial class AddAlertsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    AlertId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceSerialNumber = table.Column<string>(type: "TEXT", nullable: false),
                    AlertType = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerCreateDateTime = table.Column<string>(type: "TEXT", nullable: false),
                    SensoreCreateDateTime = table.Column<string>(type: "TEXT", nullable: false),
                    SensoreUpdateDateTime = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    ViewState = table.Column<int>(type: "INTEGER", nullable: false),
                    AlertResolve = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.AlertId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");
        }
    }
}
