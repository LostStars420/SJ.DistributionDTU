using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportProtocol.Comself
{
    public enum ErrorCodeEnum : byte
    {
        NULL = 0,
        ErrorAddress = 1, 
        ErrorCRCL,         
        ErrorCRCH,
        ErrorLength, 
        Correct,        
    }
}
