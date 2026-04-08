using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDDProject.Infrastructure.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageRevokedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RevokedBy",
                table: "Messages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedTime",
                table: "Messages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "MessageRecipients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedTime",
                table: "MessageRecipients",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RevokedBy",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RevokedTime",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "MessageRecipients");

            migrationBuilder.DropColumn(
                name: "RevokedTime",
                table: "MessageRecipients");
        }
    }
}
