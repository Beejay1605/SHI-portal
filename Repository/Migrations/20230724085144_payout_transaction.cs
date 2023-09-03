using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class payout_transaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PAYOUT_TRAN_REF",
                table: "earnings_uni_level",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PAYOUT_TRAN_REF",
                table: "earnings_referal",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PAYOUT_TRAN_REF",
                table: "earnings_pairing",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "payout_transaction",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    DISTRIBUTOR_REF = table.Column<int>(type: "int", nullable: false),
                    TOTAL_AMOUNT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CREATED_DT = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CREATED_BY = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payout_transaction", x => x.ID);
                    table.ForeignKey(
                        name: "FK_payout_transaction_distributors_details_DISTRIBUTOR_REF",
                        column: x => x.DISTRIBUTOR_REF,
                        principalTable: "distributors_details",
                        principalColumn: "DISTRIBUTOR_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payout_transaction_operations_details_CREATED_BY",
                        column: x => x.CREATED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_payout_transaction_CREATED_BY",
                table: "payout_transaction",
                column: "CREATED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_payout_transaction_DISTRIBUTOR_REF",
                table: "payout_transaction",
                column: "DISTRIBUTOR_REF");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payout_transaction");

            migrationBuilder.DropColumn(
                name: "PAYOUT_TRAN_REF",
                table: "earnings_uni_level");

            migrationBuilder.DropColumn(
                name: "PAYOUT_TRAN_REF",
                table: "earnings_referal");

            migrationBuilder.DropColumn(
                name: "PAYOUT_TRAN_REF",
                table: "earnings_pairing");
        }
    }
}
