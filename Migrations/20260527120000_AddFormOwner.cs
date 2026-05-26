using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniForm.Migrations
{
    /// <inheritdoc />
    public partial class AddFormOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Forms",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_CreatedByUserId",
                table: "Forms",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Users_CreatedByUserId",
                table: "Forms",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Users_CreatedByUserId",
                table: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_Forms_CreatedByUserId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Forms");
        }
    }
}
