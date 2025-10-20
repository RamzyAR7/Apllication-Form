using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application_Form.Migrations
{
    /// <inheritdoc />
    public partial class fix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationForms_ApplicationName",
                table: "ApplicationForms");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationForm_Name_ClientId",
                table: "ApplicationForms",
                columns: new[] { "ApplicationName", "ClientId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationForm_Name_ClientId",
                table: "ApplicationForms");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationForms_ApplicationName",
                table: "ApplicationForms",
                column: "ApplicationName");
        }
    }
}
