using System;
using System.Collections.Generic;

namespace PartTimeJobManagement.API.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int ContractId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Contract Contract { get; set; } = null!;
}
