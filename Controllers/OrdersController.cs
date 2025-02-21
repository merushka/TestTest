using System;
using System.Collections.Generic;
using LinqToDB.Mapping;

namespace WebApplication.Models
{
    [Table("ORDERS")]
    public class Order
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column, NotNull]
        public int CustomerId { get; set; }

        [Column, NotNull]
        public DateTime OrderDate { get; set; }

        // Связь с заказчиком
        [Association(ThisKey = "CustomerId", OtherKey = "Id")]
        public Customer Customer { get; set; } = null!;

        // 🔥 **Добавляем связь с OrderItems** 🔥
        [Association(ThisKey = "Id", OtherKey = "OrderId")]
        public List<OrderItem> Items { get; set; } = new(); // Инициализируем список
    }
}
