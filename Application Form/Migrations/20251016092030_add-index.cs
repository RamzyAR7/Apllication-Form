using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application_Form.Migrations
{
    /// <inheritdoc />
    public partial class addindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ApplicationForms_ApprovalStatus",
                table: "ApplicationForms",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationForms_ExpirationDate",
                table: "ApplicationForms",
                column: "ExpirationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationForms_ApprovalStatus",
                table: "ApplicationForms");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationForms_ExpirationDate",
                table: "ApplicationForms");
        }
    }
}
