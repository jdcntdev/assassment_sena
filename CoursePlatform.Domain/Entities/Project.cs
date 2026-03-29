using System.Collections.Generic;
using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;

    // Relationship
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
