using System.Collections.Generic;

namespace entity_toy.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Status { get; set; } // Nuevo atributo random para simular cambios

    // Foreign key and navigation property to User
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation property: One Project can have many Tasks
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
