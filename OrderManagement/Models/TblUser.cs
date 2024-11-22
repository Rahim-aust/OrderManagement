using System;
using System.Collections.Generic;

namespace OrderManagement.Models;

public partial class TblUser
{
    public int IntUserId { get; set; }

    public string StrUsername { get; set; } = null!;

    public string StrPasswordHash { get; set; } = null!;

    public int IntFailedLoginAttempts { get; set; }

    public bool IsLocked { get; set; }
}
