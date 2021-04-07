using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TriviaGame.Data.Migrations
{
    public partial class AddTriviaBoards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TriviaBoards",
                columns: table => new
                {
                    TriviaBoardId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Question = table.Column<string>(nullable: true),
                    TotalPoints = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaBoards", x => x.TriviaBoardId);
                });

            migrationBuilder.CreateTable(
                name: "TriviaAnswer",
                columns: table => new
                {
                    TriviaAnswerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Answer = table.Column<string>(nullable: true),
                    Points = table.Column<int>(nullable: false),
                    TriviaBoardId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaAnswer", x => x.TriviaAnswerId);
                    table.ForeignKey(
                        name: "FK_TriviaAnswer_TriviaBoards_TriviaBoardId",
                        column: x => x.TriviaBoardId,
                        principalTable: "TriviaBoards",
                        principalColumn: "TriviaBoardId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TriviaAnswer_TriviaBoardId",
                table: "TriviaAnswer",
                column: "TriviaBoardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TriviaAnswer");

            migrationBuilder.DropTable(
                name: "TriviaBoards");
        }
    }
}
