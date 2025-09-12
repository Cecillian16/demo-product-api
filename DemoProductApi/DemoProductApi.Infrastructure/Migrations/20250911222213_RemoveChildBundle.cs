using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoProductApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveChildBundle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleItems_Bundles_ChildBundleId",
                table: "BundleItems");

            migrationBuilder.DropIndex(
                name: "IX_BundleItems_BundleId_ChildBundleId",
                table: "BundleItems");

            migrationBuilder.DropIndex(
                name: "IX_BundleItems_BundleId_ChildProductItemId",
                table: "BundleItems");

            migrationBuilder.DropIndex(
                name: "IX_BundleItems_ChildBundleId",
                table: "BundleItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BundleItem_ExactlyOneChild",
                table: "BundleItems");

            migrationBuilder.DropColumn(
                name: "ChildBundleId",
                table: "BundleItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChildProductItemId",
                table: "BundleItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_BundleId",
                table: "BundleItems",
                column: "BundleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BundleItems_BundleId",
                table: "BundleItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChildProductItemId",
                table: "BundleItems",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ChildBundleId",
                table: "BundleItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_BundleId_ChildBundleId",
                table: "BundleItems",
                columns: new[] { "BundleId", "ChildBundleId" },
                unique: true,
                filter: "\"ChildBundleId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_BundleId_ChildProductItemId",
                table: "BundleItems",
                columns: new[] { "BundleId", "ChildProductItemId" },
                unique: true,
                filter: "\"ChildProductItemId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_ChildBundleId",
                table: "BundleItems",
                column: "ChildBundleId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BundleItem_ExactlyOneChild",
                table: "BundleItems",
                sql: "(CASE WHEN \"ChildProductItemId\" IS NOT NULL THEN 1 ELSE 0 END +  CASE WHEN \"ChildBundleId\" IS NOT NULL THEN 1 ELSE 0 END) = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleItems_Bundles_ChildBundleId",
                table: "BundleItems",
                column: "ChildBundleId",
                principalTable: "Bundles",
                principalColumn: "BundleId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
