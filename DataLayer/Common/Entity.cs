namespace DataLayer.Common;

public class Entity<TKey>
{
    public TKey Id { get; set; } = default!;
}

public class Entity : Entity<int>
{

}
