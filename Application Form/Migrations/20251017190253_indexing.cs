using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application_Form.Migrations
{
    /// <inheritdoc />
    public partial class indexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ExpirationDate",
                table: "ApplicationForms",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationForms_CreatedAt",
                table: "ApplicationForms",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationForms_IsActive",
                table: "ApplicationForms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationForms_IsDeleted",
                table: "ApplicationForms",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationForms_CreatedAt",
                table: "ApplicationForms");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationForms_IsActive",
                table: "ApplicationForms");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationForms_IsDeleted",
                table: "ApplicationForms");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpirationDate",
                table: "ApplicationForms",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
