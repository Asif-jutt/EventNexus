using System;
using System.Collections.Generic;

namespace EventNexus.Models;

public partial class Register
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;
}
