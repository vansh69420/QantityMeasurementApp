using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class Uc22_Monolith_Postgres_Initial : Migration
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MeasurementType = table.Column<int>(type: "integer", nullable: false),
                    OperationType = table.Column<int>(type: "integer", nullable: false),
                    FirstValue = table.Column<double>(type: "double precision", nullable: false),
                    FirstUnitText = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SecondValue = table.Column<double>(type: "double precision", nullable: true),
                    SecondUnitText = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    TargetUnitText = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    EqualityResult = table.Column<bool>(type: "boolean", nullable: true),
                    ScalarResult = table.Column<double>(type: "double precision", nullable: true),
                    ResultValue = table.Column<double>(type: "double precision", nullable: true),
                    ResultUnitText = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    HasError = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityMeasurementOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "bytea", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_UserId", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RefreshTokenId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReplacedByRefreshTokenId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

                            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS dbo.""AuditLog""
                (
                    ""AuditId"" BIGINT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                    ""TableName"" character varying(128) NOT NULL,
                    ""ActionType"" character varying(10) NOT NULL,
                    ""ActionTimestampUtc"" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
                    ""OperationId"" uuid NULL,
                    ""OldRowJson"" text NULL,
                    ""NewRowJson"" text NULL
                );

                CREATE INDEX IF NOT EXISTS ""IX_AuditLog_Table_ActionTime""
                ON dbo.""AuditLog"" (""TableName"", ""ActionTimestampUtc"" DESC);

                CREATE INDEX IF NOT EXISTS ""IX_AuditLog_OperationId""
                ON dbo.""AuditLog"" (""OperationId"");

                CREATE OR REPLACE FUNCTION dbo.""fn_QuantityMeasurementOperations_Audit""()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    IF TG_OP = 'INSERT' THEN
                        INSERT INTO dbo.""AuditLog""
                        (
                            ""TableName"",
                            ""ActionType"",
                            ""ActionTimestampUtc"",
                            ""OperationId"",
                            ""OldRowJson"",
                            ""NewRowJson""
                        )
                        VALUES
                        (
                            'QuantityMeasurementOperations',
                            'INSERT',
                            CURRENT_TIMESTAMP AT TIME ZONE 'UTC',
                            NEW.""OperationId"",
                            NULL,
                            row_to_json(NEW)::text
                        );

                        RETURN NEW;
                    ELSIF TG_OP = 'DELETE' THEN
                        INSERT INTO dbo.""AuditLog""
                        (
                            ""TableName"",
                            ""ActionType"",
                            ""ActionTimestampUtc"",
                            ""OperationId"",
                            ""OldRowJson"",
                            ""NewRowJson""
                        )
                        VALUES
                        (
                            'QuantityMeasurementOperations',
                            'DELETE',
                            CURRENT_TIMESTAMP AT TIME ZONE 'UTC',
                            OLD.""OperationId"",
                            row_to_json(OLD)::text,
                            NULL
                        );

                        RETURN OLD;
                    ELSIF TG_OP = 'UPDATE' THEN
                        INSERT INTO dbo.""AuditLog""
                        (
                            ""TableName"",
                            ""ActionType"",
                            ""ActionTimestampUtc"",
                            ""OperationId"",
                            ""OldRowJson"",
                            ""NewRowJson""
                        )
                        VALUES
                        (
                            'QuantityMeasurementOperations',
                            'UPDATE',
                            CURRENT_TIMESTAMP AT TIME ZONE 'UTC',
                            NEW.""OperationId"",
                            row_to_json(OLD)::text,
                            row_to_json(NEW)::text
                        );

                        RETURN NEW;
                    END IF;

                    RETURN NULL;
                END;
                $$;

                DROP TRIGGER IF EXISTS ""tr_QuantityMeasurementOperations_Audit"" ON dbo.""QuantityMeasurementOperations"";

                CREATE TRIGGER ""tr_QuantityMeasurementOperations_Audit""
                AFTER INSERT OR UPDATE OR DELETE
                ON dbo.""QuantityMeasurementOperations""
                FOR EACH ROW
                EXECUTE FUNCTION dbo.""fn_QuantityMeasurementOperations_Audit""();
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

            migrationBuilder.CreateIndex(
                name: "IX_QuantityMeasurementOperations_UserId",
                schema: "dbo",
                table: "QuantityMeasurementOperations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_RefreshTokenId",
                schema: "dbo",
                table: "RefreshTokens",
                column: "RefreshTokenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                schema: "dbo",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "dbo",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "dbo",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserId",
                schema: "dbo",
                table: "Users",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "dbo",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS ""tr_QuantityMeasurementOperations_Audit"" ON dbo.""QuantityMeasurementOperations"";
                DROP FUNCTION IF EXISTS dbo.""fn_QuantityMeasurementOperations_Audit""();
                DROP TABLE IF EXISTS dbo.""AuditLog"";
            ");
            migrationBuilder.DropTable(
                name: "QuantityMeasurementOperations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");

        }
    }
}
