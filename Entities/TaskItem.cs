using System;

namespace entity_toy.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; } // Nuevo atributo para simular otra migración

    // Foreign key and navigation property to Project
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
