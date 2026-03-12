using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class Uc17_CreateOrmOperationsAndIndexes : Migration
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
                    HasError = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityMeasurementOperations", x => x.Id);
                });

            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.AuditLog', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.AuditLog
                    (
                        AuditId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditLog PRIMARY KEY,
                        TableName NVARCHAR(128) NOT NULL,
                        ActionType NVARCHAR(10) NOT NULL,
                        ActionTimestampUtc DATETIME2(7) NOT NULL CONSTRAINT DF_AuditLog_ActionTimestampUtc DEFAULT SYSUTCDATETIME(),
                        OperationId UNIQUEIDENTIFIER NULL,
                        OldRowJson NVARCHAR(MAX) NULL,
                        NewRowJson NVARCHAR(MAX) NULL
                    );
                END;

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLog_Table_ActionTime' AND object_id = OBJECT_ID(N'dbo.AuditLog'))
                BEGIN
                    CREATE INDEX IX_AuditLog_Table_ActionTime
                    ON dbo.AuditLog (TableName, ActionTimestampUtc DESC);
                END;

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLog_OperationId' AND object_id = OBJECT_ID(N'dbo.AuditLog'))
                BEGIN
                    CREATE INDEX IX_AuditLog_OperationId
                    ON dbo.AuditLog (OperationId);
                END;

                /* Trigger (DROP + CREATE via EXEC to avoid GO batch separators) */
                IF OBJECT_ID(N'dbo.tr_QuantityMeasurementOperations_Audit', N'TR') IS NOT NULL
                BEGIN
                    DROP TRIGGER dbo.tr_QuantityMeasurementOperations_Audit;
                END;

                EXEC(N'
                CREATE TRIGGER dbo.tr_QuantityMeasurementOperations_Audit
                ON dbo.QuantityMeasurementOperations
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    /* INSERT rows (present in inserted, not in deleted) */
                    INSERT INTO dbo.AuditLog (TableName, ActionType, ActionTimestampUtc, OperationId, OldRowJson, NewRowJson)
                    SELECT
                        N''QuantityMeasurementOperations'',
                        N''INSERT'',
                        SYSUTCDATETIME(),
                        i.OperationId,
                        NULL,
                        (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
                    FROM inserted i
                    LEFT JOIN deleted d ON d.OperationId = i.OperationId
                    WHERE d.OperationId IS NULL;

                    /* DELETE rows (present in deleted, not in inserted) */
                    INSERT INTO dbo.AuditLog (TableName, ActionType, ActionTimestampUtc, OperationId, OldRowJson, NewRowJson)
                    SELECT
                        N''QuantityMeasurementOperations'',
                        N''DELETE'',
                        SYSUTCDATETIME(),
                        d.OperationId,
                        (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
                        NULL
                    FROM deleted d
                    LEFT JOIN inserted i ON i.OperationId = d.OperationId
                    WHERE i.OperationId IS NULL;

                    /* UPDATE rows (present in both) */
                    INSERT INTO dbo.AuditLog (TableName, ActionType, ActionTimestampUtc, OperationId, OldRowJson, NewRowJson)
                    SELECT
                        N''QuantityMeasurementOperations'',
                        N''UPDATE'',
                        SYSUTCDATETIME(),
                        i.OperationId,
                        (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
                        (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
                    FROM inserted i
                    INNER JOIN deleted d ON d.OperationId = i.OperationId;
                END
                ');
                ");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuantityMeasurementOperations",
                schema: "dbo");

            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.tr_QuantityMeasurementOperations_Audit', N'TR') IS NOT NULL
                BEGIN
                    DROP TRIGGER dbo.tr_QuantityMeasurementOperations_Audit;
                END;

                IF OBJECT_ID(N'dbo.AuditLog', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE dbo.AuditLog;
                END;
                ");
        }
    }
}
