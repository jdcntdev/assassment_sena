using System;
using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class ProjectEnrollment : BaseEntity
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }

    public virtual Project? Project { get; set; }
    public virtual ApplicationUser? User { get; set; }
}
