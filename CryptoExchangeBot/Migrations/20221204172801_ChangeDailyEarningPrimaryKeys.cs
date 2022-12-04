using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoExchangeBot.Migrations
{
    public partial class ChangeDailyEarningPrimaryKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyEarnings",
                table: "DailyEarnings");

            migrationBuilder.AlterColumn<long>(
                name: "ChatId",
                table: "DailyEarnings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyEarnings",
                table: "DailyEarnings",
                columns: new[] { "ChatId", "DateCreated" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyEarnings",
                table: "DailyEarnings");

            migrationBuilder.AlterColumn<long>(
                name: "ChatId",
                table: "DailyEarnings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyEarnings",
                table: "DailyEarnings",
                column: "ChatId");
        }
    }
}
