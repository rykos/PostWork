using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PostWork.Migrations.Post
{
    public partial class Post2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Cv",
                table: "Submissions",
                type: "MEDIUMBLOB",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "blob",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Cv",
                table: "Submissions",
                type: "blob",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "MEDIUMBLOB",
                oldNullable: true);
        }
    }
}
