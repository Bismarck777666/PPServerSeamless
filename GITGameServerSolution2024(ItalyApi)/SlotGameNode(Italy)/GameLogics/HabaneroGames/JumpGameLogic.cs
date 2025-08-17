using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class JumpWinStack
    {
        public List<int> StackSymbols    { get; set; }
        public JumpWinStack()
        {
            StackSymbols = new List<int>();
        }
    }
    public class JumpBetInfo : BaseHabaneroSlotBetInfo
    {
        public Dictionary<double, JumpWinStack> StackPerBets { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            int count       = reader.ReadInt32();
            StackPerBets    = new Dictionary<double, JumpWinStack>();
            for (int i = 0; i < count; i++)
            {
                double key = reader.ReadDouble();
                JumpWinStack stacks = new JumpWinStack();
                int cnt = reader.ReadInt32();
                for(int j = 0; j < cnt; j++)
                    stacks.StackSymbols.Add(reader.ReadInt32());
                StackPerBets.Add(key, stacks);
            }
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            if (this.StackPerBets == null)
            {
                writer.Write(0);
                return;
            }
            writer.Write(this.StackPerBets.Count);
            foreach (KeyValuePair<double, JumpWinStack> pair in this.StackPerBets)
            {
                writer.Write(pair.Key);

                if (pair.Value.StackSymbols == null)
                {
                    writer.Write(0);
                    continue;
                }
                
                writer.Write(pair.Value.StackSymbols.Count);
                foreach (int symbol in pair.Value.StackSymbols)
                {
                    writer.Write(symbol);
                }
            }
        }

        public JumpBetInfo(float minibet): base(minibet)
        {
            this.MiniBet        = minibet;
            this.StackPerBets   = new Dictionary<double, JumpWinStack>();
        }
    }

    public class JumpGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGJump";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "e03e3f77-9884-4147-a0ad-7caee5651934";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "980aac2b6ea501949121d7947192e4a57a34522a";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.3256.254";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 25.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 192;
            }
        }
        protected override string BetType
        {
            get
            {
                return "Ways";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",        name = "idWild"     } },    //조커
                    {2,   new HabaneroLogSymbolIDName{id = "idSeven",       name = "Seven"      } },    //7
                    {3,   new HabaneroLogSymbolIDName{id = "idBell",        name = "Bell"       } },    //벨
                    {4,   new HabaneroLogSymbolIDName{id = "idWatermelon",  name = "Watermelon" } },    //수박
                    {5,   new HabaneroLogSymbolIDName{id = "idLemon",       name = "Lemon"      } },    //레몬
                    {6,   new HabaneroLogSymbolIDName{id = "idPlum",        name = "Plum"       } },    //추리
                    {7,   new HabaneroLogSymbolIDName{id = "idCherry",      name = "Cherry"     } },    //체리

                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 420;
            }
        }
        #endregion

        public JumpGameLogic()
        {
            _gameID     = GAMEID.Jump;
            GameName    = "Jump";
        }

        protected override void onDoInit(string strGlobalUserID, int currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                string strGrid          = (string)message.Pop();
                string strToken         = (string)message.Pop();

                HabaneroResponse response = new HabaneroResponse();
                
                HabaneroResponseHeader header = makeHabaneroResponseHeader(strGlobalUserID, currency, userBalance, strToken);

                JObject portMessage = new JObject();
                portMessage["reelid"]           = InitReelStatusNo;
                portMessage["pssid"]            = Guid.NewGuid().ToString();
                portMessage["Jump_betStates"]   = new JObject();
                
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseHabaneroSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                    string gameInstanceId   = _dicUserResultInfos[strGlobalUserID].GameId;
                    string gameRoundId      = _dicUserResultInfos[strGlobalUserID].RoundId;

                    JObject jumpBetStates = buildBetStates(betInfo);
                    portMessage["Jump_betStates"] = jumpBetStates;

                    dynamic lastResult = JsonConvert.DeserializeObject<dynamic>(_dicUserResultInfos[strGlobalUserID].ResultString);
                    if(!object.ReferenceEquals(lastResult["isgamedone"], null) && !Convert.ToBoolean(lastResult["isgamedone"]))
                    {
                        JArray resumeGames = buildInitResumeGame(strGlobalUserID, betInfo,lastResult,gameInstanceId,gameRoundId,_dicUserResultInfos[strGlobalUserID].CurrentAction);
                        portMessage["resumegames"]  = resumeGames;
                        portMessage["gssid"]        = lastResult["gssid"];
                        if (!object.ReferenceEquals(lastResult["pssid"], null))
                            portMessage["pssid"] = lastResult["pssid"];
                    }
                }

                JObject game = new JObject();
                game["action"]      = "init";
                game["apiversion"]  = "5.1.10768.643";
                game["brandgameid"] = BrandGameId;
                game["friendlyid"]  = 0;
                game["gamehash"]    = GameHash;
                game["gameid"]      = "00000000-0000-0000-0000-000000000000";
                game["gameversion"] = GameVersion;
                game["jphash"]      = JPHash;
                game["jpversion"]   = JPVersion;
                game["rnghash"]     = RngHash;
                game["rngversion"]  = RngVersion;
                game["sessionid"]   = Guid.NewGuid().ToString();
                game["init"]        = new JObject();
                game["init"]["coinsincrement"]  = string.Join("|", CoinsIncrement);

                List<double> newStakeIncrement = new List<double>();
                for (int i = 0; i < StakeIncrement.Length; i++)
                {
                    newStakeIncrement.Add(StakeIncrement[i] * new Currencies()._currencyInfo[currency].Rate);
                }

                game["init"]["stakeincrement"]  = string.Join("|", newStakeIncrement);
                game["init"]["configid"]        = Guid.NewGuid().ToString();
                game["init"]["defaultstake"]    = newStakeIncrement[4];
                game["init"]["maxpaylimit"]     = MaxPayLimit * newStakeIncrement[newStakeIncrement.Count - 1];
                game["init"]["maxstake"]        = MiniCoin * CoinsIncrement[CoinsIncrement.Length - 1] * newStakeIncrement[newStakeIncrement.Count - 1];
                game["init"]["minstake"]        = MiniCoin * CoinsIncrement[0] * newStakeIncrement[0];

                response.game           = game;
                response.header         = header;
                response.grid           = strGrid;
                response.portmessage    = portMessage;
                
                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_HABANERO_DOINIT);
                responseMessage.Append(JsonConvert.SerializeObject(response));
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JumpGameLogic::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }

        protected override BaseHabaneroSlotSpinResult calculateResult(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, string strSpinResponse, bool isFirst,HabaneroActionType currentAction)
        {
            try
            {
                BaseHabaneroSlotSpinResult spinResult = new BaseHabaneroSlotSpinResult();
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(resultContext, betInfo.TotalBet);

                string strNextAction        = (string)resultContext["nextgamestate"];
                spinResult.NextAction       = convertStringToAction(strNextAction);
                spinResult.CurrentAction    = currentAction;
                if (isFirst)
                {
                    spinResult.RoundId = (((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds) * 5).ToString();
                    spinResult.GameId = Guid.NewGuid().ToString();
                }
                else
                {
                    spinResult.RoundId = _dicUserResultInfos[strGlobalUserID].RoundId;
                    spinResult.GameId = _dicUserResultInfos[strGlobalUserID].GameId;
                }

                if (spinResult.NextAction == HabaneroActionType.MAIN)
                    spinResult.TotalWin = Convert.ToDouble(resultContext["totalwincash"]);

                double currentBet = Math.Round(betInfo.TotalBet, 2);

                double winCash = 0;
                if (!object.ReferenceEquals(resultContext["wincash"], null))
                    winCash = Convert.ToDouble(resultContext["wincash"]);

                int winSymbolCnt        = 0;
                JumpWinStack winStack   = null;
                if ((betInfo as JumpBetInfo).StackPerBets.TryGetValue(currentBet, out winStack))
                {
                    if (winStack.StackSymbols == null)
                        changeJumpFullStack(0, resultContext);
                    else
                    {
                        winSymbolCnt = winStack.StackSymbols.Count;
                        changeJumpFullStack(winSymbolCnt, resultContext);
                        if(winSymbolCnt == 6)
                            (betInfo as JumpBetInfo).StackPerBets[currentBet] = new JumpWinStack();
                    }
                }
                else
                {
                    changeJumpFullStack(0, resultContext);
                }
                
                if (winCash > 0 && winSymbolCnt < 6)
                {
                    List<int> winSymbols = getWinSymbols(resultContext);

                    if ((betInfo as JumpBetInfo).StackPerBets.TryGetValue(currentBet, out winStack))
                    {

                        foreach(int winSymbol in winSymbols)
                        {
                            if(!winStack.StackSymbols.Any(_ => _ == winSymbol))
                                winStack.StackSymbols.Add(winSymbol);
                        }
                    }
                    else
                    {
                        winStack = new JumpWinStack();
                        
                        foreach (int winSymbol in winSymbols)
                        {
                            if (!winStack.StackSymbols.Any(_ => _ == winSymbol))
                                winStack.StackSymbols.Add(winSymbol);
                        }

                        (betInfo as JumpBetInfo).StackPerBets.Add(currentBet, winStack);
                    }
                }
                
                addJumpBS(betInfo, resultContext);
                spinResult.ResultString = JsonConvert.SerializeObject(resultContext);
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JumpGameLogic::calculateResult {0}", ex);
                return null;
            }
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                float coinValue = (float)message.Pop();
                int lineCount   = (int)message.Pop();
                int betLevel    = (int)message.Pop();

                BaseHabaneroSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.CoinValue    = coinValue;
                    oldBetInfo.LineCount    = lineCount;
                    oldBetInfo.BetLevel     = betLevel;
                }
                else
                {
                    JumpBetInfo betInfo = new JumpBetInfo(MiniCoin);
                    betInfo.CoinValue   = coinValue;
                    betInfo.LineCount   = lineCount;
                    betInfo.BetLevel    = betLevel;

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JumpGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        
        protected override BaseHabaneroSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            JumpBetInfo betInfo = new JumpBetInfo(MiniCoin);
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        
        protected override JObject buildBetStates(BaseHabaneroSlotBetInfo betInfo)
        {
            JObject jumpBetStates = new JObject();
            JumpBetInfo jumpBetInfo = betInfo as JumpBetInfo;
            foreach (KeyValuePair<double, JumpWinStack> pair in jumpBetInfo.StackPerBets)
            {
                if(pair.Value != null && pair.Value.StackSymbols != null && pair.Value.StackSymbols.Count > 0)
                {
                    string  betStateItemKey     = string.Format("1;{0:N6}", pair.Key / MiniCoin);
                    JObject betStateItemValue   = new JObject();
                    for(int i = 0; i < 6; i++)
                        betStateItemValue[string.Format("sym{0}",i + 1)] = 0;

                    foreach(int winSymbol in pair.Value.StackSymbols)
                    {
                        string key = string.Format("sym{0}", winSymbol - 1);
                        if (winSymbol == 2)
                            key = "sym2";
                        if (winSymbol == 3)
                            key = "sym1";
                        betStateItemValue[key] = 1;
                    }

                    jumpBetStates[betStateItemKey] = betStateItemValue;
                }
            }
            return jumpBetStates;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid,HabaneroActionType currentAction)
        {
            JArray resumeGames = new JArray();
            JObject resumeGame = new JObject();

            if (!object.ReferenceEquals(lastResult["Jump_reSpinIndex"], null))
                resumeGame["Jump_reSpinIndex"] = lastResult["Jump_reSpinIndex"];
            resumeGame["friendlygameid"] = roundid;
            resumeGame["gameinstanceid"] = gameinstanceid;
            resumeGame["betcoin"]       = betInfo.CoinValue;
            resumeGame["betlevel"]      = betInfo.BetLevel;
            resumeGame["betlines"]      = betInfo.LineCount;
            resumeGame["virtualreels"]  = lastResult["virtualreels"];
            resumeGame["totalwincash"]  = lastResult["totalwincash"];
            
            resumeGame["numfreegames"]  = 0;
            if (!object.ReferenceEquals(lastResult["numfreegames"], null))
                resumeGame["numfreegames"] = lastResult["numfreegames"];
            
            resumeGame["currfreegame"]  = -1;
            resumeGame["gamemode"]      = "respin";
            if (!object.ReferenceEquals(lastResult["currentfreegame"], null))
            {
                resumeGame["currfreegame"]  = lastResult["currentfreegame"];
                resumeGame["gamemode"]      = "freegame";
            }

            resumeGames.Add(resumeGame);

            return resumeGames;
        }

        private List<int> getWinSymbols(dynamic resultContext)
        {
            List<int> winSymbols = new List<int>();

            if (!object.ReferenceEquals(resultContext["linewins"], null))
            {
                JArray lineWinArray = resultContext["linewins"] as JArray;
                for(int i = 0; i < lineWinArray.Count; i++)
                {
                    int symbolId = Convert.ToInt32(lineWinArray[i]["symbolid"]);
                    if (winSymbols.Any(_ => _ == symbolId))
                        continue;
                    winSymbols.Add(symbolId);
                }
            }
            return winSymbols;
        }

        private void addJumpBS(BaseHabaneroSlotBetInfo betInfo, dynamic resultContext)
        {
            JumpBetInfo jumpBetInfo = betInfo as JumpBetInfo;

            double currentBet = jumpBetInfo.TotalBet;

            JumpWinStack winStack = null;
            JObject Jump_Bs = new JObject();
            Jump_Bs["sym1"] = 0;
            Jump_Bs["sym2"] = 0;
            Jump_Bs["sym3"] = 0;
            Jump_Bs["sym4"] = 0;
            Jump_Bs["sym5"] = 0;
            Jump_Bs["sym6"] = 0;
            if ((betInfo as JumpBetInfo).StackPerBets.TryGetValue(currentBet, out winStack))
            {
                foreach(int winSymbol in winStack.StackSymbols)
                {
                    string key = string.Format("sym{0}",winSymbol - 1);
                    if (winSymbol == 2)
                        key = "sym2";
                    if (winSymbol == 3)
                        key = "sym1";

                    Jump_Bs[key] = 1;
                }
            }
            resultContext["Jump_bs"] = Jump_Bs;
        }

        private void changeJumpFullStack(int winSymbolCnt, dynamic resultContext)
        {
            if(winSymbolCnt < 6)
            {
                if (!object.ReferenceEquals(resultContext["Jump_stackedSpinSymbol"], null))
                    (resultContext as JObject).Property("Jump_stackedSpinSymbol").Remove();

                if (!object.ReferenceEquals(resultContext["Jump_stackedSpinPositions"], null))
                    (resultContext as JObject).Property("Jump_stackedSpinPositions").Remove();
            }
            else
            {
                List<List<int>> currentVirtualReels = convertJArrayToListIntArray(resultContext["virtualreels"] as JArray);
                
                Dictionary<int, int> symbolAndCounts    = new Dictionary<int, int>();
                List<int> candidateWinSymbols           = new List<int>();

                bool isWildInFirstCol = false;
                for (int i = 0; i < currentVirtualReels.Count; i++)
                {
                    if (i == 0)
                    {
                        for (int j = 2; j < currentVirtualReels[i].Count - 2; j++)
                        {
                            if (currentVirtualReels[i][j] == 1)
                            {
                                isWildInFirstCol = true;
                                break;
                            }
                        }
                    }

                    for (int j = 2; j < currentVirtualReels[i].Count - 2; j++)
                    {
                        if (currentVirtualReels[i][j] != 1)
                        {
                            if (symbolAndCounts.ContainsKey(currentVirtualReels[i][j]))
                                symbolAndCounts[currentVirtualReels[i][j]]++;
                            else
                                symbolAndCounts.Add(currentVirtualReels[i][j], 1);

                            if (i == 0 && isWildInFirstCol)
                                continue;
                            if (i == 0 && !candidateWinSymbols.Any(_ => _ == currentVirtualReels[i][j]))
                                candidateWinSymbols.Add(currentVirtualReels[i][j]);
                            if (i == 1 && isWildInFirstCol && !candidateWinSymbols.Any(_ => _ == currentVirtualReels[i][j]))
                                candidateWinSymbols.Add(currentVirtualReels[i][j]);
                        }
                    }
                }

                int stackSymbol = 0;
                List<HabaneroReelAndSymbolIndex> stackPositions = new List<HabaneroReelAndSymbolIndex>();
                //윈후보심볼의 총개수가 5이상이면 그심벌을 스택시키기
                foreach(int symbol in candidateWinSymbols)
                {
                    if (symbolAndCounts[symbol] >= 5)
                    {
                        stackSymbol = symbol;
                        break;
                    }
                }

                //후보에 5개이상이 없을때 전체개수가 5이상인 심벌을 스택시키기
                if (stackSymbol == 0)
                {
                    foreach(KeyValuePair<int,int> pair in symbolAndCounts)
                    {
                        if (pair.Value >= 5)
                        {
                            stackSymbol = pair.Key;
                            break;
                        }
                    }
                }

                if(stackSymbol != 0)
                {
                    for (int i = 0; i < currentVirtualReels.Count; i++)
                    {
                        for (int j = 2; j < currentVirtualReels[i].Count - 2; j++)
                        {
                            if (currentVirtualReels[i][j] == stackSymbol)
                                stackPositions.Add(new HabaneroReelAndSymbolIndex() { reelindex = i, symbolindex = j - 2 });

                            if (stackPositions.Count == 5)
                                break;
                        }
                        if (stackPositions.Count == 5)
                            break;
                    }
                }
                else
                {
                    //그렇지 않은경우에는 후보이외의 심벌중 5개를 후보를 제외한 한가지 심벌로 바꿔준다.(이외의 상황은 디비에서 삭제)
                    List<int> allSymbols = new List<int>() { 2, 3, 4, 5, 6, 7 };
                    foreach (int symbol in candidateWinSymbols)
                        allSymbols.Remove(symbol);

                    stackSymbol = allSymbols[Pcg.Default.Next(0, allSymbols.Count)];

                    for (int i = 0; i < currentVirtualReels.Count; i++)
                    {
                        for (int j = 2; j < currentVirtualReels[i].Count - 2; j++)
                        {
                            if (!candidateWinSymbols.Any(_ => _ == currentVirtualReels[i][j]) && currentVirtualReels[i][j] != 1)
                                stackPositions.Add(new HabaneroReelAndSymbolIndex() { reelindex = i, symbolindex = j - 2 });

                            if (stackPositions.Count == 5)
                                break;
                        }
                        if (stackPositions.Count == 5)
                            break;
                    }
                }

                resultContext["Jump_stackedSpinSymbol"]     = stackSymbol;

                JArray Jump_stackedSpinPositions = new JArray();
                foreach(HabaneroReelAndSymbolIndex item in stackPositions)
                {
                    JObject stackItem = new JObject();
                    stackItem = JObject.FromObject(item);
                    Jump_stackedSpinPositions.Add(stackItem);

                    currentVirtualReels[item.reelindex][item.symbolindex + 2] = stackSymbol;
                }

                resultContext["virtualreels"]               = convertListIntArrayToJArray(currentVirtualReels);
                resultContext["Jump_stackedSpinPositions"]  = Jump_stackedSpinPositions;
            }
        }
    }
}
