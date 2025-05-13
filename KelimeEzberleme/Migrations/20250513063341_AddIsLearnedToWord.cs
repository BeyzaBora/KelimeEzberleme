using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KelimeEzberleme.Migrations
{
    /// <inheritdoc />
    public partial class AddIsLearnedToWord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLearned",
                table: "Words",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLearned",
                table: "Words");
        }
    }
}
