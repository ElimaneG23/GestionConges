using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionConges.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "TenantSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "TenantSettings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Notifications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "LeaveTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "LeaveTypes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "LeaveRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "LeaveRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "LeaveBalances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "LeaveBalances",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Holidays",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "Holidays",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "LeaveTypes");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "LeaveBalances");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "LeaveBalances");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "Holidays");
        }
    }
}
