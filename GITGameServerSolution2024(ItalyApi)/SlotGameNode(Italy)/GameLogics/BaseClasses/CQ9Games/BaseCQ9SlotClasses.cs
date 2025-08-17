using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;

namespace SlotGamesNode.GameLogics
{
    public class BaseCQ9SlotSpinResult
    {
        public CQ9Actions           Action              { get; set; }
        public double               TotalWin            { get; set; }
        public string               ResultString        { get; set; }
        public int                  MessageID           { get; set; }
        public virtual double WinMoney
        {
            get
            {
                return this.TotalWin;
            }
        }
        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.TotalWin);
            writer.Write(this.ResultString);
            writer.Write(this.MessageID);
            writer.Write((int)this.Action);
        }
        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.TotalWin           = reader.ReadDouble();
            this.ResultString       = reader.ReadString();
            this.MessageID          = reader.ReadInt32();
            this.Action             = (CQ9Actions)reader.ReadInt32();
        }
        public byte[] convertToByte()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    this.SerializeTo(bw);
                }
                return ms.ToArray();
            }
        }
        public BaseCQ9SlotSpinResult()
        {
            this.ResultString       = "";
        }
    }

    public enum CQ9Actions
    {
        None                    = 0,
        NormalSpin              = 1,
        EndSpin                 = 2, 
        FreeSpinStart           = 3,
        FreeSpin                = 4,
        FreeSpinResult          = 5,
        FreeSpinOptionStart     = 6,
        FreeSpinOptionSelect    = 7,
        FreeSpinOptionResult    = 8,
        TembleSpin             = 9,

    }
    public enum CQ9LogGameType
    {
        NormalSpin      = 0,
        NormalSpinPay   = 1,
        FreeSpin        = 50,
        FreeSpinPay     = 51,
        TembleSpin      = 30,
    }
    public class BaseCQ9ActionToResponse
    {
        public CQ9Actions       ActionType  { get; set; }
        public string           Response    { get; set; }

        public BaseCQ9ActionToResponse(CQ9Actions actionType, string strResponse)
        {
            this.ActionType = actionType;
            this.Response   = strResponse;
        }
    }

    public class BaseCQ9SlotBetInfo
    {
        public int                              PlayLine            { get; set; }
        public int                              PlayBet             { get; set; }
        public int                              PlayDenom           { get; set; }
        public int                              MiniBet             { get; set; }
        public int                              IsExtraBet          { get; set; }
        public double                           ReelPay             { get; set; }
        public List<int>                        ReelSelected        { get; set; }
        public BasePPSlotSpinData               SpinData            { get; set; }
        public List<BaseCQ9ActionToResponse>    RemainReponses      { get; set; }

        public bool HasRemainResponse
        {
            get
            {
                if (this.RemainReponses != null && this.RemainReponses.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public virtual double TotalBet
        {
            get
            {
                return  ((double)((double)MiniBet * PlayLine * PlayBet * PlayDenom / 10000.0));
            }
        }
        public BaseCQ9SlotBetInfo()
        {
        }
        public BaseCQ9ActionToResponse pullRemainResponse()
        {
            if (RemainReponses == null || RemainReponses.Count == 0)
                return null;

            BaseCQ9ActionToResponse response = RemainReponses[0];
            RemainReponses.RemoveAt(0);
            return response;
        }

        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.PlayLine       = reader.ReadInt32();
            this.PlayBet        = reader.ReadInt32();
            this.PlayDenom      = reader.ReadInt32();
            this.MiniBet        = reader.ReadInt32();

            int reelSelectCnt   = reader.ReadInt32();
            this.ReelSelected   = new List<int>();
            for(int i = 0; i < reelSelectCnt; i++)
                this.ReelSelected.Add(reader.ReadInt32());
            int remainCount     = reader.ReadInt32();
            if (remainCount == 0)
            {
                this.RemainReponses = null;
            }
            else
            {
                this.RemainReponses = new List<BaseCQ9ActionToResponse>();
                for(int i = 0; i < remainCount; i++)
                {
                    CQ9Actions actionType   = (CQ9Actions)(byte)reader.ReadByte();
                    string      strResponse = (string)reader.ReadString();
                    this.RemainReponses.Add(new BaseCQ9ActionToResponse(actionType, strResponse));
                }
            }
            int spinDataType    = reader.ReadInt32();
            if (spinDataType == 0)
            {
                this.SpinData = null;
            }
            else if (spinDataType == 1)
            {
                this.SpinData = new BasePPSlotSpinData();
                this.SpinData.SerializeFrom(reader);
            }
            else
            {
                this.SpinData = new BasePPSlotStartSpinData();
                this.SpinData.SerializeFrom(reader);
            }
        }
        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.PlayLine);
            writer.Write(this.PlayBet);
            writer.Write(this.PlayDenom);
            writer.Write(this.MiniBet);
            writer.Write(this.ReelSelected.Count);
            for(int i = 0; i < this.ReelSelected.Count; i++)
            {
                writer.Write(this.ReelSelected[i]);
            }
            if (this.RemainReponses == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.RemainReponses.Count);
                for(int i = 0; i < this.RemainReponses.Count; i++)
                {
                    writer.Write((byte)     this.RemainReponses[i].ActionType);
                    writer.Write((string)   this.RemainReponses[i].Response);
                }
            }
            if (this.SpinData == null)
                writer.Write(0);
            else if (this.SpinData is BasePPSlotStartSpinData)
                writer.Write(2);
            else
                writer.Write(1);
            if (this.SpinData != null)
                this.SpinData.SerializeTo(writer);
        }
        public byte[] convertToByte()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    this.SerializeTo(bw);
                }
                return ms.ToArray();
            }
        }
    }
