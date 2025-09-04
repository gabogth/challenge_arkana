using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transaction_ms.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    source_account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    target_account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    transfer_type_id = table.Column<int>(type: "int", nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_created_at",
                table: "transactions",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_source_account_id_created_at",
                table: "transactions",
                columns: new[] { "source_account_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transactions");
        }
    }
}
