using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class Uc20_AddUserIdToQuantityMeasurementOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "dbo",
                table: "QuantityMeasurementOperations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuantityMeasurementOperations_UserId",
                schema: "dbo",
                table: "QuantityMeasurementOperations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuantityMeasurementOperations_UserId",
                schema: "dbo",
                table: "QuantityMeasurementOperations");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "QuantityMeasurementOperations");
        }
    }
}
