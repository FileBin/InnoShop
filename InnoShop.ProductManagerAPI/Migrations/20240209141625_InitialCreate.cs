using InnoShop.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnoShop.Infrastructure.ProductManagerAPI.Migrations {
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:aviability_status", "draft,published,sold");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    desc = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    status = table.Column<AviabilityStatus>(type: "aviability_status", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    creation_timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update_timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
