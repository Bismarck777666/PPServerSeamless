using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITProtocol
{
    #region 메시지코드
    public enum CQ9MessageCode
    {
        InitGame1Request            = 11,
        InitGame2Request            = 12,
        NormalSpinRequest           = 31,
        SpinEndRequest              = 32,
        TembleSpinRequest           = 33,
        FreeSpinStartRequest        = 41,
        FreeSpinRequest             = 42,
        FreeSpinSumRequest          = 43,
        FreeSpinOptionStartRequest  = 44,
        FreeSpinOptionSelectRequest = 45,
        FreeSpinOptionResultRequest = 46,

        InitGame1Response           = 111,
        InitGame2Response           = 112,
        NormalSpinResponse          = 131,
        SpinEndResponse             = 132,
        TembleSpinResponse          = 133,
        FreeSpinStartResponse       = 141,
        FreeSpinResponse            = 142,
        FreeSpinSumResponse         = 143,
        FreeSpinOptionStartResponse = 144,
        FreeSpinOptionSelectResponse = 145,
        FreeSpinOptionResultResponse = 146,
    }
    enum CQ9NextModule
    {
        Normal      = 0,
        FreeStart   = 20,
        FreeOption  = 40,
    }
    #endregion
}
