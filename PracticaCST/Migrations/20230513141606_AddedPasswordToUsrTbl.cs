using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticaCST.Migrations
{
    /// <inheritdoc />
    public partial class AddedPasswordToUsrTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HashPassword",
                table: "Users",
                newName: "HashedPassword");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HashedPassword",
                table: "Users",
                newName: "HashPassword");
        }
    }
}
