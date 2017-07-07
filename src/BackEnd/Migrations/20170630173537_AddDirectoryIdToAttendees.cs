using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BackEnd.Migrations
{
    public partial class AddDirectoryIdToAttendees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Attendees_AttendeeID",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_AttendeeID",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "AttendeeID",
                table: "Sessions");

            migrationBuilder.AddColumn<string>(
                name: "DirectoryObjectId",
                table: "Attendees",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SessionAttendee",
                columns: table => new
                {
                    SessionID = table.Column<int>(type: "int", nullable: false),
                    AttendeeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionAttendee", x => new { x.SessionID, x.AttendeeID });
                    table.ForeignKey(
                        name: "FK_SessionAttendee_Attendees_AttendeeID",
                        column: x => x.AttendeeID,
                        principalTable: "Attendees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionAttendee_Sessions_SessionID",
                        column: x => x.SessionID,
                        principalTable: "Sessions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendees_DirectoryObjectId",
                table: "Attendees",
                column: "DirectoryObjectId",
                unique: true,
                filter: "[DirectoryObjectId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SessionAttendee_AttendeeID",
                table: "SessionAttendee",
                column: "AttendeeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionAttendee");

            migrationBuilder.DropIndex(
                name: "IX_Attendees_DirectoryObjectId",
                table: "Attendees");

            migrationBuilder.DropColumn(
                name: "DirectoryObjectId",
                table: "Attendees");

            migrationBuilder.AddColumn<int>(
                name: "AttendeeID",
                table: "Sessions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_AttendeeID",
                table: "Sessions",
                column: "AttendeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Attendees_AttendeeID",
                table: "Sessions",
                column: "AttendeeID",
                principalTable: "Attendees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
