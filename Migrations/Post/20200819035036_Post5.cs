using Microsoft.EntityFrameworkCore.Migrations;

namespace PostWork.Migrations.Post
{
    public partial class Post5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Submissions_PostId",
                table: "Submissions",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Posts_PostId",
                table: "Submissions",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Posts_PostId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_PostId",
                table: "Submissions");
        }
    }
}
