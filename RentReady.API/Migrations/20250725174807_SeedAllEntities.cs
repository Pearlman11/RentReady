using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RentReady.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "Email", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "alice@example.com", "Alice Johnson", "555-1234" },
                    { 2, "bob@example.com", "Bob Williams", "555-5678" }
                });

            migrationBuilder.InsertData(
                table: "Leases",
                columns: new[] { "Id", "EndDate", "PropertyId", "StartDate", "TenantId" },
                values: new object[,]
                {
                    { 1, null, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, null, 2, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "Date", "LeaseId" },
                values: new object[,]
                {
                    { 1, 1200m, new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 1500m, new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Leases",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Leases",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
