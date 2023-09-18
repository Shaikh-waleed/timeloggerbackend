using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace TimeLogger.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            if (config["Component:Security:SecurityService"] == "AspnetIdentity")
            {
                migrationBuilder.CreateTable(
                    name: "AspNetRoles",
                    columns: table => new
                    {
                        Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                        NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                        ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                    });

                migrationBuilder.CreateTable(
                    name: "Companies",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        Website = table.Column<string>(type: "nvarchar(max)", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Companies", x => x.Id);
                    });

                migrationBuilder.CreateTable(
                    name: "Countries",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                        Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        Iso = table.Column<string>(type: "nvarchar(max)", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Countries", x => x.Id);
                    });

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
                    name: "TwoFactorTypes",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false),
                        Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_TwoFactorTypes", x => x.Id);
                    });

                migrationBuilder.CreateTable(
                    name: "AspNetRoleClaims",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                        table.ForeignKey(
                            name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                            column: x => x.RoleId,
                            principalTable: "AspNetRoles",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: "Cities",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                        Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        Iso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        CountryId = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Cities", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Cities_Countries_CountryId",
                            column: x => x.CountryId,
                            principalTable: "Countries",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
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

                migrationBuilder.CreateTable(
                    name: "AspNetUsers",
                    columns: table => new
                    {
                        Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        Picture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        TwoFactorTypeId = table.Column<int>(type: "int", nullable: false),
                        CompanyId = table.Column<int>(type: "int", nullable: true),
                        StatusId = table.Column<int>(type: "int", nullable: false),
                        UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                        NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                        Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                        NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                        EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                        PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                        TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                        LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                        LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                        AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                        table.ForeignKey(
                            name: "FK_AspNetUsers_Companies_CompanyId",
                            column: x => x.CompanyId,
                            principalTable: "Companies",
                            principalColumn: "Id");
                        table.ForeignKey(
                            name: "FK_AspNetUsers_Statuses_StatusId",
                            column: x => x.StatusId,
                            principalTable: "Statuses",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                        table.ForeignKey(
                            name: "FK_AspNetUsers_TwoFactorTypes_TwoFactorTypeId",
                            column: x => x.TwoFactorTypeId,
                            principalTable: "TwoFactorTypes",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: "Addresses",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                        CompanyId = table.Column<int>(type: "int", nullable: true),
                        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        CountryId = table.Column<int>(type: "int", nullable: true),
                        CityId = table.Column<int>(type: "int", nullable: true),
                        IsDefault = table.Column<bool>(type: "bit", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Addresses", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Addresses_AspNetUsers_UserId",
                            column: x => x.UserId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id");
                        table.ForeignKey(
                            name: "FK_Addresses_Cities_CityId",
                            column: x => x.CityId,
                            principalTable: "Cities",
                            principalColumn: "Id");
                        table.ForeignKey(
                            name: "FK_Addresses_Companies_CompanyId",
                            column: x => x.CompanyId,
                            principalTable: "Companies",
                            principalColumn: "Id");
                        table.ForeignKey(
                            name: "FK_Addresses_Countries_CountryId",
                            column: x => x.CountryId,
                            principalTable: "Countries",
                            principalColumn: "Id");
                    });

                migrationBuilder.CreateTable(
                    name: "AspNetUserClaims",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                        table.ForeignKey(
                            name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                            column: x => x.UserId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: "AspNetUserLogins",
                    columns: table => new
                    {
                        LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                        table.ForeignKey(
                            name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                            column: x => x.UserId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: "AspNetUserRoles",
                    columns: table => new
                    {
                        UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                        table.ForeignKey(
                            name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                            column: x => x.RoleId,
                            principalTable: "AspNetRoles",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                        table.ForeignKey(
                            name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                            column: x => x.UserId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: "AspNetUserTokens",
                    columns: table => new
                    {
                        UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                        table.ForeignKey(
                            name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                            column: x => x.UserId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: "PreviousPasswords",
                    columns: table => new
                    {
                        PasswordHash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_PreviousPasswords", x => new { x.PasswordHash, x.UserId });
                        table.ForeignKey(
                            name: "FK_PreviousPasswords_AspNetUsers_UserId",
                            column: x => x.UserId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_Addresses_CityId",
                    table: "Addresses",
                    column: "CityId");

                migrationBuilder.CreateIndex(
                    name: "IX_Addresses_CompanyId",
                    table: "Addresses",
                    column: "CompanyId");

                migrationBuilder.CreateIndex(
                    name: "IX_Addresses_CountryId",
                    table: "Addresses",
                    column: "CountryId");

                migrationBuilder.CreateIndex(
                    name: "IX_Addresses_UserId",
                    table: "Addresses",
                    column: "UserId");

                migrationBuilder.CreateIndex(
                    name: "IX_AspNetRoleClaims_RoleId",
                    table: "AspNetRoleClaims",
                    column: "RoleId");

                migrationBuilder.CreateIndex(
                    name: "RoleNameIndex",
                    table: "AspNetRoles",
                    column: "NormalizedName",
                    unique: true,
                    filter: "[NormalizedName] IS NOT NULL");

                migrationBuilder.CreateIndex(
                    name: "IX_AspNetUserClaims_UserId",
                    table: "AspNetUserClaims",
                    column: "UserId");

                migrationBuilder.CreateIndex(
                    name: "IX_AspNetUserLogins_UserId",
                    table: "AspNetUserLogins",
                    column: "UserId");

                migrationBuilder.CreateIndex(
                    name: "IX_AspNetUserRoles_RoleId",
                    table: "AspNetUserRoles",
                    column: "RoleId");

                migrationBuilder.CreateIndex(
                    name: "EmailIndex",
                    table: "AspNetUsers",
                    column: "NormalizedEmail");

                migrationBuilder.CreateIndex(
                    name: "IX_AspNetUsers_CompanyId",
                    table: "AspNetUsers",
                    column: "CompanyId");

                migrationBuilder.CreateIndex(
                    name: "IX_AspNetUsers_StatusId",
                    table: "AspNetUsers",
                    column: "StatusId");

                migrationBuilder.CreateIndex(
                    name: "IX_AspNetUsers_TwoFactorTypeId",
                    table: "AspNetUsers",
                    column: "TwoFactorTypeId");

                migrationBuilder.CreateIndex(
                    name: "UserNameIndex",
                    table: "AspNetUsers",
                    column: "NormalizedUserName",
                    unique: true,
                    filter: "[NormalizedUserName] IS NOT NULL");

                migrationBuilder.CreateIndex(
                    name: "IX_Cities_CountryId",
                    table: "Cities",
                    column: "CountryId");

                migrationBuilder.CreateIndex(
                    name: "IX_PreviousPasswords_UserId",
                    table: "PreviousPasswords",
                    column: "UserId");

                migrationBuilder.CreateIndex(
                    name: "IX_Statuses_TypeId",
                    table: "Statuses",
                    column: "TypeId");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            if (config["Component:Security:SecurityService"] == "AspnetIdentity")
            {
                migrationBuilder.DropTable(
                name: "Addresses");

                migrationBuilder.DropTable(
                    name: "AspNetRoleClaims");

                migrationBuilder.DropTable(
                    name: "AspNetUserClaims");

                migrationBuilder.DropTable(
                    name: "AspNetUserLogins");

                migrationBuilder.DropTable(
                    name: "AspNetUserRoles");

                migrationBuilder.DropTable(
                    name: "AspNetUserTokens");

                migrationBuilder.DropTable(
                    name: "PreviousPasswords");

                migrationBuilder.DropTable(
                    name: "Cities");

                migrationBuilder.DropTable(
                    name: "AspNetRoles");

                migrationBuilder.DropTable(
                    name: "AspNetUsers");

                migrationBuilder.DropTable(
                    name: "Countries");

                migrationBuilder.DropTable(
                    name: "Companies");

                migrationBuilder.DropTable(
                    name: "Statuses");

                migrationBuilder.DropTable(
                    name: "TwoFactorTypes");

                migrationBuilder.DropTable(
                    name: "StatusTypes");
            }
        }
    }
}
