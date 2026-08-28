using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LBPUnion.ProjectLighthouse.Migrations
{
    /// <inheritdoc />
    public partial class AddPatchworkSessionDataToGameToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PatchworkJoinKeyEnabled",
                table: "GameTokens",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatchworkMajor",
                table: "GameTokens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatchworkMinor",
                table: "GameTokens",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatchworkJoinKeyEnabled",
                table: "GameTokens");

            migrationBuilder.DropColumn(
                name: "PatchworkMajor",
                table: "GameTokens");

            migrationBuilder.DropColumn(
                name: "PatchworkMinor",
                table: "GameTokens");
        }
    }
}
