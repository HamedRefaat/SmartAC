using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theoremone.SmartAc.Application.Common.Interfaces
{
    public interface IDateTime
    {
        DateTimeOffset Now { get; }
    }
}
