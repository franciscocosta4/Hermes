using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hermes.Migrations
{
    /// <inheritdoc />
    public partial class CreateSavingsGoalTableWithFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "minimum_balance_to_keep",
                table: "SavingsGoals",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "percentage_of_income",
                table: "SavingsGoals",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "minimum_balance_to_keep",
                table: "SavingsGoals");

            migrationBuilder.DropColumn(
                name: "percentage_of_income",
                table: "SavingsGoals");
        }
    }
}
