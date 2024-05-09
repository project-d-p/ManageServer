using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchingClient.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToLoginId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_LoginId",
                table: "Users",
                column: "LoginId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_LoginId",
                table: "Users");
        }
    }
}
