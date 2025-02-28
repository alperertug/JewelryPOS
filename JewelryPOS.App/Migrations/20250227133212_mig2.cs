using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JewelryPOS.App.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedById",
                table: "Products",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_CreatedById",
                table: "Products",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_CreatedById",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CreatedById",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Products");
        }
    }
}
