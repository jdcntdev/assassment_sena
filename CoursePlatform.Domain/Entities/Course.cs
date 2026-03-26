using System.Collections.Generic;
using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    
    // Relationship
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
