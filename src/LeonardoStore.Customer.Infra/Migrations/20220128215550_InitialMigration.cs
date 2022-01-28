using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LeonardoStore.Customer.Infra.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    first_name = table.Column<string>(type: "varchar(40)", nullable: true),
                    last_name = table.Column<string>(type: "varchar(40)", nullable: true),
                    document = table.Column<string>(type: "varchar(14)", nullable: true),
                    document_type = table.Column<int>(type: "int", nullable: true),
                    email = table.Column<string>(type: "varchar(150)", nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    Street = table.Column<string>(type: "varchar(80)", nullable: false),
                    Number = table.Column<string>(type: "varchar(30)", nullable: false),
                    Neighborhood = table.Column<string>(type: "varchar(80)", nullable: false),
                    City = table.Column<string>(type: "varchar(80)", nullable: false),
                    State = table.Column<string>(type: "varchar(20)", nullable: false),
                    Country = table.Column<string>(type: "varchar(80)", nullable: false),
                    ZipCode = table.Column<string>(type: "varchar(9)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_address_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_address_CustomerId",
                table: "address",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "customers");
        }
    }
}
