using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace etrade_core.persistence.Migrations.Domain
{
    /// <inheritdoc />
    public partial class ConvertOrderStatusToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Önce yeni bir int kolonu ekle
            migrationBuilder.AddColumn<int>(
                name: "StatusInt",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Mevcut string değerleri int'e çevir
            migrationBuilder.Sql(@"
                UPDATE ""Orders"" 
                SET ""StatusInt"" = CASE 
                    WHEN ""Status"" = 'Pending' THEN 0
                    WHEN ""Status"" = 'Confirmed' THEN 1
                    WHEN ""Status"" = 'Processing' THEN 2
                    WHEN ""Status"" = 'Shipped' THEN 3
                    WHEN ""Status"" = 'Delivered' THEN 4
                    WHEN ""Status"" = 'Cancelled' THEN 5
                    WHEN ""Status"" = 'Refunded' THEN 6
                    ELSE 0
                END
            ");

            // Eski string kolonu sil
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            // Yeni int kolonunu Status olarak yeniden adlandır
            migrationBuilder.RenameColumn(
                name: "StatusInt",
                table: "Orders",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Önce yeni bir string kolonu ekle
            migrationBuilder.AddColumn<string>(
                name: "StatusString",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "Pending");

            // Mevcut int değerleri string'e çevir
            migrationBuilder.Sql(@"
                UPDATE ""Orders"" 
                SET ""StatusString"" = CASE 
                    WHEN ""Status"" = 0 THEN 'Pending'
                    WHEN ""Status"" = 1 THEN 'Confirmed'
                    WHEN ""Status"" = 2 THEN 'Processing'
                    WHEN ""Status"" = 3 THEN 'Shipped'
                    WHEN ""Status"" = 4 THEN 'Delivered'
                    WHEN ""Status"" = 5 THEN 'Cancelled'
                    WHEN ""Status"" = 6 THEN 'Refunded'
                    ELSE 'Pending'
                END
            ");

            // Eski int kolonu sil
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            // Yeni string kolonunu Status olarak yeniden adlandır
            migrationBuilder.RenameColumn(
                name: "StatusString",
                table: "Orders",
                newName: "Status");
        }
    }
}
