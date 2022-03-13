using DiffingAPI.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DefiningAPI.Migrations
{
    [DbContext(typeof(DiffContext))]
    [Migration("CreateDiffTable_2203111535")]
    public class CreateDiffTable_2203111535: Migration 
    {
        protected override void Up (MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable (
             name: "Diff",
             columns: table => new
             {
                 Id = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),

                 DiffKey = table.Column<int>(nullable: false),
                 Data = table.Column<string>(nullable: false),
                 Side = table.Column<string>(nullable: false),
                 Size = table.Column<int>(nullable: true)
             },
             constraints: table =>
             {
                 table.PrimaryKey("PK_Id_Key", x => x.Id);
             });

        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name:"Diff");
        }

    }
}