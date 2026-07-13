using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using entity_toy.Persistence;
using entity_toy.Entities;

namespace Toy.Seeder;

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
            Console.Error.WriteLine("[Toy-Seeder] ERROR: No connection string provided. Use --connection or set TOY_DB_CONNECTION_STRING.");
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
            // 3. Build DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<ToyDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var context = new ToyDbContext(optionsBuilder.Options);

            // Ensure database is accessible and has migrations applied (optional sanity check)
            if (!context.Database.CanConnect())
            {
                Console.Error.WriteLine("[Toy-Seeder] ERROR: Cannot connect to the target database.");
                return 3;
            }

            // 4. Seeding Logic (Idempotent: Check before insert)
            
            // A. Seed default user if not exists (User with Id=1 is already seeded by EF HasData,
            // but let's seed a dynamic test user with Id=2 for testing).
            var testUser = context.Users.FirstOrDefault(u => u.Username == "testuser");
            if (testUser == null)
            {
                Console.WriteLine("[Toy-Seeder] Seeding test user...");
                testUser = new User
                {
                    Username = "testuser",
                    Email = "testuser@example.com",
                    PhoneNumber = "999888777"
                };
                context.Users.Add(testUser);
                context.SaveChanges(); // Save to get the generated Id if it's identity
            }

            // B. Seed default project if not exists
            var testProject = context.Projects.FirstOrDefault(p => p.Name == "Project Alpha");
            if (testProject == null)
            {
                Console.WriteLine("[Toy-Seeder] Seeding default project Alpha...");
                testProject = new Project
                {
                    Name = "Project Alpha",
                    Description = "Seeded by Toy.Seeder console application.",
                    Status = "Active",
                    User = testUser // Association
                };
                context.Projects.Add(testProject);
                context.SaveChanges();
            }

            // C. Seed default tasks if not exists
            if (!context.Tasks.Any(t => t.Project.Id == testProject.Id))
            {
                Console.WriteLine("[Toy-Seeder] Seeding default tasks for Project Alpha...");
                context.Tasks.Add(new TaskItem
                {
                    Title = "Task seeded automatically by C# Seeder",
                    IsCompleted = false,
                    DueDate = DateTime.Now.AddDays(7),
                    Project = testProject
                });
                context.SaveChanges();
            }

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
