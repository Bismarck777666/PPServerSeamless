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
    public class RodeoDriveGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGRodeoDrive";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "f6497444-2290-4f8d-89eb-c77280330862";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "48ceb07f34a21dda62ec61bcffdd3c212dc5ec79";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.12539.874";
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
                return 25;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idGlamorGirl",      name = "GlamorGirl"     } },
                    {2,   new HabaneroLogSymbolIDName{id = "idHollywoodStar",   name = "HollywoodStar"  } },
                    {3,   new HabaneroLogSymbolIDName{id = "idShop",            name = "Shop"           } },
                    {4,   new HabaneroLogSymbolIDName{id = "idDoorman",         name = "Doorman"        } },
                    {5,   new HabaneroLogSymbolIDName{id = "idCar",             name = "Car"            } },
                    {6,   new HabaneroLogSymbolIDName{id = "idDog",             name = "Dog"            } },
                    {7,   new HabaneroLogSymbolIDName{id = "idShoppingBag",     name = "ShoppingBag"    } },
                    {8,   new HabaneroLogSymbolIDName{id = "idHandbag",         name = "Handbag"        } },
                    {9,   new HabaneroLogSymbolIDName{id = "idShoes",           name = "Shoes"          } },
                    {10,  new HabaneroLogSymbolIDName{id = "idPhone",           name = "Phone"          } },
                    {11,  new HabaneroLogSymbolIDName{id = "idBracelet",        name = "Bracelet"       } },
                    {12,  new HabaneroLogSymbolIDName{id = "idDrink",           name = "Drink"          } },
                    {13,  new HabaneroLogSymbolIDName{id = "idTicket",          name = "Ticket"         } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 204;
            }
        }
        #endregion

        public RodeoDriveGameLogic()
        {
            _gameID     = GAMEID.RodeoDrive;
            GameName    = "RodeoDrive";
        }

        protected override async Task onProcMessage(string strUserID, int agentID, CurrencyEnum currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_HABANERO_DOCLIENT)
            {
                await onDoSpin(strUserID, agentID, (int)currency, message, userBonus, userBalance, isMustLose);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_HABANERO_DOUPDATECLIENTPICK)
            {
                onDoUpdateClientPick(strUserID, agentID, (int)currency, message, userBalance);
            }
            else
            {
                await base.onProcMessage(strUserID, agentID, currency, message, userBonus, userBalance, isMustLose);
            }
        }

        protected void onDoUpdateClientPick(string strUserID, int agentID, int currency, GITMessage message, double userBalance)
        {
            try
            {
                string strSessionID = (string) message.Pop();
                string strGrid      = (string) message.Pop();
                string strToken     = (string) message.Pop();

                HabaneroResponse response = new HabaneroResponse();
                HabaneroResponseGame game = new HabaneroResponseGame();
                game.action         = "pick";
                game.brandgameid    = BrandGameId;
                game.gameid         = "00000000-0000-0000-0000-000000000000";
                game.sessionid      = strSessionID;
                game.friendlyid     = 0;

                HabaneroResponseHeader responseHeader = makeHabaneroResponseHeader(strUserID, currency, userBalance, strToken);

                response.game           = game;
                response.header         = responseHeader;
                response.grid           = strGrid;
                JObject portMessage     = new JObject();
                portMessage.Add("gssid" ,Guid.NewGuid().ToString());
                response.portmessage    = portMessage;

                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_HABANERO_DOUPDATECLIENTPICK);
                reponseMessage.Append(JsonConvert.SerializeObject(response));
                Sender.Tell(new ToUserMessage((int)_gameID, reponseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RodeoDriveGameLogic::onDoUpdateClientPick GameID: {0}, {1}", _gameID, ex);
            }
        }
        
        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            base.convertWinsByBet(resultContext as JObject, currentBet);

            if (!object.ReferenceEquals(resultContext["RodeoDrive_pickResults"], null))
            {
                for(int i = 0; i < (resultContext["RodeoDrive_pickResults"] as JArray).Count; i++)
                {
                    if(!object.ReferenceEquals(resultContext["RodeoDrive_pickResults"][i]["wincash"], null))
                        resultContext["RodeoDrive_pickResults"][i]["wincash"] = convertWinByBet((double)resultContext["RodeoDrive_pickResults"][i]["wincash"], currentBet);
                }
            }
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];

            if(responses.Action != HabaneroActionType.HOLLYWOODSTARPICK)
            {
                JObject eventItem       = base.buildEventItem(strUserId, currentIndex);
                dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(responses.Response);

                if (!object.ReferenceEquals(resultContext["freegamemultiplier"], null))
                    eventItem["multiplier"] = resultContext["freegamemultiplier"];

                return eventItem;
            }
            else
            {
                JObject eventItem       = base.buildEventItem(strUserId, currentIndex);
                HabaneroHistoryResponses triggerResponse = _dicUserHistory[strUserId].Responses[currentIndex - 1];
                dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(triggerResponse.Response);
                
                JArray subEvents = new JArray();
                if (!object.ReferenceEquals(resultContext["RodeoDrive_pickResults"], null))
                {
                    for(int i = 0; i < (resultContext["RodeoDrive_pickResults"] as JArray).Count; i++)
                    {
                        if (!object.ReferenceEquals(resultContext["RodeoDrive_pickResults"][i]["winfreegames"], null))
                        {
                            JObject subEventItem = new JObject();
                            subEventItem["type"]            = "pick";
                            subEventItem["picktype"]        = resultContext["RodeoDrive_pickResults"][i]["type"];
                            subEventItem["winfreegames"]    = resultContext["RodeoDrive_pickResults"][i]["winfreegames"];
                            subEvents.Add(subEventItem);
                        }

                        if (!object.ReferenceEquals(resultContext["RodeoDrive_pickResults"][i]["winmultiplier"], null))
                        {
                            JObject subEventItem = new JObject();
                            subEventItem["type"]            = "pick";
                            subEventItem["picktype"]        = resultContext["RodeoDrive_pickResults"][i]["type"];
                            subEventItem["winmultiplier"]   = resultContext["RodeoDrive_pickResults"][i]["winmultiplier"];
                            subEvents.Add(subEventItem);
                        }

                        if (!object.ReferenceEquals(resultContext["RodeoDrive_pickResults"][i]["wincash"], null))
                        {
                            JObject subEventItem = new JObject();
                            subEventItem["type"]                = "pick";
                            subEventItem["picktype"]            = resultContext["RodeoDrive_pickResults"][i]["type"];
                            subEventItem["wincash"]             = resultContext["RodeoDrive_pickResults"][i]["wincash"];
                            subEventItem["wincashmultiplier"]   = resultContext["RodeoDrive_pickResults"][i]["wincashmultiplier"];
                            subEvents.Add(subEventItem);
                        }
                    }
                }

                if (subEvents.Count > 0)
                    eventItem["subevents"] = subEvents;

                return eventItem;
            }
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            resumeGames[0]["gamemode"] = lastResult["nextgamestate"];

            if(lastResult["nextgamestate"].ToString() == "hollywoodstarpick")
            {
                JObject RodeoDrive_clientPickState  = new JObject();
                JArray  items                       = new JArray();
                for(int i = 0; i < lastResult["virtualreels"].Count(); i++)
                {
                    bool containScatter = false;
                    for(int j = 2; j < lastResult["virtualreels"][i].Count() - 2; j++)
                    {
                        if(lastResult["virtualreels"][i][j].ToString() == "2")
                        {
                            containScatter = true;
                            break;
                        }
                    }

                    if (containScatter)
                        items.Add(i);
                }
                RodeoDrive_clientPickState.Add("items", items);
                
                resumeGames[0]["RodeoDrive_clientPickState"]    = RodeoDrive_clientPickState;
                resumeGames[0]["RodeoDrive_pickResults"]        = lastResult["RodeoDrive_pickResults"];

                if (object.ReferenceEquals(lastResult["currentfreegame"], null))
                {
                    resumeGames[0]["numfreegames"] = 0;
                    resumeGames[0]["currfreegame"] = 0;
                }
            }

            return resumeGames;
        }
    }
}
