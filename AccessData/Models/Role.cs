using System;
using System.Collections.Generic;

namespace AccessData.Models;

public partial class Role
{
    public int Id { get; set; }

    public string RolName { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
