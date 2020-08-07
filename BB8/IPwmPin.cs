using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB8
{
    public interface IPwmPin
    {
        uint PwmRange { get; }
        uint PwmValue { get; set; }
    }
}
