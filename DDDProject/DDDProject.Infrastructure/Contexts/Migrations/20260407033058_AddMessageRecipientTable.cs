using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDDProject.Infrastructure.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageRecipientTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageRecipients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReadTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageRecipients_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_IsDeleted",
                table: "MessageRecipients",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_IsRead",
                table: "MessageRecipients",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_MessageId",
                table: "MessageRecipients",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_MessageId_RecipientId",
                table: "MessageRecipients",
                columns: new[] { "MessageId", "RecipientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_RecipientId",
                table: "MessageRecipients",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_RecipientId_IsDeleted_IsRead",
                table: "MessageRecipients",
                columns: new[] { "RecipientId", "IsDeleted", "IsRead" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageRecipients");
        }
    }
}
