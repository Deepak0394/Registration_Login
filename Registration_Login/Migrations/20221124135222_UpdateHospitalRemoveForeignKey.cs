using Microsoft.EntityFrameworkCore.Migrations;

namespace Registration_Login.Migrations
{
    public partial class UpdateHospitalRemoveForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_hospital_doctorlists_doctorlistId",
                table: "hospital");

            migrationBuilder.DropIndex(
                name: "IX_hospital_doctorlistId",
                table: "hospital");

            migrationBuilder.DropColumn(
                name: "doctorlistId",
                table: "hospital");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "doctorlistId",
                table: "hospital",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_hospital_doctorlistId",
                table: "hospital",
                column: "doctorlistId");

            migrationBuilder.AddForeignKey(
                name: "FK_hospital_doctorlists_doctorlistId",
                table: "hospital",
                column: "doctorlistId",
                principalTable: "doctorlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
