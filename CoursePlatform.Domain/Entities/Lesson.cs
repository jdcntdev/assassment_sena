using System;
using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class Lesson : BaseEntity
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    
    // Relationship
    public virtual Course? Course { get; set; }
}
