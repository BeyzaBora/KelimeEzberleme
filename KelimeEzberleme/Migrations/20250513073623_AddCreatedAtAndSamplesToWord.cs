using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KelimeEzberleme.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtAndSamplesToWord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 'NextReviewDate' sütununu kaldırma
            migrationBuilder.DropColumn(
                name: "NextReviewDate",
                table: "WordProgresses");

            // 'CreatedAt' sütunu ekleme
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Words",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.Now);  // Varsayılan olarak bugünün tarihini ver

            // 'EngWordName' ve 'TurWordName' sütunlarını nullable yapmak
            migrationBuilder.AlterColumn<string>(
                name: "TurWordName",
                table: "Words",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EngWordName",
                table: "Words",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 'EngWordName' ve 'TurWordName' sütunlarını eski haline getirme
            migrationBuilder.AlterColumn<string>(
                name: "TurWordName",
                table: "Words",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EngWordName",
                table: "Words",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // 'NextReviewDate' sütununu geri eklemek
            migrationBuilder.AddColumn<DateTime>(
                name: "NextReviewDate",
                table: "WordProgresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}