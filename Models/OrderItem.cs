using LinqToDB.Mapping;

namespace WebApplication.Models
{
    [Table(Name = "ORDERITEMS")]
    public class OrderItem
    {
        [PrimaryKey, Identity]
        [Column("ID")]
        public int Id { get; set; }

        [Column("ORDERID"), NotNull]
        public int OrderId { get; set; }

        [Column("PRODUCTID"), NotNull]
        public int ProductId { get; set; }

        [Column("QUANTITY"), NotNull]
        public int Quantity { get; set; }

        [Column("PRICE"), NotNull]
        public decimal Price { get; set; }

        [Association(ThisKey = "OrderId", OtherKey = "Id")]
        public Order? Order { get; set; }

        [Association(ThisKey = "ProductId", OtherKey = "Id")]
        public Product? Product { get; set; }
    }
}
