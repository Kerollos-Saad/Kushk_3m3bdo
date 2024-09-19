using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kushl_3m3bdo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyToWalletModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Wallets",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "IsSubscripeToPlan",
                table: "Wallets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanSubscriptionStrated",
                table: "Wallets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "Wallets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlanId",
                table: "Wallets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscripeToPlan",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "PlanSubscriptionStrated",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanId",
                table: "Wallets");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Wallets",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
