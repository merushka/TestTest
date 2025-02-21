using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.Oracle;
using Microsoft.Extensions.Configuration;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class DatabaseContext : DataConnection
    {
        public DatabaseContext(IConfiguration config)
            : base("Oracle.Managed", config.GetConnectionString("OracleDb")!)
        {
        }

        public ITable<Customer> Customers => this.GetTable<Customer>();
        public ITable<Product> Products => this.GetTable<Product>();
        public ITable<Order> Orders => this.GetTable<Order>();
        public ITable<OrderItem> OrderItems => this.GetTable<OrderItem>();
    }
}
