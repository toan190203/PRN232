using System;
using System.Collections.Generic;

namespace PartTimeJobManagement.API.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    public int ApplicationId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal SalaryAgreed { get; set; }

    public string? ContractFile { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
