using System;
using System.Collections.Generic;

namespace PartTimeJobManagement.API.Models;

public partial class Employer
{
    public int EmployerId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? ContactName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? TaxCode { get; set; }

    public bool IsVerified { get; set; }

    public virtual User EmployerNavigation { get; set; } = null!;

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
