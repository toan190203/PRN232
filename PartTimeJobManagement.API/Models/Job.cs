using System;
using System.Collections.Generic;

namespace PartTimeJobManagement.API.Models;

public partial class Job
{
    public int JobId { get; set; }

    public int EmployerId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal? Salary { get; set; }

    public string? Location { get; set; }

    public int? CategoryId { get; set; }

    public DateTime PostedDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual JobCategory? Category { get; set; }

    public virtual Employer Employer { get; set; } = null!;
}
