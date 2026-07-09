using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace entity_toy.Persistence;

public class ToyDbContextFactory : IDesignTimeDbContextFactory<ToyDbContext>
{
    public ToyDbContext CreateDbContext(string[] args)
    {
        // Environment variable read during migrations compile/generation time
        var connectionString = Environment.GetEnvironmentVariable("TOY_DB_CONNECTION_STRING");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "No se pudo determinar la cadena de conexión. Defina la variable 'TOY_DB_CONNECTION_STRING'.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ToyDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ToyDbContext(optionsBuilder.Options);
    }
}
