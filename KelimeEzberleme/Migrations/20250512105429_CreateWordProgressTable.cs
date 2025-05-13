using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KelimeEzberleme.Migrations
{
    /// <inheritdoc />
    public partial class CreateWordProgressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WordProgresses",
                columns: table => new
                {
                    WordProgressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    WordID = table.Column<int>(type: "int", nullable: false),
                    CorrectCount = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordProgresses", x => x.WordProgressID);
                    table.ForeignKey(
                        name: "FK_WordProgresses_Words_WordID",
                        column: x => x.WordID,
                        principalTable: "Words",
                        principalColumn: "WordID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordProgresses_WordID",
                table: "WordProgresses",
                column: "WordID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordProgresses");
        }
    }
}
