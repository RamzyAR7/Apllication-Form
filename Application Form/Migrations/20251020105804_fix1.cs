using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application_Form.Migrations
{
    /// <inheritdoc />
    public partial class fix1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationForms_ApplicationName",
                table: "ApplicationForms");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationForms_ApplicationName",
                table: "ApplicationForms",
                column: "ApplicationName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationForms_ApplicationName",
                table: "ApplicationForms");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationForms_ApplicationName",
                table: "ApplicationForms",
                column: "ApplicationName",
                unique: true);
        }
    }
}
