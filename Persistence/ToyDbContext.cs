using Microsoft.EntityFrameworkCore;
using entity_toy.Entities;

namespace entity_toy.Persistence;

public class ToyDbContext : DbContext
{
    public ToyDbContext(DbContextOptions<ToyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<TaskItem> Tasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API configurations
        
        // 1. User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique(); // Email must be unique
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        // 2. Project Configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(30); // Configuración del nuevo atributo

            // One-to-Many Relationship (User -> Projects)
            entity.HasOne(p => p.User)
                  .WithMany(u => u.Projects)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade); // Cascade delete projects if user is deleted
        });

        // 3. TaskItem Configuration
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(150).IsRequired();
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);

            // One-to-Many Relationship (Project -> Tasks)
            entity.HasOne(t => t.Project)
                  .WithMany(p => p.Tasks)
                  .HasForeignKey(t => t.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade); // Cascade delete tasks if project is deleted
        });
    }
}
