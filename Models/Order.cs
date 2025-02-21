using LinqToDB.Mapping;

[Table("ORDERS")]
public class Order
{
    [PrimaryKey, Identity]
    public int Id { get; set; } 

    [Column, NotNull]
    public int CustomerId { get; set; }

    [Column, NotNull]
    public DateTime OrderDate { get; set; } 

    [Association(ThisKey = "CustomerId", OtherKey = "Id")]
    public Customer Customer { get; set; } = null!;
}
