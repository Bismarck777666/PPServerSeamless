using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommNode.HTTPService.Models
{
    public enum WithdrawResults
    {
       OK               = 0,
       INVALIDACCOUNT   = 1,
       NOTENOUGHMONEY   = 2,
       PARAMERROR       = 3,
       OTHERERROR       = 4,
    }
}
