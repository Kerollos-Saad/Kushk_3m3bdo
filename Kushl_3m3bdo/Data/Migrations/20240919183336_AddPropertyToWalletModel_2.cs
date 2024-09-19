using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kushl_3m3bdo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyToWalletModel_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isDebts",
                table: "Wallets",
                newName: "IsDebts");

            migrationBuilder.RenameColumn(
                name: "PlanSubscriptionStrated",
                table: "Wallets",
                newName: "WalletCreatedDate");

            migrationBuilder.RenameColumn(
                name: "IsSubscripeToPlan",
                table: "Wallets",
                newName: "IsSubscribeToPlan");

            migrationBuilder.AddColumn<bool>(
                name: "DebtRequest",
                table: "Wallets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPurchases",
                table: "Wallets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfSubscriptionPlans",
                table: "Wallets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanSubscriptionStartDate",
                table: "Wallets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "PriceOfPurchases",
                table: "Wallets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DebtRequest",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "NumberOfPurchases",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "NumberOfSubscriptionPlans",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "PlanSubscriptionStartDate",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "PriceOfPurchases",
                table: "Wallets");

            migrationBuilder.RenameColumn(
                name: "IsDebts",
                table: "Wallets",
                newName: "isDebts");

            migrationBuilder.RenameColumn(
                name: "WalletCreatedDate",
                table: "Wallets",
                newName: "PlanSubscriptionStrated");

            migrationBuilder.RenameColumn(
                name: "IsSubscribeToPlan",
                table: "Wallets",
                newName: "IsSubscripeToPlan");
        }
    }
}
