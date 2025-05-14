using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastFood.Repository.Migrations
{
    public partial class updatedToEnums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // STEP 1: Update existing string values to match new enum int values
            migrationBuilder.Sql(@"
                UPDATE OrderHeaders SET PaymentStatus =
                    CASE PaymentStatus
                        WHEN 'Pending' THEN '0'
                        WHEN 'Completed' THEN '1'
                        WHEN 'Failed' THEN '2'
                        WHEN 'Refunded' THEN '3'
                        WHEN 'Canceled' THEN '4'
                        ELSE '0'
                    END
            ");

            migrationBuilder.Sql(@"
                UPDATE OrderHeaders SET OrderStatus =
                    CASE OrderStatus
                        WHEN 'Pending' THEN '0'
                        WHEN 'Processing' THEN '1'
                        WHEN 'Shipped' THEN '2'
                        WHEN 'Delivered' THEN '3'
                        WHEN 'Canceled' THEN '4'
                        ELSE '0'
                    END
            ");

            // STEP 2: Alter columns from string to int (to support enums)
            migrationBuilder.AlterColumn<int>(
                name: "PaymentStatus",
                table: "OrderHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderStatus",
                table: "OrderHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // STEP 3: Convert int values back to strings (only if rolling back)
            migrationBuilder.Sql(@"
                UPDATE OrderHeaders SET PaymentStatus =
                    CASE PaymentStatus
                        WHEN 0 THEN 'Pending'
                        WHEN 1 THEN 'Completed'
                        WHEN 2 THEN 'Failed'
                        WHEN 3 THEN 'Refunded'
                        WHEN 4 THEN 'Canceled'
                        ELSE 'Pending'
                    END
            ");

            migrationBuilder.Sql(@"
                UPDATE OrderHeaders SET OrderStatus =
                    CASE OrderStatus
                        WHEN 0 THEN 'Pending'
                        WHEN 1 THEN 'Processing'
                        WHEN 2 THEN 'Shipped'
                        WHEN 3 THEN 'Delivered'
                        WHEN 4 THEN 'Canceled'
                        ELSE 'Pending'
                    END
            ");

            // STEP 4: Revert columns back to string
            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "OrderStatus",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
