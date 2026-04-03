using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDDProject.Infrastructure.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class SyncMessageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPushed",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PushedTime",
                table: "Messages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IsPushed",
                table: "Messages",
                column: "IsPushed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_IsPushed",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsPushed",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "PushedTime",
                table: "Messages");
        }
    }
}