#region CQ9 Init 부분
    public class CQ9InitData
    {
        public int      Type                    { get; set; }
        public int      ID                      { get; set; }
        public int      Version                 { get; set; }
        public int      ErrorCode               { get; set; }
        public int[]    DenomDefine             { get; set; }
        public int[]    BetButton               { get; set; }
        public int      DefaultDenomIdx         { get; set; }
        public int      MaxBet                  { get; set; }
        public int      MaxLine                 { get; set; }
        public int      WinLimitLock            { get; set; }
        public int      DollarSignId            { get; set; }
        public int      EmulatorType            { get; set; }
        public int      GameExtraDataCount      { get; set; }
        public object   ExtraData               { get; set; }
        public object   ExtendFeatureByGame     { get; set; }
        public object   ExtendFeatureByGame2    { get; set; }
        public bool     IsReelPayType           { get; set; }
        public object   Cobrand                 { get; set; }
        public string   PlayerOrderURL          { get; set; }
        public CQ9PromotionItem[] PromotionData { get; set; }
        public bool     IsShowFreehand          { get; set; }
        public bool     IsAllowFreehand         { get; set; }
        public string   FeedbackURL             { get; set; }
        public string   UserAccount             { get; set; }
        public int      DenomMultiple           { get; set; }
        public object   FreeTicketList          { get; set; }
        public object   FreeSpinLeftTimesInfoList { get; set; }
        public CQ9RecommendItem RecommendList   { get; set; }
        public string   Currency                { get; set; }

        public CQ9InitData()
        {
            this.Type                       = 1;
            this.ID                         = 111;
            this.Version                    = 0;
            this.ErrorCode                  = 0;
            this.DefaultDenomIdx            = 4;
            this.MaxBet                     = 500;
            this.MaxLine                    = 1;
            this.WinLimitLock               = 300000000;
            this.DollarSignId               = 0;
            this.EmulatorType               = 0;
            this.GameExtraDataCount         = 0;
            this.ExtraData                  = null;
            this.ExtendFeatureByGame        = null;
            this.ExtendFeatureByGame2       = null;
            this.IsReelPayType              = true;
            this.Cobrand                    = null;
            this.PlayerOrderURL             = "";
            this.PromotionData              = null;
            this.IsShowFreehand             = false;
            this.IsAllowFreehand            = false;
            this.FeedbackURL                = "";
            this.UserAccount                = "";
            this.DenomMultiple              = 10000;
            this.FreeTicketList             = null;
            this.FreeSpinLeftTimesInfoList  = null;
            this.RecommendList              = null;
            this.Currency                   = "";
        }
    }
    public class CQ9ExtendedFeature2
    {
        public string name  { get; set; }
        public string value { get; set; }
    }
    public class CQ9PromotionItem
    {
        public string           name            { get; set; }
        public string           promotionid     { get; set; }
        public string           promourl        { get; set; }
        public string           imageurl        { get; set; }
        public CQ9PromotionIcon icon            { get; set; }
        public bool             haslink         { get; set; }
    }

    public class CQ9PromotionIcon
    {
        public string png   { get; set; }
        public string json  { get; set; }
    }
    public class CQ9RecommendItem
    {
        public CQ9RecommendGameItem[]   recommendGameList   { get; set; }
        public CQ9HotGameItem[]         hotRankingGameList  { get; set; }
    }
    public class CQ9RecommendGameItem
    {
        public int      type            { get; set; }
        public string   gameCode        { get; set; }
        public string   maxScore        { get; set; }
        public int      maxMultiplier   { get; set; }
        public string   iconURL         { get; set; }
        public string   backgroundURL   { get; set; }
        public CQ9RecommendGameItem(CQ9GameDBItem item)
        {
            this.type           = 1;
            this.gameCode       = item.Symbol;
            this.maxScore       = item.MaxScore.ToString();
            this.maxMultiplier  = Convert.ToInt32(item.MaxMultiple);
            this.iconURL        = item.IconUrl;
            this.backgroundURL  = item.BackGroundurl;
        }
    }
    public class CQ9HotGameItem
    {
        public int      type            { get; set; }
        public string   gameCode        { get; set; }
        public string   maxScore        { get; set; }
        public int      maxMultiplier   { get; set; }
        public string   iconURL         { get; set; }
        public string   backgroundURL   { get; set; }
        public int      difficulty      { get; set; }
        public CQ9HotGameItem(CQ9GameDBItem item)
        {
            this.type           = 2;
            this.gameCode       = item.Symbol;
            this.maxScore       = item.MaxScore.ToString();
            this.maxMultiplier  = Convert.ToInt32(item.MaxMultiple);
            this.iconURL        = item.IconUrl;
            this.backgroundURL  = item.BackGroundurl;
            this.difficulty     = item.Difficulty;
        }
    }
    public class CQ9GameDBItem
    {
        public string   Symbol          { get; set; }
        public string   IconUrl         { get; set; }
        public string   BackGroundurl   { get; set; }
        public int      Difficulty      { get; set; }
        public double   MaxScore        { get; set; }
        public double   MaxMultiple     { get; set; }
        public CQ9GameDBItem()
        {
        }
        public CQ9GameDBItem(string symbol,string iconurl,string background,int difficulty)
        {
            this.Symbol         = symbol;
            this.IconUrl        = iconurl;
            this.BackGroundurl  = background;
            this.Difficulty     = difficulty;
            this.MaxMultiple    = Pcg.Default.Next(30, 300);
            this.MaxScore       = Math.Max(Pcg.Default.Next(10000, 50000),this.MaxMultiple * 100);

        }
    }
    #endregion

    public class CQ9WinInfoItem
    {
        public string   SymbolId        { get; set; }
        public int      LinePrize       { get; set; }
        public int      NumOfKind       { get; set; }
        public int      SymbolCount     { get; set; }
        public int      WinLineNo       { get; set; }
        public int      LineMultiplier  { get; set; }
        public int[][]  WinPosition     { get; set; }
        public int[]    LineExtraData   { get; set; }
        public int      LineType        { get; set; }
    }

    public class CQ9HistoryItem
    {
        public List<CQ9ResponseHistory>     Responses { get; set; }
        public CQ9HistoryItem()
        {
            this.Responses = new List<CQ9ResponseHistory>();
        }

        public void SerializeFrom(BinaryReader reader)
        {
            this.Responses = new List<CQ9ResponseHistory>();
            int count = reader.ReadInt32();
            for(int i = 0; i < count; i++)
            {
                CQ9ResponseHistory responseHistory = new CQ9ResponseHistory();
                responseHistory.SerializeFrom(reader);
                this.Responses.Add(responseHistory);
            }
        }
        public byte[] convertToByte()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    this.SerializeTo(bw);
                }
                return ms.ToArray();
            }
        }
        private void SerializeTo(BinaryWriter writer)
        {
            if(this.Responses == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.Responses.Count);
                for (int i = 0; i < this.Responses.Count; i++)
                    this.Responses[i].SerializeTo(writer);
            }
        }
        private string convertUtcToCQ9Time(DateTime dateTime, bool includeMili)
        {
            if(includeMili)
                return string.Format("{0}-04:00", dateTime.Subtract(TimeSpan.FromHours(4.0)).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff"));
            else
                return string.Format("{0}-04:00", dateTime.Subtract(TimeSpan.FromHours(4.0)).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));
        }
        public CQ9GameLogItem buildGameLogItem(int agentID, string strUserID, string currencySymbol, int gameID, BaseCQ9SlotBetInfo betInfo, double balance, double winMoney, string gameSymbol,string cq9GameName, List<CQ9GameName> nameSet)
        {
            CQ9GameLogItem item = new CQ9GameLogItem();
            item.AgentID    = agentID;
            item.UserID     = strUserID;
            item.GameID     = gameID;
            double realBet = betInfo.TotalBet;
            if (betInfo.ReelPay > 0)
                realBet = betInfo.ReelPay * betInfo.PlayDenom / 10000;
            item.Bet        = realBet;
            item.Win        = winMoney;
            item.Time       = DateTime.UtcNow;

            CQ9GameLogDetailWager detailWager = buildDetailWager(gameSymbol, strUserID,currencySymbol, betInfo, winMoney);

            CQ9GameLogDetail detail = new CQ9GameLogDetail();
            detail.account          = strUserID;
            detail.actionlist       = new List<CQ9GameLogActionItem>();
            detail.actionlist.Add(new CQ9GameLogActionItem("bet", realBet,      convertUtcToCQ9Time(this.Responses[0].Time, false)));
            detail.actionlist.Add(new CQ9GameLogActionItem("win", winMoney,     convertUtcToCQ9Time(DateTime.UtcNow, false)));
            detail.detail           = new JObject();
            detail.detail["wager"]  = JObject.FromObject(detailWager);
            detail.parentacc        = "gitigaming";
            item.Detail = JsonConvert.SerializeObject(detail);

            CQ9GameLogOverview overview = buildGameLogOverview(gameSymbol, detailWager.seq_no, balance, realBet, winMoney, cq9GameName, nameSet, DateTime.UtcNow);
            item.Overview = JsonConvert.SerializeObject(overview);

            item.RoundID = overview.roundid;
            return item;
        }

        public CQ9GameLogDetailWager buildDetailWager(string strGameSymbol, string strUserID, string currencySymbol,BaseCQ9SlotBetInfo betInfo, double winMoney)
        {
            dynamic firstResult = JsonConvert.DeserializeObject<dynamic>(this.Responses[0].Response);
            string strRoundID = (string) firstResult["GamePlaySerialNumber"];

            CQ9GameLogDetailWager detailWager   = new CQ9GameLogDetailWager();
            detailWager.bet_multiple            = betInfo.PlayBet.ToString();
            detailWager.bet_tid                 = string.Format("pro-bet-{0}", strRoundID);
            detailWager.win_tid                 = string.Format("pro-win-{0}", strRoundID);
            detailWager.base_game_win           = ((double) firstResult["TotalWin"] * betInfo.PlayDenom / 10000.0).ToString();
            detailWager.client_ip               = "182.19.124.56";
            detailWager.currency                = currencySymbol;
            detailWager.order_time              = convertUtcToCQ9Time(DateTime.UtcNow, false);
            detailWager.end_time                = convertUtcToCQ9Time(DateTime.UtcNow, false);
            detailWager.game_id                 = strGameSymbol;
            detailWager.game_type               = (int)CQ9LogGameType.NormalSpin;
            if (betInfo.ReelPay > 0)
                detailWager.game_type = (int)CQ9LogGameType.NormalSpinPay;
            detailWager.multiple                = firstResult["Multiple"];
            detailWager.platform                = "web";
            double realBet = betInfo.TotalBet;
            if (betInfo.ReelPay > 0)
                realBet = betInfo.ReelPay * betInfo.PlayDenom / 10000;
            detailWager.play_bet                = realBet.ToString();
            detailWager.play_denom              = betInfo.PlayDenom.ToString();
            detailWager.seq_no                  = strRoundID;
            detailWager.server_ip               = "10.9.16.15";
            detailWager.settle_type             = 0;
            detailWager.start_time              = convertUtcToCQ9Time(this.Responses[0].Time, false);
            detailWager.total_win               = winMoney.ToString();
            detailWager.user_id                 = strUserID;
            detailWager.wager_type              = findWagerType(firstResult["RespinReels"]);
            detailWager.win_line_count          = firstResult["WinLineCount"];
            detailWager.win_over_limit_lock     = 0;
            detailWager.win_type                = firstResult["WinType"];
            detailWager.pick = new List<CQ9GameLogDetailPickInfo>();
            dynamic freeSpinSelResult = null;
            for(int i = 0; i < this.Responses.Count; i++)
            {
                if(this.Responses[i].Action == CQ9Actions.FreeSpinOptionSelect)
                {
                    freeSpinSelResult = JsonConvert.DeserializeObject<dynamic>(this.Responses[i].Response);
                    break;
                }
            }
            List<CQ9GameLogDetailSub> subData = new List<CQ9GameLogDetailSub>();
            if(freeSpinSelResult != null)
            {
                CQ9GameLogDetailPickInfo pickInfo = buildPickInfo(freeSpinSelResult);
                detailWager.pick.Add(pickInfo);
            }

            int subID = 1;
            for (int i = 0; i < this.Responses.Count; i++)
            {
                if (this.Responses[i].Action == CQ9Actions.FreeSpin)
                {
                    if(betInfo.ReelPay == 0)
                        subData.Add(buildSub(betInfo, this.Responses[i].Response, subID, CQ9LogGameType.FreeSpin, strGameSymbol));
                    else
                        subData.Add(buildSub(betInfo, this.Responses[i].Response, subID, CQ9LogGameType.FreeSpinPay, strGameSymbol));
                    subID++;
                }
                else if(this.Responses[i].Action == CQ9Actions.TembleSpin)
                {
                    subData.Add(buildSub(betInfo, this.Responses[i].Response, subID, CQ9LogGameType.TembleSpin, strGameSymbol));
                    subID++;
                }
            }

            detailWager.sub   = subData;
            detailWager.proof = buildProof(betInfo, firstResult, strGameSymbol);
            detailWager.rng   = firstResult["RngData"];
            return detailWager;
        }
        private int findWagerType(JArray respinArray)
        {           
            for(int i = 0; i < respinArray.Count; i++)
            {
                if ((int) respinArray[i] == 1)
                    return 1;
            }
            return 0;
        }
        private CQ9GameLogDetailSub buildSub(BaseCQ9SlotBetInfo betInfo, string strResult, int subNo,CQ9LogGameType gametype, string strGameSymbol)
        {
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(strResult);
            CQ9GameLogDetailSub subData = new CQ9GameLogDetailSub();
            subData.sub_no          = subNo;
            subData.game_type       = (int)gametype;
            subData.multiple        = resultContext["Multiple"].ToString();
            subData.rng             = resultContext["RngData"];
            subData.win             = ((double)resultContext["TotalWin"] * betInfo.PlayDenom / 10000.0).ToString();
            subData.win_line_count  = resultContext["WinLineCount"];
            subData.win_type        = resultContext["WinType"];
            subData.proof           = buildProof(betInfo, resultContext, strGameSymbol);
            return subData;
        }
        private CQ9GameLogDetailPickInfo buildPickInfo(dynamic resultContext)
        {
            CQ9GameLogDetailPickInfo pickInfo = new CQ9GameLogDetailPickInfo();
            pickInfo.game_type = 777;
            pickInfo.multiple  = "0";
            pickInfo.pick_no   = 1;
            pickInfo.win       = "0";
            pickInfo.proof     = new JObject();
            pickInfo.proof.extend_feature_by_game2  = new JArray();
            pickInfo.proof.extra_options            = new JArray();
            pickInfo.proof.multiple_options         = resultContext["udcDataSet"]["SelMultiplier"];
            pickInfo.proof.win_options              = resultContext["udcDataSet"]["SelWin"];
            pickInfo.proof.fg_rounds = 0;
            pickInfo.proof.fg_times = 0;
            pickInfo.proof.jp_item_level = null;
            pickInfo.proof.jp_item_selected = null;
            pickInfo.proof.next_s_table = 0;
            pickInfo.proof.player_selected      = resultContext["udcDataSet"]["PlayerSelected"];
            pickInfo.proof.spin_times_options   = resultContext["udcDataSet"]["SelSpinTimes"];
            return pickInfo;
        }
        private CQ9GameLogOverview buildGameLogOverview(string gameSymbol, string strRoundID, double balance, double betMoney, double winMoney, string cq9GameName, List<CQ9GameName> nameSet, DateTime createTime)
        {
            CQ9GameLogOverview overview = new CQ9GameLogOverview();
            overview.bets               = betMoney;
            overview.wins               = winMoney;
            overview.balance            = balance + winMoney;
            overview.createtime         = convertUtcToCQ9Time(createTime, true);
            overview.gamecode           = gameSymbol;
            overview.gamename           = cq9GameName;
            overview.nameset            = nameSet;
            overview.roundid            = strRoundID;

            dynamic freeSpinStartResult = null;
            foreach(CQ9ResponseHistory response in this.Responses)
            {
                if(response.Action == CQ9Actions.FreeSpinStart)
                {
                    freeSpinStartResult = JsonConvert.DeserializeObject<dynamic>(response.Response);
                    break;
                }
            }

            overview.detail = new JArray();
            JObject freeGameObj         = new JObject();
            JObject luckyDrawObj        = new JObject();
            JObject bonusObj            = new JObject();
            bonusObj["bonus"] = 0;
            if (freeSpinStartResult == null)
            {
                freeGameObj["freegame"]   = 0;
                luckyDrawObj["luckydraw"] = 0;
            }
            else
            {
                freeGameObj["freegame"]   = freeSpinStartResult["AwardSpinTimes"];
                luckyDrawObj["luckydraw"] = 1;
            }
            overview.detail.Add(freeGameObj);
            overview.detail.Add(luckyDrawObj);
            overview.detail.Add(bonusObj);

            return overview;
        }        
        private dynamic buildProof(BaseCQ9SlotBetInfo betInfo, dynamic resultContext, string strGameSymbol)
        {
            dynamic proof = new JObject();
            proof["bonustype"]                  = resultContext["BonusType"];
            proof["bonus_type"]                 = resultContext["BonusType"];
            proof["denom_multiple"]             = betInfo.PlayDenom;
            proof["extend_feature_by_game"]     = new JArray();

            JArray ExtendFeatureByGame2     = resultContext["ExtendFeatureByGame2"];
            JArray NewExtendFeatureByGame2  = new JArray();
            if (strGameSymbol == "GB12" || strGameSymbol == "225" || strGameSymbol == "226" || strGameSymbol == "227" || strGameSymbol == "229" || strGameSymbol == "231")
            {
                for (int i = 0; i < ExtendFeatureByGame2.Count; i++)
                {
                    if (!object.ReferenceEquals(ExtendFeatureByGame2[i]["Name"], null))
                    {
                        JObject item = new JObject();

                        item["name"] = ExtendFeatureByGame2[i]["Name"];
                        if (!object.ReferenceEquals(ExtendFeatureByGame2[i]["Value"], null))
                            item["value"] = ExtendFeatureByGame2[i]["Value"];
                        else
                            item["value"] = null;

                        NewExtendFeatureByGame2.Add(JsonConvert.DeserializeObject<dynamic>(item.ToString()));
                    }
                }
            }
            else
            {
                NewExtendFeatureByGame2 = resultContext["ExtendFeatureByGame2"];
            }

            proof["extend_feature_by_game2"]    = NewExtendFeatureByGame2;
            proof["extra_data"]                 = resultContext["ExtraData"];
            proof["fg_rounds"]                  = 0;
            proof["fg_times"]                   = 0;
            proof["free_ticket_times"]          = 0;
            proof["free_ticket_used_times"]     = 0;

            proof["is_respin"]                  = resultContext["IsRespin"];
            proof["lock_position"]              = new JArray();
            if (!object.ReferenceEquals(resultContext["LockPos"], null))
                proof["lock_position"] = resultContext["LockPos"];
            proof["next_s_table"]               = resultContext["NextSTable"];
            proof["reel_len_change"]            = resultContext["ReelLenChange"];
            proof["reel_pay"]                   = resultContext["ReelPay"];
            if (betInfo.ReelPay > 0)
            {
                JArray reel_pay = new JArray();
                reel_pay.Add((long)betInfo.ReelPay);
                proof["reel_pay"]= reel_pay;
            }
            proof["reel_pos_chg"]               = resultContext["ReellPosChg"];
            proof["respin_reels"]               = resultContext["RespinReels"];
            proof["special_award"]              = resultContext["SpecialAward"];
            proof["special_symbol"]             = resultContext["SpecialSymbol"];
            proof["symbol_data"]                = resultContext["SymbolResult"];
            proof["symbol_data_after"]          = new JArray();

            JArray winLineData = new JArray();
            JArray winLineArray = resultContext["udsOutputWinLine"] as JArray;
            for (int i = 0; i < winLineArray.Count; i++)
                winLineData.Add(convertWinLine(winLineArray[i], betInfo));
            proof["win_line_data"] = winLineData;
            return proof;

        }
        private dynamic convertWinLine(dynamic winLineResult, BaseCQ9SlotBetInfo betInfo)
        {
            JObject winLine = new JObject();
            winLine["line_extra_data"]  = winLineResult["LineExtraData"];
            winLine["line_multiplier"]  = winLineResult["LineMultiplier"];
            winLine["line_prize"]       = (double)winLineResult["LinePrize"] * betInfo.PlayDenom / 10000.0;
            winLine["line_type"]        = winLineResult["LineType"];
            winLine["num_of_kind"]      = winLineResult["NumOfKind"];
            winLine["symbol_count"]     = winLineResult["SymbolCount"];
            winLine["symbol_id"]        = winLineResult["SymbolId"];
            winLine["win_line_no"]      = winLineResult["WinLineNo"];
            winLine["win_position"]     = winLineResult["WinPosition"];

            return winLine;

        }
    }
    
    public class CQ9GameLogItem
    {
        public  int         AgentID     { get; set; }
        public  string      UserID      { get; set; }
        public  int         GameID      { get; set; }
        public  string      RoundID     { get; set; }
        public  double      Bet         { get; set; }
        public  double      Win         { get; set; }
        public  string      Overview    { get; set; }
        public  string      Detail      { get; set; }
        public  DateTime    Time        { get; set; }
    }
    public class CQ9ResponseHistory
    {
        public CQ9Actions   Action      { get; set; }
        public DateTime     Time        { get; set; }
        public string       Response    { get; set; }
        public CQ9ResponseHistory()
        {

        }
        public CQ9ResponseHistory(CQ9Actions action, DateTime time, string strResponse)
        {
            this.Action     = action;
            this.Time       = time;
            this.Response   = strResponse;
        }
        public void SerializeTo(BinaryWriter writer)
        {
            writer.Write((int)this.Action);
            writer.Write(this.Time.ToString());
            writer.Write(this.Response);
        }
        public void SerializeFrom(BinaryReader reader)
        {
            this.Action     = (CQ9Actions)(int)reader.ReadInt32();
            this.Time       = DateTime.Parse(reader.ReadString());
            this.Response   = reader.ReadString();
        }
    }
    public class CQ9GameLogOverview
    {
        public double       balance     { get; set; }
        public double       bets        { get; set; }
        public string       bettype     { get; set; }
        public string       createtime  { get; set; }

        public JArray      detail      { get; set; }
        public string       gamecode    { get; set; }
        public string       gamehall    { get; set; }
        public string       gamename    { get; set; }
        public string       gameplat    { get; set; }
        public string       gametype    { get; set; }
        public string       giventype   { get; set; }
        public double       jackpot     { get; set; }
        public List<double> jackpotcontribution { get; set; }
        public string       jackpottype { get; set; }
        public string       roundid     { get; set; }
        public string       roundnumber { get; set; }
        public bool         singlerowbet { get; set; }
        public string       tableid     { get; set; }
        public string       tabletype   { get; set; }
        public string       ticketid    { get; set; }
        public string       tickettype  { get; set; }
        public double       validbet    { get; set; }
        public double       wins        { get; set; }

        public List<CQ9GameName>            nameset     { get; set; }
        public CQ9GameLogOverviewResult     gameresult  { get; set; }
        public CQ9GameLogOverview()
        {
            this.bettype = null;
            this.detail  = new JArray();
            this.gamehall = "cq9";
            this.gameplat = "web";
            this.gameresult = new CQ9GameLogOverviewResult();
            this.gametype = "slot";
            this.giventype = "";
            this.jackpot = 0.0;
            this.jackpotcontribution = new List<double>();
            this.jackpottype = "";
            this.roundnumber = "";
            this.singlerowbet = false;
            this.tableid = "";
            this.tabletype = "";
            this.ticketid = "";
            this.tickettype = "";
            this.validbet = 0.0;
        }
    }
    public class CQ9GameName
    {
        public string lang { get; set; }
        public string name { get; set; }
    }
    public class CQ9GameLogOverviewResult
    {
        public string cards  { get; set; }
        public string points { get; set; }
    }    
    public class CQ9GameLogDetailPickInfo
    {
        public int      game_type   { get; set; }
        public string   multiple    { get; set; }
        public int      pick_no     { get; set; }
        public string   win         { get; set; }
        public dynamic  proof       { get; set; }
    }
    public class CQ9GameLogDetail
    {
        public string                       account     { get; set; }
        public List<CQ9GameLogActionItem>   actionlist  { get; set; }
        public dynamic                      detail      { get; set; }
        public string                       parentacc   { get; set; }
    }
    public class CQ9GameLogDetailWager
    {
        public string base_game_win { get; set; }
        public string bet_multiple  { get; set; }
        public string bet_tid       { get; set; }
        public string client_ip     { get; set; }
        public string currency      { get; set; }
        public string end_time      { get; set; }
        public string game_id       { get; set; }
        public int    game_type     { get; set; }
        public string    multiple      { get; set; }
        public string order_time    { get; set; }
        
        public List<CQ9GameLogDetailPickInfo> pick { get; set; }
        public string platform      { get; set; }
        public string play_bet      { get; set; }
        public string play_denom    { get; set; }
        public dynamic proof        { get; set; }
        public dynamic rng        { get; set; }
        public string seq_no        { get; set; }
        public string server_ip     { get; set; }
        public int  settle_type     { get; set; }
        public string start_time    { get; set; }

        public List<CQ9GameLogDetailSub> sub { get; set; }
        public string                   total_win { get; set; }
        public string                   user_id { get; set; }
        public int                      wager_type { get; set; }
        public int                      win_line_count { get; set;}
        public int                      win_over_limit_lock { get; set; }
        public string                   win_tid { get; set; }
        public int                      win_type { get; set; }

        public CQ9GameLogDetailWager()
        {

        }

    }
    public class CQ9GameLogDetailSub
    {
        public int          game_type       { get; set; }
        public string       multiple        { get; set; }
        public dynamic      proof           { get; set; }
        public dynamic      rng             { get; set; }
        public int          sub_no          { get; set; }
        public string       win             { get; set; }
        public int          win_line_count  { get; set; }
        public int          win_type        { get; set; }
    }
    public class CQ9GameLogActionItem
    {
        public string   action      { get; set; }
        public double   amount      { get; set; }
        public string eventtime   { get; set; }

        public CQ9GameLogActionItem()
        {

        }
        public CQ9GameLogActionItem(string action, double amount, string time)
        {
            this.action = action;
            this.amount = amount;
            this.eventtime = time;
        }

    }
}
