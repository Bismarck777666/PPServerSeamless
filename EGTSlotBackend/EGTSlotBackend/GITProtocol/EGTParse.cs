using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITProtocol
{
    public enum EGTMessageType
    {
        HeartBeat       = 0,
        NormalSpin      = 3,
        Collect         = 4,
        FreeTrigger     = 5,
        FreeSpin        = 6,
        GamblePick      = 7,
        GambleHalf      = 8,
        NoCollectSpin   = 9, 
        ExtendFree      = 10,
        FreeReopen      = 11,
        LastFree        = 12,
        FreeOption      = 16,
        WheelTrigger    = 20, 
        Wheel           = 21,
        LastWheel       = 22,
        RespinTrigger   = 30,
        Respin          = 31,
        LastRespin      = 32,
        FreeRespinStart = 34,
        FreeRespin      = 35,
        FreeRespinEnd   = 36,
        TriggerPower    = 37,
        PowerRespin     = 38,
        LastPower       = 39,
        BonusTrigger    = 45,   
        BonusSpin       = 46,
        BonusEnd        = 47,
        FreeCashStart   = 52,
        FreeCashSpin    = 53,
        FreeCashEnd     = 54,
        DiamondStart    = 57,
        DiamondSpin     = 58,
        DiamondEnd      = 59,
        
        PurFree         = 66,
    }
}
