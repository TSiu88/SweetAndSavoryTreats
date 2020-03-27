using Microsoft.EntityFrameworkCore.Migrations;

namespace SweetAndSavoryTreats.Migrations
{
    public partial class AddFlavorDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Flavors",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Flavors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Flavors");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Flavors",
                newName: "Name");
        }
    }
}
