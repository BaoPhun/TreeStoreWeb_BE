using Microsoft.EntityFrameworkCore;
using TreeStore.Models.Entities;

public class TreeStoreContext : DbContext
{
    public TreeStoreContext(DbContextOptions<TreeStoreContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
}
