using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hermes.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
                migrationBuilder.Sql(@"
    INSERT INTO ""Transactions"" (""Amount"", ""Date"", ""Description"", ""Category"", ""Type"", ""UserId"")
    SELECT ""Amount"", ""Date"", '-', NULL, 0, ""UserId""
    FROM ""Incomes""
");
migrationBuilder.Sql(@"
    INSERT INTO ""Transactions"" (""Amount"", ""Date"", ""Description"", ""Category"", ""Type"", ""UserId"")
    SELECT e.""Amount"", e.""Date"", e.""Description"", c.""Name"", 1, e.""UserId""
    FROM ""Expenses"" e
    LEFT JOIN ""Categories"" c ON e.""CategoryId"" = c.""Id""
");
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
