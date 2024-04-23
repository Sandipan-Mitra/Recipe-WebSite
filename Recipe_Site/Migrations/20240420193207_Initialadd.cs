using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipe_Site.Migrations
{
    /// <inheritdoc />
    public partial class Initialadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblUsers",
                columns: table => new
                {
                    EmailId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fname = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Lname = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DOB = table.Column<DateTime>(type: "Date", nullable: false),
                    ProfilePic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OTP = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsers", x => x.EmailId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblUsers");
        }
    }
}
