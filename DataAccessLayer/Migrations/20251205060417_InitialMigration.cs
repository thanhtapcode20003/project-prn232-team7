using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "campus",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "service_location",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    campus_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    date_update = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_location", x => x.id);
                    table.ForeignKey(
                        name: "FK_service_location_campus",
                        column: x => x.campus_id,
                        principalTable: "campus",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    mssv = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    campus_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    user_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_campus",
                        column: x => x.campus_id,
                        principalTable: "campus",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_role",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "image_storage",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    entity_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    entity_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    image_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_order = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    file_size = table.Column<long>(type: "bigint", nullable: true),
                    mime_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    uploaded_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image_storage", x => x.id);
                    table.ForeignKey(
                        name: "FK_image_storage_user",
                        column: x => x.uploaded_by,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "item",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    found_location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    found_campus_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    found_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    current_location_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    category_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_category",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_item_found_campus",
                        column: x => x.found_campus_id,
                        principalTable: "campus",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_item_location",
                        column: x => x.current_location_id,
                        principalTable: "service_location",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_item_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    reference_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_revoked = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    old_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    new_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    changed_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_history_item",
                        column: x => x.item_id,
                        principalTable: "item",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_item_history_user",
                        column: x => x.changed_by,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "receipt",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "PENDING"),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    date_update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    staff_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    verify_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_verified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receipt", x => x.id);
                    table.ForeignKey(
                        name: "FK_receipt_item",
                        column: x => x.item_id,
                        principalTable: "item",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_receipt_staff",
                        column: x => x.staff_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_receipt_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "report",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    action_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "OPEN"),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_report_item",
                        column: x => x.item_id,
                        principalTable: "item",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_report_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "return_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    staff_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    img_cccd_front = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    img_cccd_back = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    evidence_img = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    confirm_img = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    verify_notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "COMPLETED"),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    date_update = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_return_record", x => x.id);
                    table.ForeignKey(
                        name: "FK_return_record_item",
                        column: x => x.item_id,
                        principalTable: "item",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_return_record_staff",
                        column: x => x.staff_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_return_record_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "UQ_campus_name",
                table: "campus",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_categories_name",
                table: "categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_image_storage_entity",
                table: "image_storage",
                columns: new[] { "entity_type", "entity_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_image_storage_uploaded_by",
                table: "image_storage",
                column: "uploaded_by");

            migrationBuilder.CreateIndex(
                name: "IX_item_current_location_id",
                table: "item",
                column: "current_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_found_campus_id",
                table: "item",
                column: "found_campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_search",
                table: "item",
                columns: new[] { "category_id", "found_campus_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_status",
                table: "item",
                columns: new[] { "status", "date_created" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_item_user_id",
                table: "item",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_history_changed_by",
                table: "item_history",
                column: "changed_by");

            migrationBuilder.CreateIndex(
                name: "IX_item_history_item_id",
                table: "item_history",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user",
                table: "notifications",
                columns: new[] { "user_id", "is_read", "date_created" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_receipt_item_id",
                table: "receipt",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_staff_id",
                table: "receipt",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_user_id",
                table: "receipt",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_item_id",
                table: "report",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_user_id",
                table: "report",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_return_record_item_id",
                table: "return_record",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_return_record_staff_id",
                table: "return_record",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_return_record_user_id",
                table: "return_record",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_location_campus_id",
                table: "service_location",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_campus",
                table: "user",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_role",
                table: "user",
                columns: new[] { "role_id", "status" });

            migrationBuilder.CreateIndex(
                name: "UQ_user_gmail",
                table: "user",
                column: "gmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_user_mssv",
                table: "user",
                column: "mssv",
                unique: true,
                filter: "[mssv] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_user_username",
                table: "user",
                column: "user_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "image_storage");

            migrationBuilder.DropTable(
                name: "item_history");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "receipt");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "report");

            migrationBuilder.DropTable(
                name: "return_record");

            migrationBuilder.DropTable(
                name: "item");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "service_location");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "campus");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
