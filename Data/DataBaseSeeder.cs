using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using WebApplicationTest.Data;
using WebApplicationTest.Models;

public static class DatabaseSeeder
{
    public static void SeedAll(DatabaseContext db)
    {
        Console.WriteLine("=== Starting Seeding ===");

        ClearDatabase(db);

        SeedCustomers(db, 100);
        SeedProducts(db, 1_000_000);
        SeedOrders(db, 100_000);
        SeedTwoBigOrders(db);

        Console.WriteLine("=== Seeding Complete ===");
    }

    private static void ClearDatabase(DatabaseContext db)
    {
        Console.WriteLine("Clearing tables...");

        db.Execute("DELETE FROM ORDERITEMS");
        db.Execute("DELETE FROM ORDERS");
        db.Execute("DELETE FROM PRODUCTS");
        db.Execute("DELETE FROM CUSTOMERS");

        try
        {
            db.Execute("ALTER TABLE CUSTOMERS MODIFY ID GENERATED ALWAYS AS IDENTITY (START WITH 1)");
            db.Execute("ALTER TABLE PRODUCTS MODIFY ID GENERATED ALWAYS AS IDENTITY (START WITH 1)");
            db.Execute("ALTER TABLE ORDERS MODIFY ID GENERATED ALWAYS AS IDENTITY (START WITH 1)");
        }
        catch
        {
            Console.WriteLine("Skipping IDENTITY reset (not supported).");
        }

        Console.WriteLine("Tables cleared.");
    }

    private static void SeedCustomers(DatabaseContext db, int count)
    {
        if (db == null)
        {
            throw new ArgumentNullException(nameof(db), "❌ Ошибка: Database context is null!");
        }

        Console.WriteLine($"Seeding {count} customers...");
        var rand = new Random();

        using var transaction = db.BeginTransaction();

        for (int i = 1; i <= count; i++)
        {
            string email = $"user{i}@example.com";

            if (!db.Customers.Any(c => c.Email == email))
            {
                var customer = new Customer
                {
                    Name = $"User{i}",
                    Email = email
                };

                try
                {
                    // ✅ Добавлено детальное логирование перед вставкой
                    Console.WriteLine($"? Вставляем Customer: {customer.Name}, {customer.Email}");

                    var insertedId = db.InsertWithInt32Identity(customer);
                    if (insertedId <= 0)
                    {
                        throw new NullReferenceException("❌ Ошибка: InsertWithInt32Identity вернул некорректный ID!");
                    }

                    customer.Id = insertedId;

                    // ✅ Добавлено логирование ID после успешной вставки
                    Console.WriteLine($"✅ Вставлен Customer ID: {customer.Id}");

                    if (i % 50 == 0)
                    {
                        Console.WriteLine($"Inserted {i} customers...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Ошибка при вставке: {ex.Message}");
                }
            }
        }

        transaction.Commit();
        Console.WriteLine("Customers inserted.");
    }


    private static void SeedProducts(DatabaseContext db, int totalCount)
    {
        Console.WriteLine($"Seeding {totalCount} products...");
        var rand = new Random();

        for (int i = 1; i <= totalCount; i++)
        {
            var product = new Product
            {
                Name = $"Product_{i}_{Guid.NewGuid():N}",
                Price = rand.Next(10, 2000),
                Quantity = rand.Next(1, 500)
            };

            product.Id = db.InsertWithInt32Identity(product);

            if (i % 50_000 == 0)
            {
                Console.WriteLine($"Inserted {i} products...");
            }
        }

        Console.WriteLine("Products inserted.");

    }

    private static void SeedOrders(DatabaseContext db, int totalOrders)
    {
        if (db == null)
            throw new ArgumentNullException(nameof(db), "❌ Ошибка: Database context is null!");

        Console.WriteLine($"Seeding {totalOrders} orders...");
        var rand = new Random();

        var customerIds = db.Customers.Select(c => c.Id).ToList();
        var productIds = db.Products.Select(p => p.Id).ToList();

        if (customerIds.Count == 0 || productIds.Count == 0)
            throw new InvalidOperationException("❌ Ошибка: Нет данных в Customers или Products!");

        using var transaction = db.BeginTransaction();

        try
        {
            for (int i = 1; i <= totalOrders; i++)
            {
                var order = new Order
                {
                    CustomerId = customerIds[rand.Next(customerIds.Count)],
                    OrderDate = DateTime.Now.AddDays(-rand.Next(365))
                };

                var insertedId = db.InsertWithInt32Identity(order);
                if (insertedId <= 0)
                {
                    throw new NullReferenceException("❌ Ошибка: InsertWithInt32Identity вернул некорректный ID!");
                }

                order.Id = insertedId;
                Console.WriteLine($"✅ Вставлен Order ID: {order.Id}");

                if (i % 50_000 == 0)
                {
                    Console.WriteLine($"Inserted {i} orders...");
                }
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"❌ Ошибка при вставке заказов: {ex.Message}");
        }
    }

    private static void SeedTwoBigOrders(DatabaseContext db)
    {
        if (db == null)
            throw new ArgumentNullException(nameof(db), "❌ Ошибка: Database context is null!");

        Console.WriteLine("Seeding 2 big orders...");

        var firstCustId = db.Customers.Min(c => c.Id);
        if (firstCustId == 0)
            throw new InvalidOperationException("❌ Ошибка: В базе нет ни одного Customer!");

        using var transaction = db.BeginTransaction();

        try
        {
            var bigOrder1 = new Order { CustomerId = firstCustId, OrderDate = DateTime.Now };
            var bigOrder2 = new Order { CustomerId = firstCustId, OrderDate = DateTime.Now };

            // ✅ Вставляем заказы и сразу получаем ID
            bigOrder1.Id = db.InsertWithInt32Identity(bigOrder1);
            bigOrder2.Id = db.InsertWithInt32Identity(bigOrder2);

            // ✅ Проверяем, получены ли корректные ID
            if (bigOrder1.Id == 0 || bigOrder2.Id == 0)
            {
                // Если вдруг ID не записались, получаем MAX(ID)
                bigOrder1.Id = db.Orders.Max(o => o.Id);
                bigOrder2.Id = db.Orders.Max(o => o.Id);
            }

            if (bigOrder1.Id == 0 || bigOrder2.Id == 0)
            {
                throw new Exception("❌ Ошибка: Вставленные ID некорректны!");
            }

            Console.WriteLine($"✅ Вставлен Big Order ID: {bigOrder1.Id}");
            Console.WriteLine($"✅ Вставлен Big Order ID: {bigOrder2.Id}");

            transaction.Commit();

            // ✅ Вставляем `OrderItems`
            var productIds = db.Products.Select(p => p.Id).Take(2000).ToList();

            for (int i = 0; i < 1000; i++)
            {
                db.Insert(new OrderItem { OrderId = bigOrder1.Id, ProductId = productIds[i], Quantity = 1, Price = 123 });
                db.Insert(new OrderItem { OrderId = bigOrder2.Id, ProductId = productIds[i + 1000], Quantity = 1, Price = 456 });
            }

            Console.WriteLine("✅ 2 big orders inserted.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"❌ Ошибка при вставке заказов: {ex.Message}");
        }
    }


}