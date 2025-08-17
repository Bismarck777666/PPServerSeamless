using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;

namespace SlotGamesNode.GameLogics
{
    public class BasePlaysonGroupSlotBetInfo : BasePlaysonSlotBetInfo
    {
        public Dictionary<float, List<BasePlaysonActionToResponse>>     SeqRemainReponses   { get; set; }
        public Dictionary<float, string>                                SeqResultStrings    { get; set; }

        public override bool HasRemainResponse
        {
            get
            {
                if (SeqRemainReponses == null)
                    return false;
                if (!SeqRemainReponses.ContainsKey(TotalBet))
                    return false;
                if (SeqRemainReponses[TotalBet] == null)
                    return false;
                if(SeqRemainReponses[TotalBet].Count == 0)
                    return false;
                
                return true;
            }
        }

        public BasePlaysonGroupSlotBetInfo()
        {
            SeqRemainReponses   = new Dictionary<float, List<BasePlaysonActionToResponse>>();
            SeqResultStrings    = new Dictionary<float, string>();
        }

        public override BasePlaysonActionToResponse pullRemainResponse()
        {
            if (SeqRemainReponses[TotalBet] == null || SeqRemainReponses[TotalBet].Count == 0)
                return null;

            BasePlaysonActionToResponse response = SeqRemainReponses[TotalBet][0];
            SeqRemainReponses[TotalBet].RemoveAt(0);
            return response;
        }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            
            int seqRemainCount  = reader.ReadInt32();
            if (seqRemainCount == 0)
            {
                SeqRemainReponses = null;
            }
            else
            {
                SeqRemainReponses   = new Dictionary<float, List<BasePlaysonActionToResponse>>();
                for (int i = 0; i < seqRemainCount; i++)
                {
                    float seqKey    = reader.ReadSingle();
                    int remainCnt   = reader.ReadInt32();
                    if(remainCnt == 0)
                    {
                        if (!SeqRemainReponses.ContainsKey(seqKey))
                            SeqRemainReponses.Add(seqKey, null);
                        else
                            SeqRemainReponses[seqKey] = null;
                    }
                    else
                    {
                        List<BasePlaysonActionToResponse> remainResponses = new List<BasePlaysonActionToResponse>();
                        for (int j = 0; j < remainCnt; j++)
                        {
                            PlaysonActionTypes actionType   = (PlaysonActionTypes)(byte)reader.ReadByte();
                            string strResponse              = (string)reader.ReadString();
                            remainResponses.Add(new BasePlaysonActionToResponse(actionType, strResponse));
                        }
                        if (!SeqRemainReponses.ContainsKey(seqKey))
                            SeqRemainReponses.Add(seqKey, remainResponses);
                        else
                            SeqRemainReponses[seqKey] = remainResponses;
                    }
                }
            }
            
