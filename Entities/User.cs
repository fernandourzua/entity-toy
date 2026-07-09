using System.Collections.Generic;

namespace entity_toy.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;

    // Navigation property: One User can have many Projects
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
