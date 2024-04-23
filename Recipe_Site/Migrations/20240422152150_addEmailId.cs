using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipe_Site.Migrations
{
    /// <inheritdoc />
    public partial class addEmailId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailID",
                table: "tblRecipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailID",
                table: "tblRecipes");
        }
    }
}
