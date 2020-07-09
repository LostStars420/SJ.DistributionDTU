using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.Modbus
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
