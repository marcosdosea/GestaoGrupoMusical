using System;
using System.Collections.Generic;

namespace Core;

public partial class Aspnetuser
{
    public string Id { get; set; } = null!;

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public ulong EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public ulong PhoneNumberConfirmed { get; set; }

    public ulong TwoFactorEnabled { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public ulong LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public virtual ICollection<Aspnetuserclaim> Aspnetuserclaims { get; set; } = new List<Aspnetuserclaim>();

    public virtual ICollection<Aspnetuserlogin> Aspnetuserlogins { get; set; } = new List<Aspnetuserlogin>();

    public virtual ICollection<Aspnetusertoken> Aspnetusertokens { get; set; } = new List<Aspnetusertoken>();

    public virtual ICollection<Aspnetrole> Roles { get; set; } = new List<Aspnetrole>();
}
