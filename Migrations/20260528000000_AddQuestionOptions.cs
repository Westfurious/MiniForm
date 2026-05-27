using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniForm.Migrations;

public partial class AddQuestionOptions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Type",
            table: "Questions",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<Guid>(
            name: "SelectedOptionId",
            table: "Answers",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "QuestionOptions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                Text = table.Column<string>(type: "text", nullable: false),
                Order = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                table.ForeignKey(
                    name: "FK_QuestionOptions_Questions_QuestionId",
                    column: x => x.QuestionId,
                    principalTable: "Questions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_QuestionOptions_QuestionId",
            table: "QuestionOptions",
            column: "QuestionId");

        migrationBuilder.CreateIndex(
            name: "IX_Answers_SelectedOptionId",
            table: "Answers",
            column: "SelectedOptionId");

        migrationBuilder.AddForeignKey(
            name: "FK_Answers_QuestionOptions_SelectedOptionId",
            table: "Answers",
            column: "SelectedOptionId",
            principalTable: "QuestionOptions",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Answers_QuestionOptions_SelectedOptionId", table: "Answers");
        migrationBuilder.DropIndex(name: "IX_Answers_SelectedOptionId", table: "Answers");
        migrationBuilder.DropTable(name: "QuestionOptions");
        migrationBuilder.DropColumn(name: "SelectedOptionId", table: "Answers");
        migrationBuilder.DropColumn(name: "Type", table: "Questions");
    }
}