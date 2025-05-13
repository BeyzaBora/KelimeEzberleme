using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KelimeEzberleme.Migrations
{
    /// <inheritdoc />
    public partial class AddNextRepeatAndLastAnsweredToWordProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "WordProgresses",
                newName: "NextRepeat");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAnswered",
                table: "WordProgresses",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAnswered",
                table: "WordProgresses");

            migrationBuilder.RenameColumn(
                name: "NextRepeat",
                table: "WordProgresses",
                newName: "LastUpdated");
        }
    }
}
