using System;
using System.Collections.Generic;

namespace PartTimeJobManagement.API.Models;

public partial class Application
{
    public int ApplicationId { get; set; }

    public int StudentId { get; set; }

    public int JobId { get; set; }

    public DateTime ApplicationDate { get; set; }

    public string? CoverLetter { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<ApplicationHistory> ApplicationHistories { get; set; } = new List<ApplicationHistory>();

    public virtual Contract? Contract { get; set; }

    public virtual Job Job { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
