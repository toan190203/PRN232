using System;
using System.Collections.Generic;

namespace PartTimeJobManagement.API.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string FullName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Major { get; set; }

    public int? YearOfStudy { get; set; }

    public string? Cvfile { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual User StudentNavigation { get; set; } = null!;
}
