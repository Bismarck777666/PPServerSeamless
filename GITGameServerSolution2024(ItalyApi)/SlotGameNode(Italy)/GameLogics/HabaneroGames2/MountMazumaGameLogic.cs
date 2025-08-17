using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class MountMazumaGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGMountMazuma";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "34931f1e-9fce-44dc-9e27-7e6abd9972ab";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "4ad3208955dab6fb3ecf4c803a3ea4de1ff53a6a";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.3697.282";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 20.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 243;
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
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"           } },
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",       name = "Scatter"        } },
                    {3,     new HabaneroLogSymbolIDName{id = "idCoconut",       name = "Coconut"        } },
                    {4,     new HabaneroLogSymbolIDName{id = "idPineapple",     name = "Pineapple"      } },    
                    {5,     new HabaneroLogSymbolIDName{id = "idBanana",        name = "Banana"         } },    
                    {6,     new HabaneroLogSymbolIDName{id = "idKiwi",          name = "Kiwi"           } },    
                    {7,     new HabaneroLogSymbolIDName{id = "idOrange",        name = "Orange"         } },    
                    {8,     new HabaneroLogSymbolIDName{id = "idA",             name = "A"              } },    
                    {9,     new HabaneroLogSymbolIDName{id = "idK",             name = "K"              } },    
                    {10,    new HabaneroLogSymbolIDName{id = "idQ",             name = "Q"              } },    
                    {11,    new HabaneroLogSymbolIDName{id = "idJ",             name = "J"              } },
                    {12,    new HabaneroLogSymbolIDName{id = "idB",             name = "B"              } },//블록(록크)

                    {101,   new HabaneroLogSymbolIDName{id = "idWild_X2",       name = "Wild_X2"        } },
                    {103,   new HabaneroLogSymbolIDName{id = "idCoconut_X2",    name = "Coconut_X2"     } },
                    {104,   new HabaneroLogSymbolIDName{id = "idPineapple_X2",  name = "Pineapple_X2"   } },
                    {105,   new HabaneroLogSymbolIDName{id = "idBanana_X2",     name = "Banana_X2"      } },
                    {106,   new HabaneroLogSymbolIDName{id = "idKiwi_X2",       name = "Kiwi_X2"        } },
                    {107,   new HabaneroLogSymbolIDName{id = "idOrange_X2",     name = "Orange_X2"      } },
                    {108,   new HabaneroLogSymbolIDName{id = "idA_X2",          name = "A_X2"           } },
                    {109,   new HabaneroLogSymbolIDName{id = "idK_X2",          name = "K_X2"           } },
                    {110,   new HabaneroLogSymbolIDName{id = "idQ_X2",          name = "Q_X2"           } },
                    {111,   new HabaneroLogSymbolIDName{id = "idJ_X2",          name = "J_X2"           } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 446;
            }
        }
        #endregion

        public MountMazumaGameLogic()
        {
            _gameID     = GAMEID.MountMazuma;
            GameName    = "MountMazuma";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            dynamic eventItem = base.buildEventItem(strGlobalUserId, currentIndex);

            bool[][] lockPosNow = findLockPosition(strGlobalUserId, currentIndex);

            int numWinWays = 1;
            for (int i = 0; i < lockPosNow.Length; i++)
            {
                int colRows = 0;
                for (int j = 0; j < lockPosNow[i].Length; j++)
                {
                    if (lockPosNow[i][j])
                        eventItem["reels"][i][j] = SymbolIdStringForLog[12].id;
                    else
                        colRows++;
                }
                numWinWays *= colRows;
            }

            eventItem["numways"] = numWinWays;

            dynamic response = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);
            bool isSuperSpin = false;
            if (!object.ReferenceEquals(response["MountMazuma_isSuperSpin"], null))
                isSuperSpin = (bool)response["MountMazuma_isSuperSpin"];
            
            if(isSuperSpin)
                eventItem["numways"] = 3125;

            JArray originReels = new JArray();
            for (int i = 0; i < response["virtualreels"].Count; i++)
            {
                JArray reelsCol = new JArray();
                for (int j = 2; j < response["virtualreels"][i].Count - 2; j++)
                    reelsCol.Add(SymbolIdStringForLog[(int)response["virtualreels"][i][j]].id);
                
                originReels.Add(reelsCol);
            }

            if(currentIndex != 0)
            {
                bool[][] lockPosBefore = findLockPosition(strGlobalUserId, currentIndex - 1);
                for(int i = 0; i < lockPosBefore.Length; i++)
                {
                    for(int j = 0; j < lockPosBefore[i].Length; j++)
                    {
                        if (lockPosBefore[i][j])
                            originReels[i][j] = SymbolIdStringForLog[12].id;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    originReels[i][0] = SymbolIdStringForLog[12].id;
                    originReels[i][4] = SymbolIdStringForLog[12].id;
                }
            }

            JArray reelsList        = new JArray();
            
            JObject reelsListItem   = new JObject();
            reelsListItem["reels"]  = originReels;
            reelsList.Add(JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(reelsListItem)));

            if (!isSuperSpin)
            {
                if (!object.ReferenceEquals(response["MountMazuma_unlockList"], null) && response["MountMazuma_unlockList"].Count > 0)
                {
                    for (int i = 0; i < lockPosNow.Length; i++)
                    {
                        for (int j = 0; j < lockPosNow[i].Length; j++)
                        {
                            if (!lockPosNow[i][j])
                                originReels[i][j] = SymbolIdStringForLog[(int)response["virtualreels"][i][j + 2]].id;
                            else
                                originReels[i][j] = SymbolIdStringForLog[12].id;
                        }
                    }
                    reelsListItem["reels"] = originReels;
                    reelsList.Add(JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(reelsListItem)));
                }
            }
            else
            {
                for (int i = 0; i < lockPosNow.Length; i++)
                {
                    for (int j = 0; j < lockPosNow[i].Length; j++)
                        originReels[i][j] = SymbolIdStringForLog[(int)response["virtualreels"][i][j + 2]].id;
                }
                reelsListItem["reels"] = originReels;
                reelsList.Add(JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(reelsListItem)));
            }

            if (!object.ReferenceEquals(response["MountMazuma_wildList"],null) && response["MountMazuma_wildList"].Count > 0)
            {
                for (int i = 0; i < response["MountMazuma_wildList"].Count; i++)
                {
                    int reelindex   = response["MountMazuma_wildList"][i]["reelindex"];
                    int symbolindex = response["MountMazuma_wildList"][i]["symbolindex"];
                    response["reels"][reelindex][symbolindex]["symbolid"] = 1;
                    originReels[reelindex][symbolindex] = SymbolIdStringForLog[1].id;
                }
                reelsListItem["reels"] = originReels;
                reelsList.Add(JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(reelsListItem)));
            }

            if (!object.ReferenceEquals(response["waymultiplierlist"], null) && response["waymultiplierlist"].Count > 0)
            {
                for (int i = 0; i < response["waymultiplierlist"].Count; i++)
                {
                    int reelindex   = response["waymultiplierlist"][i]["pos"]["reelindex"];
                    int symbolindex = response["waymultiplierlist"][i]["pos"]["symbolindex"];
                    int symbolid    = (int)response["reels"][reelindex][symbolindex]["symbolid"];
                    originReels[reelindex][symbolindex] = SymbolIdStringForLog[symbolid + 100].id;
                }
                reelsListItem["reels"] = originReels;
                reelsList.Add(JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(reelsListItem)));
            }

            eventItem["reelslist"]  = reelsList;
            eventItem["reels"]      = originReels;
            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            dynamic reels = base.buildHabaneroLogReels(strGlobalUserId, currentIndex, response as JObject, containWild);
            if (!object.ReferenceEquals(response["MountMazuma_wildList"], null) && response["MountMazuma_wildList"].Count > 0)
            {
                for (int i = 0; i < response["MountMazuma_wildList"].Count; i++)
                {
                    int reelindex   = response["MountMazuma_wildList"][i]["reelindex"];
                    int symbolindex = response["MountMazuma_wildList"][i]["symbolindex"];
                    reels[reelindex][symbolindex] = SymbolIdStringForLog[1].id;
                }
            }

            if(!object.ReferenceEquals(response["waymultiplierlist"],null) && response["waymultiplierlist"].Count > 0)
            {
                for(int i = 0; i < response["waymultiplierlist"].Count; i++)
                {
                    int reelindex   = response["waymultiplierlist"][i]["pos"]["reelindex"];
                    int symbolindex = response["waymultiplierlist"][i]["pos"]["symbolindex"];
                    int symbolid    = (int)response["reels"][reelindex][symbolindex]["symbolid"];
                    reels[reelindex][symbolindex] = SymbolIdStringForLog[symbolid + 100].id;
                }
            }

            return reels;
        }

        private bool[][] findLockPosition(string strGlobalUserId, int endIndex)
        {
            bool[][] lockPos = new bool[][] {
                new bool[]{ true,false,false,false,true },
                new bool[]{ true,false,false,false,true },
                new bool[]{ true,false,false,false,true },
                new bool[]{ true,false,false,false,true },
                new bool[]{ true,false,false,false,true },
            };

            for(int i = 0; i <= endIndex; i++)
            {
                dynamic response = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[i].Response);
                if (!object.ReferenceEquals(response["MountMazuma_unlockList"], null) && response["MountMazuma_unlockList"].Count > 0)
                {
                    for(int j = 0; j < response["MountMazuma_unlockList"].Count; j++)
                    {
                        int reelindex   = response["MountMazuma_unlockList"][j]["pos"]["reelindex"];
                        int symbolindex = response["MountMazuma_unlockList"][j]["pos"]["symbolindex"];
                        lockPos[reelindex][symbolindex] = false;
                    }
                }
            }
            return lockPos;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            resumeGames[0]["betlines"] = 243;
            bool[][] lockPosNow = findLockPosition(strGlobalUserId, _dicUserHistory[strGlobalUserId].Responses.Count - 1);
            JArray blocks = new JArray();
            for(int i = 0; i < lockPosNow.Length; i++)
            {
                JArray row = new JArray();
                for (int j = 0; j < lockPosNow[i].Length; j++)
                    row.Add(lockPosNow[i][j]);
                blocks.Add(row);
            }
            resumeGames[0]["blocks"] = blocks;

            dynamic response        = lastResult as dynamic;
            dynamic virtualreels    = resumeGames[0]["virtualreels"];

            if (!object.ReferenceEquals(response["MountMazuma_wildList"], null) && response["MountMazuma_wildList"].Count > 0)
            {
                for (int i = 0; i < response["MountMazuma_wildList"].Count; i++)
                {
                    int reelindex   = response["MountMazuma_wildList"][i]["reelindex"];
                    int symbolindex = response["MountMazuma_wildList"][i]["symbolindex"];
                    response["reels"][reelindex][symbolindex]["symbolid"] = 1;
                    virtualreels[reelindex][symbolindex + 2] = 1;
                }
            }

            if (!object.ReferenceEquals(response["waymultiplierlist"], null) && response["waymultiplierlist"].Count > 0)
            {
                for (int i = 0; i < response["waymultiplierlist"].Count; i++)
                {
                    int reelindex   = response["waymultiplierlist"][i]["pos"]["reelindex"];
                    int symbolindex = response["waymultiplierlist"][i]["pos"]["symbolindex"];
                    int symbolid    = (int)response["reels"][reelindex][symbolindex]["symbolid"];
                    virtualreels[reelindex][symbolindex + 2] = (symbolid + 100);
                }
            }
            resumeGames[0]["virtualreels"] = virtualreels;
            return resumeGames;
        }
    }
}
