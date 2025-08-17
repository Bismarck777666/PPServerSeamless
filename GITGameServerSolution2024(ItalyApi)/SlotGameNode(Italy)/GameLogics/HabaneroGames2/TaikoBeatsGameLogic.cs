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
    public class TaikoBeatsGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTaikoBeats";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "0adbd26f-c0aa-4765-9b8b-cd0c2820ab25";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "6a329771cce9013901b63716ecff4fdb39784069";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.10375.417";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",    name = "Wild"       } },    //와일드
                    {2,   new HabaneroLogSymbolIDName{id = "idScatter", name = "Scatter"    } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idDrum",    name = "Drum"       } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idFan",     name = "Fan"        } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idKendama", name = "Kendama"    } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idDango",   name = "Dango"      } },    
                
                    {7,   new HabaneroLogSymbolIDName{id = "idA",       name = "A"          } },    
                    {8,   new HabaneroLogSymbolIDName{id = "idK",       name = "K"          } },    
                    {9,   new HabaneroLogSymbolIDName{id = "idQ",       name = "Q"          } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idJ",       name = "J"          } },

                    {11,  new HabaneroLogSymbolIDName{id = "idWildX2",  name = "WildX2"     } },    //와일드X2
                    {12,  new HabaneroLogSymbolIDName{id = "idWildX3",  name = "WildX3"     } },    //와일드X3
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 668;
            }
        }
        #endregion

        public TaikoBeatsGameLogic()
        {
            _gameID     = GAMEID.TaikoBeats;
            GameName    = "TaikoBeats";
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserID, int currentIndex, dynamic response, bool containWild = false)
        {
            dynamic reels =  base.buildHabaneroLogReels(strGlobalUserID, currentIndex, response as JObject, containWild);
            if(currentIndex > 0)
            {
                dynamic beforeResponse = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserID].Responses[currentIndex - 1].Response);
                if(!object.ReferenceEquals(beforeResponse["wildMultiplierData"],null) && beforeResponse["wildMultiplierData"].Count > 0)
                {
                    for(int i = 0; i < beforeResponse["wildMultiplierData"].Count; i++)
                    {
                        int reelindex       = (int)beforeResponse["wildMultiplierData"][i]["pos"]["reelindex"];
                        int symbolindex     = (int)beforeResponse["wildMultiplierData"][i]["pos"]["symbolindex"];
                        int nextMultiplier  = (int)beforeResponse["wildMultiplierData"][i]["nextMultiplier"];
                        if (nextMultiplier > 3)
                            nextMultiplier = 3;

                        reels[reelindex][symbolindex] = SymbolIdStringForLog[9 + nextMultiplier].id;
                    }
                }
            }

            return reels;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            dynamic resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            
            int currentIndex = _dicUserHistory[strGlobalUserID].Responses.Count - 1;
            if (currentIndex > 0)
            {
                dynamic beforeResponse = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserID].Responses[currentIndex - 1].Response);
                if (!object.ReferenceEquals(beforeResponse["wildMultiplierData"], null) && beforeResponse["wildMultiplierData"].Count > 0)
                {
                    for (int i = 0; i < beforeResponse["wildMultiplierData"].Count; i++)
                    {
                        int reelindex       = (int)beforeResponse["wildMultiplierData"][i]["pos"]["reelindex"];
                        int symbolindex     = (int)beforeResponse["wildMultiplierData"][i]["pos"]["symbolindex"];
                        int nextMultiplier  = (int)beforeResponse["wildMultiplierData"][i]["nextMultiplier"];
                        if (nextMultiplier > 3)
                            nextMultiplier = 3;

                        resumeGames[0]["virtualreels"][reelindex][symbolindex + 2] = 9 + nextMultiplier;
                    }
                }
            }
            
            if (!object.ReferenceEquals(lastResult["wildMultiplierData"], null))
                resumeGames[0]["resumeWilds"] = lastResult["wildMultiplierData"];
            else
                resumeGames[0]["resumeWilds"] = new JArray();
            
            return resumeGames;
        }
    }
}
