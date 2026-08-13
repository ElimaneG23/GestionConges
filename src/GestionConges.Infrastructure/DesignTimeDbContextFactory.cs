using GestionConges.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GestionConges.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=GestionCongesDb;Username=postgres;Password=beglebron236;",
            b => b.MigrationsAssembly("GestionConges.Infrastructure")
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}