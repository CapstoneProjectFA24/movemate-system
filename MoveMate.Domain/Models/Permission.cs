using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Permission
{
    public int Id { get; set; }

    public string? Src { get; set; }

    public string? TypePermission { get; set; }

    public virtual ICollection<GroupRolePermission> GroupRolePermissions { get; set; } = new List<GroupRolePermission>();
}
