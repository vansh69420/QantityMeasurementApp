using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class Uc22_AddGoogleAuthSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthProvider",
                schema: "dbo",
                table: "Users",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Local");

            migrationBuilder.AddColumn<string>(
                name: "GoogleSubject",
                schema: "dbo",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuthProvider_GoogleSubject",
                schema: "dbo",
                table: "Users",
                columns: new[] { "AuthProvider", "GoogleSubject" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_AuthProvider_GoogleSubject",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AuthProvider",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GoogleSubject",
                schema: "dbo",
                table: "Users");
        }
    }
}
