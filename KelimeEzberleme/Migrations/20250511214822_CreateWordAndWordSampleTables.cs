using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KelimeEzberleme.Migrations
{
    /// <inheritdoc />
    public partial class CreateWordAndWordSampleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    WordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EngWordName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TurWordName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.WordID);
                });

            migrationBuilder.CreateTable(
                name: "WordSamples",
                columns: table => new
                {
                    WordSamplesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Samples = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WordID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordSamples", x => x.WordSamplesID);
                    table.ForeignKey(
                        name: "FK_WordSamples_Words_WordID",
                        column: x => x.WordID,
                        principalTable: "Words",
                        principalColumn: "WordID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordSamples_WordID",
                table: "WordSamples",
                column: "WordID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordSamples");

            migrationBuilder.DropTable(
                name: "Words");
        }
    }
}
