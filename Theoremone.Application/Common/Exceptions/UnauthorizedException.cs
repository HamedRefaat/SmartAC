using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theoremone.SmartAc.Application.Common.Exceptions
{
    public class UnauthorizedException:Exception
    {
        public UnauthorizedException():base(){}
        public UnauthorizedException(string message):base(message){}
        
    }
}
