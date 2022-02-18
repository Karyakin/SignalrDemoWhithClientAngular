using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SignalrDemo.EFModels
{
    public class SignalrContext : DbContext, IDbContext
    {
        public SignalrContext(DbContextOptions<SignalrContext> options) : base(options)
        {
            Console.WriteLine("Hello from the SignalR!");
        }

        public DbSet<Person> Persons{ get; set; }
        public DbSet<Connections> Connections{ get; set; }
    }

    public interface IDbContext 
    {
        DbSet<Person> Persons { get; set; }
        DbSet<Connections> Connections { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}
