using Microsoft.AspNetCore.Identity;
using System;

namespace CoursePlatform.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
}
