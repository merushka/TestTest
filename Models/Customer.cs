using LinqToDB.Mapping;

[Table(Name = "CUSTOMERS")]
public class Customer
{
    [PrimaryKey, Identity]
    [Column(Name = "ID")] // ДОЛЖНО совпадать с БД
    public int Id { get; set; }

    [Column(Name = "NAME"), NotNull]
    public string Name { get; set; } = string.Empty;

    [Column(Name = "EMAIL"), NotNull]
    public string Email { get; set; } = string.Empty;
}