            int seqResultCnt = reader.ReadInt32();
            if(seqResultCnt == 0)
            {
                SeqResultStrings = new Dictionary<float, string>();
            }
            else
            {
                SeqResultStrings = new Dictionary<float, string>();
                for (int i = 0; i < seqResultCnt; i++)
                {
                    float seqKey        = reader.ReadSingle();
                    string seqResult    = reader.ReadString();
                    if (!SeqResultStrings.ContainsKey(seqKey))
                        SeqResultStrings.Add(seqKey, seqResult);
                    else
                        SeqResultStrings[seqKey] = seqResult;
                }
            }
        }
        
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            
            if (SeqRemainReponses == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(SeqRemainReponses.Count);
                foreach(KeyValuePair<float, List<BasePlaysonActionToResponse>> pair in SeqRemainReponses)
                {
                    writer.Write(pair.Key);
                    if(pair.Value == null)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(pair.Value.Count);
                        for (int i = 0; i < pair.Value.Count; i++)
                        {
                            writer.Write((byte)pair.Value[i].ActionType);
                            writer.Write((string)pair.Value[i].Response);
                        }
                    }
                }
            }

            writer.Write(SeqResultStrings.Count);
            foreach(KeyValuePair<float, string> pair in SeqResultStrings)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }
    }

    class BasePlaysonGroupGameLogic : BasePlaysonSlotGame
    {
        protected override void onDoStart(string strGlobalUserID, GITMessage message, double userBalance)
        {
            try
            {
                string strRnd   = (string)message.Pop();
                string strToken = (string)message.Pop();

                XmlDocument responseDoc = new XmlDocument();
                responseDoc.LoadXml(InitDataString);

                XmlElement serverNode = responseDoc["server"];
                if(message.MsgCode == (ushort)CSMSG_CODE.CS_PLAYSON_DORECONNECT)
                    serverNode.SetAttribute("command",  "reconnect");
                else
                    serverNode.SetAttribute("command", "start");
                serverNode.SetAttribute("rnd",      strRnd);
                serverNode.SetAttribute("session",  strToken);
                serverNode.SetAttribute("status",   "ok");

                addLastResultForStart(responseDoc,strGlobalUserID);

                XmlElement user_newNode = responseDoc.CreateElement("user_new");
                user_newNode.SetAttribute("cash", ((long)(userBalance * 100)).ToString());

                serverNode.AppendChild(user_newNode);
                responseDoc.AppendChild(serverNode);

                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_PLAYSON_DOSTART);
                responseMessage.Append(serverNode.OuterXml);
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::onDoStart GameID: {0}, {1}", _gameID, ex);
            }
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                int betPerLine  = (int)message.Pop();
                int totalBet    = (int)message.Pop();
                int purFree     = (int)message.Pop();
                
                BasePlaysonSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    if (betPerLine <= 0.0f)
                    {
                        _logger.Error("{0} betInfo.BetPerLine <= 0 in BasePlaysonGroupGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betPerLine);
                        return;
                    }

                    oldBetInfo.BetPerLine   = betPerLine;
                    oldBetInfo.TotalBet     = totalBet;
                    if(purFree == 1)
                        oldBetInfo.PurchaseFree = true;
                    else
                        oldBetInfo.PurchaseFree = false;

                    BasePlaysonGroupSlotBetInfo groupBetInfo = oldBetInfo as BasePlaysonGroupSlotBetInfo;
                    //만일 유저에게 남은 일반스핀응답이 존재하는 경우
                    if (groupBetInfo.HasRemainResponse && groupBetInfo.SeqRemainReponses[totalBet][0].ActionType != PlaysonActionTypes.SPIN)
                        return;
                }
                else
                {
                    if (betPerLine <= 0.0f)
                    {
                        _logger.Error("{0} betInfo.BetPerLine <= 0 in BasePlaysonGroupGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betPerLine);
                        return;
                    }

                    BasePlaysonGroupSlotBetInfo betInfo = new BasePlaysonGroupSlotBetInfo();
                    betInfo.BetPerLine      = betPerLine;
                    betInfo.TotalBet        = totalBet;
                    if (purFree == 1)
                        betInfo.PurchaseFree = true;
                    else
                        betInfo.PurchaseFree = false;

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonGroupGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override async Task spinGame(string strUserID, int agentID, int currency, UserBonus userBonus, double userBalance, string strRnd, string strToken, string strPlayClient)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                //해당 유저의 베팅정보를 얻는다. 만일 베팅정보가 없다면(례외상황) 그대로 리턴한다.
                BasePlaysonSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);

                BasePlaysonSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                double betMoney = betInfo.TotalBet / 100;
                BasePlaysonGroupSlotBetInfo groupBetInfo = betInfo as BasePlaysonGroupSlotBetInfo;

                if (groupBetInfo.HasRemainResponse && groupBetInfo.SeqRemainReponses[betInfo.TotalBet][0].ActionType != PlaysonActionTypes.SPIN)
                    betMoney = 0.0;

                if (SupportPurchaseFree && betInfo.PurchaseFree)
                    betMoney = Math.Round(betMoney * PurchaseFreeMultiple, 1);

                //만일 베팅머니가 유저의 밸런스보다 크다면 끝낸다.
                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    string strBalanceErrorResult = makeBalanceNotEnoughResult(strGlobalUserID, userBalance, strToken, strRnd, betInfo);
                    GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_PLAYSON_DOPLAY);
                    message.Append(strBalanceErrorResult);
                    Sender.Tell(new ToUserMessage((int)_gameID, message));
                    saveBetResultInfo(strGlobalUserID);
                    return;
                }

                //결과를 생성한다.
                BasePlaysonSlotSpinResult spinResult = await generateSpinResult(groupBetInfo, strUserID, agentID, userBonus, true);

                changeSessionAndRndToResponse(spinResult, strToken, strRnd);
                addBalanceInfo(spinResult, userBalance);

                //게임로그
                string strGameLog = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID] = spinResult;

                //결과를 보내기전에 베팅정보를 디비에 보관한다
                saveBetResultInfo(strGlobalUserID);

                //생성된 게임결과를 유저에게 보낸다.
                sendGameResult(betInfo, spinResult, strGlobalUserID, betMoney, spinResult.WinMoney, strGameLog, userBalance, strRnd, strToken);

                _dicUserLastBackupBetInfos[strGlobalUserID]     = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]  = lastResult;

                //게임결과를 디비에 보관한다.
                saveResultToHistory(spinResult,agentID ,strUserID, currency, userBalance, betMoney, spinResult.WinMoney, spinResult.CurrentAction, strPlayClient);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonGroupGameLogic::spinGame {0}", ex);
            }
        }
    
        protected void addLastNormalInfos(XmlDocument lastResultDoc, List<BasePlaysonActionToResponse> purFreeRemainResponses)
        {
            XmlNode lastGameNode    = lastResultDoc.SelectSingleNode("/server/game");

            foreach (BasePlaysonActionToResponse res in purFreeRemainResponses)
            {
                XmlDocument rootDoc = new XmlDocument();
                rootDoc.LoadXml(res.Response);
                XmlNode newGameNode = rootDoc.SelectSingleNode("/server/game");

                while (newGameNode.HasChildNodes)
                {
                    newGameNode.RemoveChild(newGameNode.FirstChild);
                }

                foreach (XmlNode xn in lastGameNode.ChildNodes)
                {
                    XmlNode copiedNode = rootDoc.ImportNode(xn, true);
                    newGameNode.AppendChild(copiedNode);
                }

                res.Response = rootDoc.InnerXml;
            }
        }

        protected override async Task<BasePlaysonSlotSpinResult> generateSpinResult(BasePlaysonSlotBetInfo betInfo, string strUserID, int agentID, UserBonus userBonus, bool usePayLimit)
        {
            BasePPSlotSpinData          spinData            = null;
            BasePlaysonSlotSpinResult   result              = null;
            BasePlaysonGroupSlotBetInfo groupSlotBetInfo    = betInfo as BasePlaysonGroupSlotBetInfo;

            //유저의 총 베팅액을 얻는다.
            float   totalBet        = groupSlotBetInfo.TotalBet;
            double  realBetMoney    = totalBet;

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (groupSlotBetInfo.HasRemainResponse)
            {
                if (SupportPurchaseFree && betInfo.PurchaseFree)
                {
                    realBetMoney    = totalBet * PurchaseFreeMultiple;//구매베팅금액
                    spinData        = await selectMinStartFreeSpinData(betInfo);
                    result          = calculateResult(strGlobalUserID, groupSlotBetInfo, spinData.SpinStrings[0], true, PlaysonActionTypes.SPIN);
                    double purFreeWin = totalBet * spinData.SpinOdd;
                    List<BasePlaysonActionToResponse> purFreeRemainResponses = buildResponseList(spinData.SpinStrings);

                    string lastResultStr = groupSlotBetInfo.SeqResultStrings[groupSlotBetInfo.BetPerLine];
                    XmlDocument lastResultDoc = new XmlDocument();
                    lastResultDoc.LoadXml(lastResultStr);
                    addLastNormalInfos(lastResultDoc, purFreeRemainResponses);

                    groupSlotBetInfo.SeqRemainReponses[groupSlotBetInfo.TotalBet].InsertRange(0, purFreeRemainResponses);

                    sumUpCompanyBetWin(agentID, realBetMoney, purFreeWin);
                }
                else
                {
                    BasePlaysonActionToResponse nextResponse = groupSlotBetInfo.pullRemainResponse();
                    result = calculateResult(strGlobalUserID, groupSlotBetInfo, nextResponse.Response, false, nextResponse.ActionType);

                    if (!groupSlotBetInfo.HasRemainResponse)
                        groupSlotBetInfo.SeqRemainReponses[groupSlotBetInfo.TotalBet] = null;
                }

                groupSlotBetInfo.SeqResultStrings[groupSlotBetInfo.BetPerLine] = result.ResultString;
                return result;
            }

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * PurchaseFreeMultiple;//구매베팅금액

            spinData = await selectRandomStop(agentID, userBonus, totalBet, betInfo);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;

            if (!usePayLimit || spinData.IsEvent || checkCompanyPayoutRate(agentID, realBetMoney, totalWin))
            {
                do
                {
                    if (spinData.IsEvent)
                    {
                        bool checkRet = await subtractEventMoney(agentID, strUserID, totalWin);
                        if (!checkRet)
                            break;
                        
                        _bonusSendMessage = null;
                        _rewardedBonusMoney = totalWin / 100;
                        _isRewardedBonus    = true;
                    }

                    result = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true,PlaysonActionTypes.SPIN);
                    if (spinData.SpinStrings.Count > 1)
                    {
                        groupSlotBetInfo.SeqRemainReponses[groupSlotBetInfo.TotalBet] = buildResponseList(spinData.SpinStrings);
                    }
                
                    groupSlotBetInfo.SeqResultStrings[groupSlotBetInfo.BetPerLine] = result.ResultString;
                    return result;
                }
                while (false);
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                result      = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, PlaysonActionTypes.SPIN);
                emptyWin    = totalBet * spinData.SpinOdd;
            }
            else
            {
                spinData    = await selectEmptySpin(agentID, betInfo);
                result      = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true,PlaysonActionTypes.SPIN);
            }

            //뒤에 응답자료가 또 있다면
            if (spinData.SpinStrings.Count > 1)
            {
                groupSlotBetInfo.SeqRemainReponses[groupSlotBetInfo.TotalBet] = buildResponseList(spinData.SpinStrings);
            }

            sumUpCompanyBetWin(agentID, realBetMoney, emptyWin);
            
            groupSlotBetInfo.SeqResultStrings[groupSlotBetInfo.BetPerLine] = result.ResultString;
            return result;
        }

        protected override BasePlaysonSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            BasePlaysonGroupSlotBetInfo betInfo = new BasePlaysonGroupSlotBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
