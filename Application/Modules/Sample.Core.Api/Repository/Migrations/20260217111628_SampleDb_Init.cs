using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasheer.Core.Api.Repository.Migrations
{
    /// <inheritdoc />
    public partial class SampleDb_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Order",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Primary key of the entity.")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The price of the order before tax."),
                    Vat = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The value-added tax (VAT) applied to the order."),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The total price of the order including VAT."),
                    OrderType = table.Column<short>(type: "smallint", nullable: false, comment: "The type of the order (Pending, Completed, Cancelled, etc.)."),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Unique identifier (GUID) for external references and API queries."),
                    Created = table.Column<DateTime>(type: "datetime2(7)", nullable: false, comment: "Date and time when the record was created."),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Username or identifier of the user who created the record."),
                    LastModified = table.Column<DateTime>(type: "datetime2(7)", nullable: true, comment: "Date and time when the record was last modified."),
                    LastModifiedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Username or identifier of the user who last modified the record.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Primary key of the entity.")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false, comment: "Foreign key to the associated order."),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "The name or description of the order item."),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Unique identifier (GUID) for external references and API queries."),
                    Created = table.Column<DateTime>(type: "datetime2(7)", nullable: false, comment: "Date and time when the record was created."),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Username or identifier of the user who created the record."),
                    LastModified = table.Column<DateTime>(type: "datetime2(7)", nullable: true, comment: "Date and time when the record was last modified."),
                    LastModifiedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Username or identifier of the user who last modified the record.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_Created",
                schema: "dbo",
                table: "Order",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderType",
                schema: "dbo",
                table: "Order",
                column: "OrderType");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Uuid",
                schema: "dbo",
                table: "Order",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_Created",
                schema: "dbo",
                table: "OrderItem",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                schema: "dbo",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_Uuid",
                schema: "dbo",
                table: "OrderItem",
                column: "Uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Order",
                schema: "dbo");
        }
    }
}
