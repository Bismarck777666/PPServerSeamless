using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/****
 * 
 * 
 *          Created by Bismarck(2022.01.11)
 * 
 */

namespace CQ9Protocol
{
    public class SendCQ9MessageToUser
    {
        public string Message   { get; set; }
        public SendCQ9MessageToUser(string encryptedText)
        {
            this.Message = encryptedText;
        }
    }
    public class CQ9MessageFromUser
    {
        public object   Connection  { get; set; }
        public object   Message     { get; set; }
        public CQ9Parse Parser      { get; set; }
        public CQ9MessageFromUser(object connection, object message, CQ9Parse parser)
        {
            this.Connection = connection;
            this.Message    = message;
            this.Parser     = parser;
        }
    }
    public class CQ9MessageToUser
    {
        public object Message { get; set; }
        public CQ9MessageToUser(object message)
        {
            this.Message = message;
        }
    }

    #region 소켓메시지들
    public class CQ9RequestPacket
    {
        public string[] vals { get; set; }
    }
    public class CQ9RequestReqPacket : CQ9RequestPacket
    {
        public int      req     { get; set; }
    }
    public class CQ9RequestIrqPacket : CQ9RequestPacket
    {
        public int irq { get; set; }
    }

    public class CQ9ResponseResPacket
    {
        public int      err     { get; set; } 
        public string   msg     { get; set; }
        public int      res     { get; set; }
        public object[] vals    { get; set; }
    }
    public class CQ9ResponseIrsPacket
    {
        public int      err     { get; set; }
        public int      irs     { get; set; }
        public string   msg     { get; set; }
        public long[]   vals    { get; set; }
    }
    public class CQ9ResponseEvtPacket
    {
        public int      evt     { get; set; }
        public double[] vals    { get; set; }
    }
    #endregion

    #region 소켓파람Object
    public class CQ9ResponseInitUI
    {
        public int      Type                        { get; set; }
        public int      ID                          { get; set; }
        public int      Version                     { get; set; }
        public int      ErrorCode                   { get; set; }
        public int[]    DenomDefine                 { get; set; }
        public int[]    BetButton                   { get; set; }
        public int[]    DefaultDenomIdx             { get; set; }
        public int      MaxBet                      { get; set; }
        public int      MaxLine                     { get; set; }
        public long     WinLimitLock                { get; set; }
        public int      DollarSignId                { get; set; }
        public int      EmulatorType                { get; set; }
        public int      GameExtraDataCount          { get; set; }
        public string   ExtraData                   { get; set; }
        public string   ExtendFeatureByGame         { get; set; }
        public string   ExtendFeatureByGame2        { get; set; }
        public bool     IsReelPayType               { get; set; }
        public string   Cobrand                     { get; set; }
        public string   PlayerOrderURL              { get; set; }
        public string   PromotionData               { get; set; }
        public bool     IsShowFreehand              { get; set; }
        public bool     IsAllowFreehand             { get; set; }
        public string   FeedbackURL                 { get; set; }
        public string   UserAccount                 { get; set; }
        public int      DenomMultiple               { get; set; }
        public object   FreeTicketList              { get; set; }
        public object   FreeSpinLeftTimesInfoList   { get; set; }
        public object   RecommendList               { get; set; }
    }
    public class CQ9ResponseInitReelSet
    {
        public int Type     { get; set; }
        public int ID       { get; set; }
        public int State    { get; set; }
    }
    public class CQ9ResponseCheck
    {
        public int Type     { get; set; }
        public int ID       { get; set; }
    }

    //45번메시지 파람
    public class CQ9RequestFreeOptSelect
    {
        public int Type                 { get; set; }
        public int ID                   { get; set; }
        public int PlayerSelectState    { get; set; }
        public int PlayerSelectIndex    { get; set; }
    }
    #endregion

   
}
