using System;
using System.Collections.Generic;

namespace PartTimeJobManagement.API.Models;

public partial class ApplicationHistory
{
    public int HistoryId { get; set; }

    public int ApplicationId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime ChangedAt { get; set; }

    public virtual Application Application { get; set; } = null!;
}
