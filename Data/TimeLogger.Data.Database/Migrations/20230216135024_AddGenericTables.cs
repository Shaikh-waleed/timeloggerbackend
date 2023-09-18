using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace TimeLogger.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddGenericTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            if (config["Component:Security:SecurityService"] == "SingleSignOn")
            {

                migrationBuilder.CreateTable(
                    name: "StatusTypes",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false),
                        Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_StatusTypes", x => x.Id);
                    });

                migrationBuilder.CreateTable(
                    name: "Statuses",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        TypeId = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Statuses", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Statuses_StatusTypes_TypeId",
                            column: x => x.TypeId,
                            principalTable: "StatusTypes",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_Statuses_TypeId",
                    table: "Statuses",
                    column: "TypeId");
            }

            migrationBuilder.CreateTable(
                name: "NotificationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BCC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_NotificationTypes_NotificationTypeId",
                        column: x => x.NotificationTypeId,
                        principalTable: "NotificationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageBody = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTemplates_NotificationTypes_NotificationTypeId",
                        column: x => x.NotificationTypeId,
                        principalTable: "NotificationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationTypeId",
                table: "Notifications",
                column: "NotificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_StatusId",
                table: "Notifications",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_NotificationTypeId",
                table: "NotificationTemplates",
                column: "NotificationTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            if (config["Component:Security:SecurityService"] == "SingleSignOn")
            {

                migrationBuilder.DropTable(
                    name: "Statuses");

                migrationBuilder.DropTable(
                    name: "StatusTypes");
            }

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "NotificationTypes");
        }
    }
}
