using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameColumnsForTablesUserNotificationsPosOperations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "UserNotifications",
                newName: "PushNotificationToken");

            migrationBuilder.RenameColumn(
                name: "SendMessageDateTime",
                table: "UserNotifications",
                newName: "DateTimeSent");

            migrationBuilder.RenameColumn(
                name: "AuditResponseDateTime",
                table: "PosOperations",
                newName: "AuditCompletionDateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PushNotificationToken",
                table: "UserNotifications",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "DateTimeSent",
                table: "UserNotifications",
                newName: "SendMessageDateTime");

            migrationBuilder.RenameColumn(
                name: "AuditCompletionDateTime",
                table: "PosOperations",
                newName: "AuditResponseDateTime");
        }
    }
}
