using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application_Form.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationForms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ApplicationDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    ApplicationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RedirectUri = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Environment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpectedRequestVolume = table.Column<int>(type: "int", nullable: true),
                    AcceptTerms = table.Column<bool>(type: "bit", nullable: false),
                    PrivacyPolicyUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DataRetentionDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TechnicalContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TechnicalContactEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    ApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationForms_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationForms_ClientId",
                table: "ApplicationForms",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationForms");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
