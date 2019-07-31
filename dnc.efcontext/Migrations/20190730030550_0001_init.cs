using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dnc.efcontext.Migrations
{
    public partial class _0001_init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    GoodsCode = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    GoodsName = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    LowestPrice = table.Column<decimal>(type: "decimal(8, 2)", nullable: true),
                    LowestPriceTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CurPrice = table.Column<decimal>(type: "decimal(8, 2)", nullable: false),
                    PlusPrice = table.Column<decimal>(type: "decimal(8, 2)", nullable: true),
                    DiscountPrice = table.Column<decimal>(type: "decimal(8, 2)", nullable: true),
                    DiscountDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpiderTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.GoodsCode);
                });

            migrationBuilder.CreateTable(
                name: "HisPrices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GoodsCode = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CurPrice = table.Column<decimal>(type: "decimal(8, 2)", nullable: false),
                    PlusPrice = table.Column<decimal>(type: "decimal(8, 2)", nullable: true),
                    DiscountPrice = table.Column<decimal>(type: "decimal(8, 2)", nullable: true),
                    DiscountDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpiderTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HisPrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proxy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IP = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Port = table.Column<int>(nullable: false),
                    IsValid = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proxy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuartzInfos",
                columns: table => new
                {
                    Guid = table.Column<string>(nullable: false),
                    TriggerGroup = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    TriggerName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CronExpression = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    FullClassName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    JobGroup = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    JobName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuartzInfos", x => x.Guid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Proxy_IsValid",
                table: "Proxy",
                column: "IsValid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "HisPrices");

            migrationBuilder.DropTable(
                name: "Proxy");

            migrationBuilder.DropTable(
                name: "QuartzInfos");
        }
    }
}
