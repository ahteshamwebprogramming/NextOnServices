using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.ViewModels.Pages;

public class Message
{
    public string? MessageHeading { get; set; }
    public string? MessageInfo { get; set; }
    public string? ButtonText { get; set; }
    public string? Redirect { get; set; }
}
