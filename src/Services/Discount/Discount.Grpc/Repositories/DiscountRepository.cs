using System.Threading.Tasks;
using Dapper;
using Discount.Grpc.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            await using var connection =
                new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var coupon =
                await connection.QueryFirstOrDefaultAsync<Coupon>(
                    @"SELECT * FROM Coupon where ProductName= @productName", new { productName });
            return coupon ?? new Coupon { Amount = 0, Description = "No Discount Desc", ProductName = "No Discount" };
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            await using var connection =
                new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected =
                await connection.ExecuteAsync
                (@"INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                    new { coupon.ProductName, coupon.Description, coupon.Amount });

            return affected != 0;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            await using var connection =
                new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected = await connection.ExecuteAsync
            (@"UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE ID = @Id",
                new { coupon.ProductName, coupon.Description, coupon.Amount, coupon.Id });

            return affected != 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            await using var connection =
                new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected = await connection.ExecuteAsync(@"DELETE FROM Coupon WHERE ProductName = @productName",
                new { productName });

            return affected != 0;
        }
    }
}