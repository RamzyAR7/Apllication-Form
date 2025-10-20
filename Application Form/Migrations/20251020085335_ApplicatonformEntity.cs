using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application_Form.Migrations
{
    /// <inheritdoc />
    public partial class ApplicatonformEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiDocsUrl",
                table: "ApplicationForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationForms_ApplicationName",
                table: "ApplicationForms",
                column: "ApplicationName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationForms_ApplicationName",
                table: "ApplicationForms");

            migrationBuilder.DropColumn(
                name: "ApiDocsUrl",
                table: "ApplicationForms");
        }
    }
}
