using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace PostWork.Migrations.Post
{
    public partial class PostMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatorId = table.Column<string>(type: "varchar(100)", nullable: false),
                    Tags = table.Column<string>(type: "varchar(150)", nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Avatar = table.Column<byte[]>(nullable: true),
                    SalaryMin = table.Column<long>(nullable: false),
                    SalaryMax = table.Column<long>(nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
