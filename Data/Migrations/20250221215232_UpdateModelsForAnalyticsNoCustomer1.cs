using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AshishGeneralStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsForAnalyticsNoCustomer1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "PasswordHash", "PhoneNumber", "Role", "Username" },
                values: new object[] { 1, "admin@example.com", "Admin User", "$2a$11$x4xGzWuj.FMNNUgYTs7M..gCTLaTiOcCopwJxiOX5le6yOxBPCbh.", "123-456-7890", "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
