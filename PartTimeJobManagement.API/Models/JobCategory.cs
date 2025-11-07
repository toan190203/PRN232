using System;
using System.Collections.Generic;

namespace PartTimeJobManagement.API.Models;

public partial class JobCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
