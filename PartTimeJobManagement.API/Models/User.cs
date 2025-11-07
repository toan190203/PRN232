using System;
using System.Collections.Generic;

namespace PartTimeJobManagement.API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Employer? Employer { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Student? Student { get; set; }
}
