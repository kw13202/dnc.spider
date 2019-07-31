using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dnc.efcontext.Migrations
{
    public partial class _0002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuartzInfos",
                table: "QuartzInfos");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "QuartzInfos");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "QuartzInfos",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuartzInfos",
                table: "QuartzInfos",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuartzInfos",
                table: "QuartzInfos");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "QuartzInfos");

            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "QuartzInfos",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuartzInfos",
                table: "QuartzInfos",
                column: "Guid");
        }
    }
}
