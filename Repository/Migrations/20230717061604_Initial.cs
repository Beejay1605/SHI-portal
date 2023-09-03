using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_credentials",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "char(36)", nullable: false),
                    USERNAME = table.Column<string>(type: "longtext", nullable: false),
                    PASSWORD = table.Column<string>(type: "longtext", nullable: false),
                    EMAIL = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ACCESS_LEVEL = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    UPDATED_AT_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DELETE_AT_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    STATUS = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    REMARKS = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    TOTAL_ATTEMPTS = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_credentials", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "operations_details",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CRED_REF = table.Column<Guid>(type: "char(36)", nullable: false),
                    FIRST_NAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    LAST_NAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    MIDDLE_NAME = table.Column<string>(type: "longtext", nullable: false),
                    POSITION = table.Column<string>(type: "longtext", nullable: false),
                    BIRTH_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ADDRESS = table.Column<string>(type: "varchar(220)", maxLength: 220, nullable: false),
                    MOBILE_NUMBER = table.Column<int>(type: "int", nullable: false),
                    MOBILE_COUNTRY_CODE = table.Column<string>(type: "longtext", nullable: false),
                    UPDATED_AT_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NATIONALITY = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operations_details", x => x.ID);
                    table.ForeignKey(
                        name: "FK_operations_details_user_credentials_CRED_REF",
                        column: x => x.CRED_REF,
                        principalTable: "user_credentials",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_tokens",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    USER_REF = table.Column<Guid>(type: "char(36)", nullable: false),
                    TOKEN = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    EXPIRATION_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IS_USED = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tokens", x => x.ID);
                    table.ForeignKey(
                        name: "FK_user_tokens_user_credentials_USER_REF",
                        column: x => x.USER_REF,
                        principalTable: "user_credentials",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "distributors_details",
                columns: table => new
                {
                    DISTRIBUTOR_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    USER_CRED_REF = table.Column<Guid>(type: "char(36)", nullable: false),
                    FIRSTNAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    LASTNAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    MIDDLENAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    SUFFIX = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    ADDRESS = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    CONTACT_NUMBER = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    BIRTH_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MESSENGER_ACCOUNT = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    TIN_NUMBER = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    ACCOUNT_TYPE = table.Column<string>(type: "longtext", nullable: false),
                    NUMBER_OF_ACCOUNTS = table.Column<int>(type: "int", nullable: false),
                    GENDER = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    UPLINE_REF_ID = table.Column<int>(type: "int", nullable: true),
                    PICTURE_PATH = table.Column<string>(type: "longtext", nullable: true),
                    CREATED_BY = table.Column<int>(type: "int", nullable: false),
                    UPDATED_BY = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_distributors_details", x => x.DISTRIBUTOR_ID);
                    table.ForeignKey(
                        name: "FK_distributors_details_distributors_details_UPLINE_REF_ID",
                        column: x => x.UPLINE_REF_ID,
                        principalTable: "distributors_details",
                        principalColumn: "DISTRIBUTOR_ID");
                    table.ForeignKey(
                        name: "FK_distributors_details_operations_details_CREATED_BY",
                        column: x => x.CREATED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_distributors_details_operations_details_UPDATED_BY",
                        column: x => x.UPDATED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_distributors_details_user_credentials_USER_CRED_REF",
                        column: x => x.USER_CRED_REF,
                        principalTable: "user_credentials",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PRODUCT_CODE = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    PRODUCT_NAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    SRP_PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MEMBERS_PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NON_MEMBERS_DISCOUNTED_PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    COMPANY_PROFIT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PAYOUT_TOTAL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CATEGORY = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    COVER_PHOTO_PATH = table.Column<string>(type: "longtext", nullable: true),
                    DESCCRIPTION_FILE_PATH = table.Column<string>(type: "longtext", nullable: true),
                    MINI_DESCRIPTION = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UPDATED_AT_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DELETED_AT_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    STATUS = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    REMARKS = table.Column<string>(type: "longtext", nullable: true),
                    IS_PACKAGE = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CREATED_BY = table.Column<int>(type: "int", nullable: false),
                    UPDATED_BY = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.ID);
                    table.ForeignKey(
                        name: "FK_products_operations_details_CREATED_BY",
                        column: x => x.CREATED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_operations_details_UPDATED_BY",
                        column: x => x.UPDATED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    TRANSACTION_NUMBER = table.Column<string>(type: "longtext", nullable: false),
                    TRANSACTION_TYPE = table.Column<string>(type: "longtext", nullable: false),
                    VOID_STATUS = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CREATED_DATE_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UPDATED_DATE_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CREATED_BY = table.Column<int>(type: "int", nullable: false),
                    UPDATED_BY = table.Column<int>(type: "int", nullable: true),
                    IS_CODE_GENERATED = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IS_ENCODED_UNILEVEL = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_transactions_operations_details_CREATED_BY",
                        column: x => x.CREATED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_operations_details_UPDATED_BY",
                        column: x => x.UPDATED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payin_codes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PAYIN_CODE = table.Column<string>(type: "longtext", nullable: false),
                    DISTRIBUTOR_REF = table.Column<int>(type: "int", nullable: false),
                    TRANSACTION_REF = table.Column<int>(type: "int", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EXPIRATION_DT = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CREATED_BY = table.Column<int>(type: "int", nullable: false),
                    UPDATED_BY = table.Column<int>(type: "int", nullable: false),
                    IS_USED = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payin_codes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_payin_codes_distributors_details_DISTRIBUTOR_REF",
                        column: x => x.DISTRIBUTOR_REF,
                        principalTable: "distributors_details",
                        principalColumn: "DISTRIBUTOR_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payin_codes_operations_details_CREATED_BY",
                        column: x => x.CREATED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payin_codes_operations_details_UPDATED_BY",
                        column: x => x.UPDATED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inventory",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PRODUCT_REF = table.Column<int>(type: "int", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    ACTION = table.Column<string>(type: "longtext", nullable: false),
                    VOID_STATUS = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DOC_PATH = table.Column<string>(type: "longtext", nullable: false),
                    CREATE_DATE_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TRANSACTION_REF = table.Column<int>(type: "int", nullable: true),
                    CREATED_BY = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_inventory_operations_details_CREATED_BY",
                        column: x => x.CREATED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inventory_products_PRODUCT_REF",
                        column: x => x.PRODUCT_REF,
                        principalTable: "products",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "package_products",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PACKAGE_ID = table.Column<int>(type: "int", nullable: false),
                    PRODUCT_REF = table.Column<int>(type: "int", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    CREATE_UPDATE_DT = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_package_products", x => x.ID);
                    table.ForeignKey(
                        name: "FK_package_products_products_PACKAGE_ID",
                        column: x => x.PACKAGE_ID,
                        principalTable: "products",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_package_products_products_PRODUCT_REF",
                        column: x => x.PRODUCT_REF,
                        principalTable: "products",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "product_images",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PHOTO_PATH = table.Column<string>(type: "longtext", nullable: false),
                    IMG_INDEX = table.Column<int>(type: "int", nullable: false),
                    PRODUCT_REF = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_images", x => x.ID);
                    table.ForeignKey(
                        name: "FK_product_images_products_PRODUCT_REF",
                        column: x => x.PRODUCT_REF,
                        principalTable: "products",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "void_inventory",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PRODUCT_REF = table.Column<int>(type: "int", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    ACTION = table.Column<string>(type: "longtext", nullable: false),
                    VOID_STATUS = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DOC_PATH = table.Column<string>(type: "longtext", nullable: false),
                    CREATE_DATE_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TRANSACTION_REF = table.Column<int>(type: "int", nullable: true),
                    CREATED_BY = table.Column<int>(type: "int", nullable: false),
                    VOIDED_BY = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_void_inventory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_void_inventory_operations_details_CREATED_BY",
                        column: x => x.CREATED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_void_inventory_operations_details_VOIDED_BY",
                        column: x => x.VOIDED_BY,
                        principalTable: "operations_details",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_void_inventory_products_PRODUCT_REF",
                        column: x => x.PRODUCT_REF,
                        principalTable: "products",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pos_transactions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    TRANSACTION_REF = table.Column<int>(type: "int", nullable: false),
                    PRODUCT_REF = table.Column<int>(type: "int", nullable: false),
                    SRP_PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MEMBERS_PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NON_MEMBERS_DISCOUNTED_PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    COMPANY_PROFIT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PAYOUT_TOTAL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PER_UNIT_PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    PAYMENT_TYPE = table.Column<string>(type: "longtext", nullable: false),
                    BRANCH = table.Column<string>(type: "longtext", nullable: false),
                    DISTRIBUTOR_REF = table.Column<int>(type: "int", nullable: true),
                    VAT_PERCENTAGE = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pos_transactions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_pos_transactions_products_PRODUCT_REF",
                        column: x => x.PRODUCT_REF,
                        principalTable: "products",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pos_transactions_transactions_TRANSACTION_REF",
                        column: x => x.TRANSACTION_REF,
                        principalTable: "transactions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "binary_tree",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    DISTRIBUTOR_REF = table.Column<int>(type: "int", nullable: false),
                    UPLINE_DETAILS_REF = table.Column<int>(type: "int", nullable: true),
                    GRAND_UPLINE_DETAILS_REF = table.Column<int>(type: "int", nullable: true),
                    PARENT_BINARY_REF = table.Column<int>(type: "int", nullable: true),
                    CHILD_LEFT_BINARY_REF = table.Column<int>(type: "int", nullable: true),
                    CHILD_RIGHT_BINARY_REF = table.Column<int>(type: "int", nullable: true),
                    POSITION = table.Column<string>(type: "longtext", nullable: true),
                    LEVELS = table.Column<int>(type: "int", nullable: false),
                    CREATED_AT_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UPDATED_AT_UTC = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PAYIN_CODE_REF = table.Column<int>(type: "int", nullable: false),
                    IMAGINARY_UPLINE_BIN_REF = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_binary_tree", x => x.ID);
                    table.ForeignKey(
                        name: "FK_binary_tree_binary_tree_CHILD_LEFT_BINARY_REF",
                        column: x => x.CHILD_LEFT_BINARY_REF,
                        principalTable: "binary_tree",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_binary_tree_binary_tree_CHILD_RIGHT_BINARY_REF",
                        column: x => x.CHILD_RIGHT_BINARY_REF,
                        principalTable: "binary_tree",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_binary_tree_binary_tree_PARENT_BINARY_REF",
                        column: x => x.PARENT_BINARY_REF,
                        principalTable: "binary_tree",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_binary_tree_distributors_details_DISTRIBUTOR_REF",
                        column: x => x.DISTRIBUTOR_REF,
                        principalTable: "distributors_details",
                        principalColumn: "DISTRIBUTOR_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_binary_tree_distributors_details_GRAND_UPLINE_DETAILS_REF",
                        column: x => x.GRAND_UPLINE_DETAILS_REF,
                        principalTable: "distributors_details",
                        principalColumn: "DISTRIBUTOR_ID");
                    table.ForeignKey(
                        name: "FK_binary_tree_distributors_details_UPLINE_DETAILS_REF",
                        column: x => x.UPLINE_DETAILS_REF,
                        principalTable: "distributors_details",
                        principalColumn: "DISTRIBUTOR_ID");
                    table.ForeignKey(
                        name: "FK_binary_tree_payin_codes_PAYIN_CODE_REF",
                        column: x => x.PAYIN_CODE_REF,
                        principalTable: "payin_codes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "earnings_pairing",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    LEFT_BIN_ID = table.Column<int>(type: "int", nullable: true),
                    RIGHT_BIN_ID = table.Column<int>(type: "int", nullable: true),
                    BENEF_BIN_ID = table.Column<int>(type: "int", nullable: false),
                    BENEF_DIST_ID = table.Column<int>(type: "int", nullable: false),
                    AMOUNT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IS_ENCASH = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LEVEL = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_earnings_pairing", x => x.ID);
                    table.ForeignKey(
                        name: "FK_earnings_pairing_binary_tree_BENEF_BIN_ID",
                        column: x => x.BENEF_BIN_ID,
                        principalTable: "binary_tree",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_earnings_pairing_distributors_details_BENEF_DIST_ID",
                        column: x => x.BENEF_DIST_ID,
                        principalTable: "distributors_details",
                        principalColumn: "DISTRIBUTOR_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "earnings_referal",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    BENEF_BINARY_REF = table.Column<int>(type: "int", nullable: false),
                    FROM_BINARY_REF = table.Column<int>(type: "int", nullable: false),
                    BENEF_DISTRIBUTOR_REF = table.Column<int>(type: "int", nullable: false),
                    BONUS_TYPE = table.Column<string>(type: "longtext", nullable: false),
                    CREATED_DT = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IS_ENCASH = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ENCASH_REQUEST_BY = table.Column<int>(type: "int", nullable: true),
                    AMOUNT = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_earnings_referal", x => x.ID);
                    table.ForeignKey(
                        name: "FK_earnings_referal_binary_tree_BENEF_BINARY_REF",
                        column: x => x.BENEF_BINARY_REF,
                        principalTable: "binary_tree",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_earnings_referal_binary_tree_FROM_BINARY_REF",
                        column: x => x.FROM_BINARY_REF,
                        principalTable: "binary_tree",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_earnings_referal_distributors_details_BENEF_DISTRIBUTOR_REF",
                        column: x => x.BENEF_DISTRIBUTOR_REF,
                        principalTable: "distributors_details",
                        principalColumn: "DISTRIBUTOR_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "earnings_uni_level",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    BINARY_REF = table.Column<int>(type: "int", nullable: false),
                    DISTRIBUTOR_REF = table.Column<int>(type: "int", nullable: false),
                    TRANSACTION_REF = table.Column<int>(type: "int", nullable: false),
                    AMOUNT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AVAILABILITY_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IS_ENCASH = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    REQUEST_BY = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_earnings_uni_level", x => x.ID);
                    table.ForeignKey(
                        name: "FK_earnings_uni_level_binary_tree_BINARY_REF",
                        column: x => x.BINARY_REF,
                        principalTable: "binary_tree",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_earnings_uni_level_distributors_details_DISTRIBUTOR_REF",
                        column: x => x.DISTRIBUTOR_REF,
                        principalTable: "distributors_details",
                        principalColumn: "DISTRIBUTOR_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_earnings_uni_level_transactions_TRANSACTION_REF",
                        column: x => x.TRANSACTION_REF,
                        principalTable: "transactions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.Sql("ALTER TABLE binary_tree AUTO_INCREMENT = 100;");
            migrationBuilder.Sql("ALTER TABLE distributors_details AUTO_INCREMENT = 1000000;");
            migrationBuilder.Sql("ALTER TABLE operations_details AUTO_INCREMENT = 1000000;");
            migrationBuilder.Sql("ALTER TABLE package_products AUTO_INCREMENT = 1000000;");
            migrationBuilder.Sql("ALTER TABLE payin_codes AUTO_INCREMENT = 1000000;");
            migrationBuilder.Sql("ALTER TABLE pos_transactions AUTO_INCREMENT = 1000000;");
            migrationBuilder.Sql("ALTER TABLE products AUTO_INCREMENT = 1000000;");
            migrationBuilder.Sql("ALTER TABLE product_images AUTO_INCREMENT = 1000000;");
            migrationBuilder.Sql("ALTER TABLE transactions AUTO_INCREMENT = 1000000;");
            migrationBuilder.Sql("ALTER TABLE user_tokens AUTO_INCREMENT = 1000000;"); 
            migrationBuilder.Sql("ALTER TABLE inventory AUTO_INCREMENT = 1000000;");
            migrationBuilder.Sql("ALTER TABLE void_inventory AUTO_INCREMENT = 1000000;"); 
            
            
            migrationBuilder.CreateIndex(
                name: "IX_binary_tree_CHILD_LEFT_BINARY_REF",
                table: "binary_tree",
                column: "CHILD_LEFT_BINARY_REF");

            migrationBuilder.CreateIndex(
                name: "IX_binary_tree_CHILD_RIGHT_BINARY_REF",
                table: "binary_tree",
                column: "CHILD_RIGHT_BINARY_REF");

            migrationBuilder.CreateIndex(
                name: "IX_binary_tree_DISTRIBUTOR_REF",
                table: "binary_tree",
                column: "DISTRIBUTOR_REF");

            migrationBuilder.CreateIndex(
                name: "IX_binary_tree_GRAND_UPLINE_DETAILS_REF",
                table: "binary_tree",
                column: "GRAND_UPLINE_DETAILS_REF");

            migrationBuilder.CreateIndex(
                name: "IX_binary_tree_PARENT_BINARY_REF",
                table: "binary_tree",
                column: "PARENT_BINARY_REF");

            migrationBuilder.CreateIndex(
                name: "IX_binary_tree_PAYIN_CODE_REF",
                table: "binary_tree",
                column: "PAYIN_CODE_REF");

            migrationBuilder.CreateIndex(
                name: "IX_binary_tree_UPLINE_DETAILS_REF",
                table: "binary_tree",
                column: "UPLINE_DETAILS_REF");

            migrationBuilder.CreateIndex(
                name: "IX_distributors_details_CREATED_BY",
                table: "distributors_details",
                column: "CREATED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_distributors_details_UPDATED_BY",
                table: "distributors_details",
                column: "UPDATED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_distributors_details_UPLINE_REF_ID",
                table: "distributors_details",
                column: "UPLINE_REF_ID");

            migrationBuilder.CreateIndex(
                name: "IX_distributors_details_USER_CRED_REF",
                table: "distributors_details",
                column: "USER_CRED_REF");

            migrationBuilder.CreateIndex(
                name: "IX_earnings_pairing_BENEF_BIN_ID",
                table: "earnings_pairing",
                column: "BENEF_BIN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_earnings_pairing_BENEF_DIST_ID",
                table: "earnings_pairing",
                column: "BENEF_DIST_ID");

            migrationBuilder.CreateIndex(
                name: "IX_earnings_referal_BENEF_BINARY_REF",
                table: "earnings_referal",
                column: "BENEF_BINARY_REF");

            migrationBuilder.CreateIndex(
                name: "IX_earnings_referal_BENEF_DISTRIBUTOR_REF",
                table: "earnings_referal",
                column: "BENEF_DISTRIBUTOR_REF");

            migrationBuilder.CreateIndex(
                name: "IX_earnings_referal_FROM_BINARY_REF",
                table: "earnings_referal",
                column: "FROM_BINARY_REF");

            migrationBuilder.CreateIndex(
                name: "IX_earnings_uni_level_BINARY_REF",
                table: "earnings_uni_level",
                column: "BINARY_REF");

            migrationBuilder.CreateIndex(
                name: "IX_earnings_uni_level_DISTRIBUTOR_REF",
                table: "earnings_uni_level",
                column: "DISTRIBUTOR_REF");

            migrationBuilder.CreateIndex(
                name: "IX_earnings_uni_level_TRANSACTION_REF",
                table: "earnings_uni_level",
                column: "TRANSACTION_REF");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_CREATED_BY",
                table: "inventory",
                column: "CREATED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_PRODUCT_REF",
                table: "inventory",
                column: "PRODUCT_REF");

            migrationBuilder.CreateIndex(
                name: "IX_operations_details_CRED_REF",
                table: "operations_details",
                column: "CRED_REF");

            migrationBuilder.CreateIndex(
                name: "IX_package_products_PACKAGE_ID",
                table: "package_products",
                column: "PACKAGE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_package_products_PRODUCT_REF",
                table: "package_products",
                column: "PRODUCT_REF");

            migrationBuilder.CreateIndex(
                name: "IX_payin_codes_CREATED_BY",
                table: "payin_codes",
                column: "CREATED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_payin_codes_DISTRIBUTOR_REF",
                table: "payin_codes",
                column: "DISTRIBUTOR_REF");

            migrationBuilder.CreateIndex(
                name: "IX_payin_codes_UPDATED_BY",
                table: "payin_codes",
                column: "UPDATED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_pos_transactions_PRODUCT_REF",
                table: "pos_transactions",
                column: "PRODUCT_REF");

            migrationBuilder.CreateIndex(
                name: "IX_pos_transactions_TRANSACTION_REF",
                table: "pos_transactions",
                column: "TRANSACTION_REF");

            migrationBuilder.CreateIndex(
                name: "IX_product_images_PRODUCT_REF",
                table: "product_images",
                column: "PRODUCT_REF");

            migrationBuilder.CreateIndex(
                name: "IX_products_CREATED_BY",
                table: "products",
                column: "CREATED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_products_UPDATED_BY",
                table: "products",
                column: "UPDATED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_CREATED_BY",
                table: "transactions",
                column: "CREATED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_UPDATED_BY",
                table: "transactions",
                column: "UPDATED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_user_tokens_USER_REF",
                table: "user_tokens",
                column: "USER_REF");

            migrationBuilder.CreateIndex(
                name: "IX_void_inventory_CREATED_BY",
                table: "void_inventory",
                column: "CREATED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_void_inventory_PRODUCT_REF",
                table: "void_inventory",
                column: "PRODUCT_REF");

            migrationBuilder.CreateIndex(
                name: "IX_void_inventory_VOIDED_BY",
                table: "void_inventory",
                column: "VOIDED_BY");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "earnings_pairing");

            migrationBuilder.DropTable(
                name: "earnings_referal");

            migrationBuilder.DropTable(
                name: "earnings_uni_level");

            migrationBuilder.DropTable(
                name: "inventory");

            migrationBuilder.DropTable(
                name: "package_products");

            migrationBuilder.DropTable(
                name: "pos_transactions");

            migrationBuilder.DropTable(
                name: "product_images");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "void_inventory");

            migrationBuilder.DropTable(
                name: "binary_tree");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "payin_codes");

            migrationBuilder.DropTable(
                name: "distributors_details");

            migrationBuilder.DropTable(
                name: "operations_details");

            migrationBuilder.DropTable(
                name: "user_credentials");
        }
    }
}
