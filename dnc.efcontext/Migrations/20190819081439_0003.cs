using Microsoft.EntityFrameworkCore.Migrations;

namespace dnc.efcontext.Migrations
{
    public partial class _0003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SysConfig",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysConfig", x => x.Code);
                });

            migrationBuilder.InsertData("SysConfig", new[] { "Code", "Value" }, new[] { "001", "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysConfig");

            migrationBuilder.DeleteData("SysConfig", "Code", "001");
        }
    }
}
