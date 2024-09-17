using System;
using System.Collections.Generic;

namespace TBSA_Printer.Models;

public partial class Eftest
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Text { get; set; }

    public DateTime? DateTime { get; set; }
}
