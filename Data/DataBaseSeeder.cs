using System;
using System.Collections.Generic;
using WebApplication.Models;
using LinqToDB;
using LinqToDB.Data;

namespace WebApplication.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedAll(DatabaseContext db)
        {
            SeedCustomers(db);
            SeedProducts(db);
            SeedOrders(db);
        }

        private static void SeedCustomers(DatabaseContext db)
        {
            Console.WriteLine("Seeding Customers...");
            db.Execute("TRUNCATE TABLE CUSTOMERS");

            var list = new List<Customer>();
            for (int i = 1; i <= 100; i++)
            {
                list.Add(new Customer
                {
                    Name = $"User{i}",
                    Email = $"user{i}@example.com"
                });
            }
            db.BulkCopy(list);
        }

        private static void SeedProducts(DatabaseContext db)
        {
            Console.WriteLine("Seeding Products...");
            db.Execute("TRUNCATE TABLE PRODUCTS");

            var list = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                list.Add(new Product
                {
                    Name = $"Product{i}",
                    Price = new Random().Next(10, 500),
                    Quantity = new Random().Next(1, 100)
                });
            }
            db.BulkCopy(list);
        }

        private static void SeedOrders(DatabaseContext db)
        {
            Console.WriteLine("Seeding Orders...");
            db.Execute("TRUNCATE TABLE ORDERITEMS");
            db.Execute("TRUNCATE TABLE ORDERS");

            var orders = new List<Order>();
            for (int i = 1; i <= 50; i++)
            {
                orders.Add(new Order
                {
                    CustomerId = new Random().Next(1, 100),
                    OrderDate = DateTime.Now.AddDays(-i)
                });
            }
            db.BulkCopy(orders);
        }
    }
}
