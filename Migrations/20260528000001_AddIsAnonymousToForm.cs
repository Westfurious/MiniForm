using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniForm.Migrations;

public partial class AddIsAnonymousToForm : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsAnonymous",
            table: "Forms",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "IsAnonymous", table: "Forms");
    }
}