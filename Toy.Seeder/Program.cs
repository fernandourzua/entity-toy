using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using entity_toy.Persistence;
using entity_toy.Entities;

namespace Toy.Seeder;

// Generic helper extensions to eliminate seeder boilerplate code
public static class EFExtensions
{
    public static T AddOrUpdate<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> predicate, T entity) where T : class
    {
        var existing = dbSet.FirstOrDefault(predicate);
        if (existing == null)
        {
            dbSet.Add(entity);
            return entity;
        }
        return existing;
    }
}

class Program
{
    static int Main(string[] args)
    {
        Console.WriteLine("[Toy-Seeder] Starting C# Seeding Process...");

        // 1. Resolve connection string from arguments or environment variable
        string? connectionString = null;
        for (int i = 0; i < args.Length; i++)
        {
            if ((args[i] == "--connection" || args[i] == "-c") && i + 1 < args.Length)
            {
                connectionString = args[i + 1];
                break;
            }
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("TOY_DB_CONNECTION_STRING");
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            Console.Error.WriteLine("[Toy-Seeder] ERROR: No connection string provided.");
            return 1;
        }

        // 2. Security Safeguard: Prevent seeding in Production
        string? aspnetEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (aspnetEnv == "Production" || connectionString.Contains("prod-sql-server", StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine("[Toy-Seeder] SECURITY BLOCK: Seeder execution is NOT allowed in Production environment!");
            return 2;
        }

        try
        {
            // 3. Initialize Database Context
            var optionsBuilder = new DbContextOptionsBuilder<ToyDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var context = new ToyDbContext(optionsBuilder.Options);

            if (!context.Database.CanConnect())
            {
                Console.Error.WriteLine("[Toy-Seeder] ERROR: Cannot connect to the target database.");
                return 3;
            }

            // 4. Declarative Seeding Logic (Cero Boilerplate)
            
            // A. Seed test user
            var testUser = context.Users.AddOrUpdate(u => u.Username == "testuser", new User
            {
                Username = "testuser",
                Email = "testuser@example.com",
                PhoneNumber = "999888777"
            });

            // B. Seed default project
            var testProject = context.Projects.AddOrUpdate(p => p.Name == "Project Alpha", new Project
            {
                Name = "Project Alpha",
                Description = "Seeded by Toy.Seeder console application.",
                Status = "Active",
                User = testUser // Auto-associates relationship in memory
            });

            // C. Seed default task
            context.Tasks.AddOrUpdate(t => t.Title == "Task seeded automatically by C# Seeder" && t.Project.Id == testProject.Id, new TaskItem
            {
                Title = "Task seeded automatically by C# Seeder",
                IsCompleted = false,
                DueDate = DateTime.Now.AddDays(7),
                Project = testProject
            });

            // C. testy test
            context.Tasks.AddOrUpdate(t => t.Title == "Task seeded automatically by C# Seeder again" && t.Project.Id == testProject.Id, new TaskItem
            {
                Title = "Task seeded automatically by C# Seeder again",
                IsCompleted = false,
                DueDate = DateTime.Now.AddDays(7),
                Project = testProject
            });


            // 5. Commit all insertions to the Database
            context.SaveChanges();

            Console.WriteLine("[Toy-Seeder] C# Seeding completed successfully.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Toy-Seeder] EXCEPTION OCCURRED: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
            return 99;
        }
    }
}
