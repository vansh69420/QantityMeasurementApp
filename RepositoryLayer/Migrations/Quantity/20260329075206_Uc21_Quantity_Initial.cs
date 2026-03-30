using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.Migrations.Quantity
{
    /// <inheritdoc />
    public partial class Uc21_Quantity_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "QuantityMeasurementOperations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeasurementType = table.Column<int>(type: "int", nullable: false),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    FirstValue = table.Column<double>(type: "float", nullable: false),
                    FirstUnitText = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SecondValue = table.Column<double>(type: "float", nullable: true),
                    SecondUnitText = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    TargetUnitText = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    EqualityResult = table.Column<bool>(type: "bit", nullable: true),
                    ScalarResult = table.Column<double>(type: "float", nullable: true),
                    ResultValue = table.Column<double>(type: "float", nullable: true),
                    ResultUnitText = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HasError = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityMeasurementOperations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuantityMeasurementOperations_MeasurementType",
                schema: "dbo",
                table: "QuantityMeasurementOperations",
                column: "MeasurementType");

            migrationBuilder.CreateIndex(
                name: "IX_QuantityMeasurementOperations_OperationId",
                schema: "dbo",
                table: "QuantityMeasurementOperations",
                column: "OperationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuantityMeasurementOperations_OperationType",
                schema: "dbo",
                table: "QuantityMeasurementOperations",
                column: "OperationType");

            migrationBuilder.CreateIndex(
                name: "IX_QuantityMeasurementOperations_TimestampUtc",
                schema: "dbo",
                table: "QuantityMeasurementOperations",
                column: "TimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_QuantityMeasurementOperations_UserId",
                schema: "dbo",
                table: "QuantityMeasurementOperations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuantityMeasurementOperations",
                schema: "dbo");
        }
    }
}
