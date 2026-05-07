using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hermes.Migrations
{
    /// <inheritdoc />
    public partial class FixIdName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Budgets",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Budgets",
                newName: "id");
        }
    }
}
