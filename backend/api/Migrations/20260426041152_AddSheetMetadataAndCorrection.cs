using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddSheetMetadataAndCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectionNotes",
                table: "AssignmentSheets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "AssignmentSheets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "AssignmentSheets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Grade",
                table: "AssignmentSheets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "AssignmentSheets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "AssignmentSheets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectionNotes",
                table: "AssignmentSheets");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "AssignmentSheets");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "AssignmentSheets");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "AssignmentSheets");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "AssignmentSheets");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "AssignmentSheets");
        }
    }
}
