using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using Akka.Event;
using GITProtocol.Utils;
using UserNode.Database;
using System.Net;
using Akka.Routing;
using System.Threading;
using StackExchange.Redis;
using Akka.Cluster;
using Newtonsoft.Json;
using System.Net.Http;
using Pipelines.Sockets.Unofficial;
using System.Transactions;
using Newtonsoft.Json.Linq;
using PCGSharp;

namespace UserNode
{
    //로그인된 사용자를 표현하는 클라스 
    public class UserActor : ReceiveActor
    {
        #region 사용자정보
        private long                    _userDBID               = 0;
        private string                  _strUserID              = "";
        private string                  _strGlobalUserID        = "";
        private string                  _agentID                = "";
        private int                     _agentDBID              = 0;
        private double                  _balance                = 0.0;
        private long                    _lastScoreCounter       = 0;
        private Currencies              _currency               = Currencies.THB;
        private bool                    _isAffiliate            = false;
        #endregion

        #region 유저의 상태변수들       
        private bool                                _userDisconnected   = false;
        private Dictionary<string, UserConnection>  _userConnections    = new Dictionary<string, UserConnection>();
        #endregion

        #region 사용자의 각종 보너스정보
        private UserRangeOddEventItem   _userRangeOddEventItem;
        private List<UserBonus>         _waitingUserBonuses     = new List<UserBonus>();
        #endregion

        private IActorRef                       _dbReader       = null;
        private IActorRef                       _dbWriter       = null;
        private readonly ILoggingAdapter        _logger         = Logging.GetLogger(Context);
        protected static RealExtensions.Epsilon _epsilion       = new RealExtensions.Epsilon(0.001);

        private ICancelable                     _conCheckCancel       = null;
        private IActorRef                       _afterQuitNotifyActor = null;
        private HttpClient                      _httpClient           = new HttpClient();

        public UserActor(CreateNewUserMessage message, IActorRef dbReader, IActorRef dbWriter)
        {
            _dbReader           = dbReader;
            _dbWriter           = dbWriter;
            _agentDBID          = message.AgentDBID;
            _agentID            = message.AgentID;
            _userDBID           = message.UserDBID;
            _strUserID          = message.UserID;
            _lastScoreCounter   = message.LastScoreCounter;
            _balance            = message.UserBalance;
            _currency           = message.Currency;
            _isAffiliate        = message.IsAffiliate;

            _strGlobalUserID    = string.Format("{0}_{1}", _agentDBID, _strUserID);
            _userConnections.Add(message.SessionToken, new UserConnection(message.SessionToken));

            Receive<UserLoggedIn>                       (onUserLoginSucceeded);
            ReceiveAsync<FromConnRevMessage>            (onProcMessage);
            Receive<HttpSessionAdded>                   (onProcHttpSessionAdded);
            ReceiveAsync<HttpSessionClosed>             (onProcHttpSessionClosed);
            ReceiveAsync<CloseHttpSession>              (onCloseHttpSession);
            Receive<QuitUserMessage>                    (onForceLogoutMessage);
            Receive<QuitAndNotifyMessage>               (onForceQuitAndNotifyMessage);
            Receive<string>                             (onCommand);            
            ReceiveAsync<SlotGamesNodeShuttingdown>     (onSlotGameServerShuttingdown);
            Receive<UserRangeOddEventItem>              (onUserRangeOddEvent);
            Receive<UserEventCancelled>                 (onUserEventCancelled);
            Receive<UserVerifyRequest>                  (onVerifyRequest);
        }

        public static Props Props(CreateNewUserMessage message, IActorRef dbReader, IActorRef dbWriter)
        {
            return Akka.Actor.Props.Create(() => new UserActor(message, dbReader, dbWriter));
        }
        protected override void PreStart()
        {
            Self.Tell(new UserLoggedIn());                                                  
            base.PreStart();
        }
        protected override void PostStop()
        {
            if (_conCheckCancel != null)
                _conCheckCancel.Cancel();
            base.PostStop();
        }
        private void onUserLoginSucceeded(UserLoggedIn message)
        {
            try
            {
                //유저온라인상태 갱신
                _dbWriter.Tell(new UserLoginStateItem(_userDBID));

                _logger.Info("{0} has been logged in successfully", _strGlobalUserID);
                _conCheckCancel = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 1000, Self, "checkConn", Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::onUserLoginSucceeded {0}", ex);
            }
        }
        private void onVerifyRequest(UserVerifyRequest request)
        {
            Sender.Tell(new HTTVerifyResponse(request.Token, request.GameString, request.GameID, _strUserID, _strUserID, _currency.ToString()));
        }
        private void addScore(long scoreID, double score)
        {
            if (scoreID <= _lastScoreCounter)
                return;

            _lastScoreCounter = scoreID;
            _balance         += score;
        }
        #region 각종 사건처리부
        private void onUserEventCancelled(UserEventCancelled message)
        {
            if (_userRangeOddEventItem != null && _userRangeOddEventItem.BonusID == message.BonusID)
            {
                int i = 0;
                for (i = 0; i < _waitingUserBonuses.Count; i++)
                {
                    if (_waitingUserBonuses[i].BonusType == UserBonusType.USEREVENT && _waitingUserBonuses[i].BonusID == message.BonusID)
                        break;
                }
                if (i < _waitingUserBonuses.Count)
                    _waitingUserBonuses.RemoveAt(i);

                _dbReader.Tell(new ClaimedUserRangeEventMessage(_userRangeOddEventItem.BonusID, _agentDBID, _strUserID, 0.0, ""));
                _userRangeOddEventItem = null;
                return;
            }
        }
        private void onUserRangeOddEvent(UserRangeOddEventItem userRangeEventItem)
        {
            if (_userRangeOddEventItem != null)
                return;

            _userRangeOddEventItem = userRangeEventItem;
            _waitingUserBonuses.Add(new UserRangeOddEventBonus(userRangeEventItem.BonusID, userRangeEventItem.MinOdd, userRangeEventItem.MaxOdd, userRangeEventItem.MaxBet));
        }
        private void fetchNewUserRangeEvent(double rewardedMoney, int gameID)
        {
            _dbWriter.Tell(new ClaimedUserRangeEventItem(_userRangeOddEventItem.BonusID, rewardedMoney, gameID.ToString(), DateTime.Now));
            _dbReader.Tell(new ClaimedUserRangeEventMessage(_userRangeOddEventItem.BonusID, _agentDBID, _strUserID, rewardedMoney, gameID.ToString()));
            _userRangeOddEventItem = null;
        }
        private UserBonus pickUserBonus(int gameID)
        {
            for (int i = 0; i < _waitingUserBonuses.Count; i++)
            {
                if (_waitingUserBonuses[i] is UserRangeOddEventBonus)
                    return _waitingUserBonuses[i];
            }
            return null;
        }
        
        private void onForceQuitAndNotifyMessage(QuitAndNotifyMessage _)
        {
            _afterQuitNotifyActor = Sender;
            onForceLogoutMessage(new QuitUserMessage(_agentDBID, _strUserID));
        }
        private void onForceLogoutMessage(QuitUserMessage _)
        {
            //모든 연결들에 통지한다.
            for (int i = 0; i < _userConnections.Count; i++)
            {
                string strToken = _userConnections.Keys.ElementAt(i);
                Self.Tell(new CloseHttpSession(strToken));
            }
            _logger.Info("User {0} has been kicked by admin", _strGlobalUserID);
        }
        private void onCommand(string strCommand)
        {
            if(strCommand == "checkConn")
            {
                foreach(KeyValuePair<string, UserConnection> pair in _userConnections)
                {
                    if (DateTime.Now.Subtract(pair.Value.LastActiveTime) >= TimeSpan.FromMinutes(5))
                        Self.Tell(new CloseHttpSession(pair.Key as string));
                }
            }
        }        
        private void onProcHttpSessionAdded(HttpSessionAdded message)
        {
            if (_userConnections.ContainsKey(message.SessionToken))
            {
                if (message.SessionToken == "PrevWS")
                    _userConnections[message.SessionToken].LastActiveTime = DateTime.Now;
                return;
            }
            _userConnections.Add(message.SessionToken, new UserConnection(message.SessionToken));
        }
        private async Task onCloseHttpSession(CloseHttpSession message)
        {
            try
            {
                //레디스에서 해당 세션토큰을 삭제한다.
                await RedisDatabase.RedisCache.HashDeleteAsync(string.Format("{0}_tokens", _strGlobalUserID), message.SessionToken);
                if (!_userConnections.ContainsKey(message.SessionToken))
                    return;

                UserConnection userConn = _userConnections[message.SessionToken];
                await onClosedUserConnection(userConn);

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::onCloseHttpSession {0}", ex);
            }
        }
        private async Task onProcHttpSessionClosed(HttpSessionClosed message)
        {
            try
            {
                if (!_userConnections.ContainsKey(message.SessionToken))
                    return;

                UserConnection userConn = _userConnections[message.SessionToken];
                await onClosedUserConnection(userConn);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::onProcHttpSessionClosed {0}", ex);
            }
        }
        private async Task onClosedUserConnection(UserConnection userConn)
        {
            int lastGameID = 0;
            if (userConn.GameActor != null)
            {
                ExitGameResponse response = null;
                try
                {
                    response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _agentDBID, _balance, true, false), Constants.RemoteTimeOut);
                }
                catch (Exception ex)
                {
                    _logger.Warning("{0} exit game {1} Failed : Exception {2}", _strGlobalUserID, userConn.GameID, ex);
                }
                lastGameID = userConn.GameID;
            }
            _userConnections.Remove(userConn.Token);
            if (_userConnections.Count == 0)
            {
                try
                {
                    //밸런스변화가 있다면
                    double balanceUpdate = await _dbWriter.Ask<double>(new FetchUserBalanceUpdate(_userDBID));
                    await _dbReader.Ask(new UserOfflineStateItem(_userDBID, lastGameID, balanceUpdate));
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in UserActor::onProcSocketConnectionClosed User ID: {0} {1}", _strGlobalUserID, ex);
                }

                try
                {
                    await RedisDatabase.RedisCache.HashDeleteAsync("onlineusers", new RedisValue[] { _strGlobalUserID, _strGlobalUserID + "_path" });
                    await RedisDatabase.RedisCache.KeyDeleteAsync(_strGlobalUserID + "_tokens");

                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in UserActor::onClosedUserConnection User ID: {0} {1}", _strGlobalUserID, ex);
                }

                _userDisconnected = true;
                _logger.Info("{0} user has been logged out", _strGlobalUserID);
                Context.Stop(Self);

                if (_afterQuitNotifyActor != null)
                {
                    _afterQuitNotifyActor.Tell(_balance);
                    _afterQuitNotifyActor = null;
                }
            }
        }
        #endregion

        #region 메세지처리함수들
        private async Task onProcMessage(FromConnRevMessage fromConnRevMsg)
        {
            //이미 로그아웃된 유저에 한해서 모든 메세지처리를 무시한다.
            if (_userDisconnected)
                return;

            if (!_userConnections.ContainsKey(fromConnRevMsg.SessionToken))
                return;

            UserConnection userConn = _userConnections[fromConnRevMsg.SessionToken];
            GITMessage message      = fromConnRevMsg.Message;
            if (message.MsgCode == MsgCodes.HEARTBEAT)
            {
                userConn.LastActiveTime = DateTime.Now;
            }
            else if (message.MsgCode == MsgCodes.ENTERGAME)
            {
                userConn.LastActiveTime = DateTime.Now;
                await procEnterGame(message, userConn);
            }
            else if (message.MsgCode == MsgCodes.GETGAMENAME)
            {
                userConn.LastActiveTime = DateTime.Now;
                procGetGameName(message, userConn);
            }
            else if (message.MsgCode == MsgCodes.GETGAMEWALLET)
            {
                userConn.LastActiveTime = DateTime.Now;
                procGetGameWallet(message, userConn);
            }
            else if (message.MsgCode == MsgCodes.GETGAMERULE)
            {
                userConn.LastActiveTime = DateTime.Now;
                procGetGameRule(message, userConn);
            }
            else if (message.MsgCode == MsgCodes.GETGAMERESOURCE)
            {
                userConn.LastActiveTime = DateTime.Now;
                procGetGameResource(message, userConn);
            }
            else if (message.MsgCode == MsgCodes.SPIN || message.MsgCode == MsgCodes.SELECTCHARACTER ||
                message.MsgCode == MsgCodes.UPDATEGAMEINFO)
            {
                userConn.LastActiveTime = DateTime.Now;
                await procSlotGameMsg(message, userConn);
            }
            else if (message.MsgCode == MsgCodes.GETHISTORYSUMMARY)
            {
                userConn.LastActiveTime = DateTime.Now;
                await procGetBetHistorySummary(message, userConn);
            }
            else if (message.MsgCode == MsgCodes.GETHISTORYITEMS)
            {
                userConn.LastActiveTime = DateTime.Now;
                await procGetBetHistoryItems(message, userConn);
            }
            else if (message.MsgCode == MsgCodes.GETHISTORYDETAIL)
            {
                userConn.LastActiveTime = DateTime.Now;
                await procGetBetHistoryDetail(message, userConn);
            }
            else
            {
                _logger.Warning("Unknown packet received from {0} Message code:{1} User ID:{2}", message.MsgCode, _strUserID);
            }
        }
        private async Task exitGameFromUserConn(UserConnection userConn)
        {
            if (userConn.GameActor == null)
                return;
            try
            {
                ExitGameResponse response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _agentDBID, _balance, true, true), Constants.RemoteTimeOut);
            }
            catch (Exception ex)
            {
                _logger.Warning("{0} exit game {1} Failed : Exception {2}", _strUserID, userConn.GameID, ex);
            }
        }
        private async Task procEnterGame(GITMessage message, UserConnection userConn)
        {
            int gameID = (int) message.Pop();
            if (userConn.GameActor != null)
            {
                await exitGameFromUserConn(userConn);
                userConn.resetGame();
            }

            UserConnection oldConn = null;
            foreach (KeyValuePair<string, UserConnection> pair in _userConnections)
            {
                if (pair.Value.GameActor != null && pair.Value.GameID == gameID)
                {
                    oldConn = pair.Value;
                    break;
                }
            }
            if (oldConn != null)
            {
                oldConn.resetGame();
                await onCloseHttpSession(new CloseHttpSession(oldConn.Token));
            }

            bool                    enterGameSucceeded  = false;
            HTTPEnterGameResponse   enterResponse       = null;
            do
            {
                //유저의 보유머니를 API로 부터 불러온다.
                var balanceResponse = await callGetBalance(gameID);
                if (balanceResponse == null || balanceResponse.code != 0 || balanceResponse.balance < 0.0M)
                {
                    enterGameSucceeded = false;
                    break;
                }
                _balance = Math.Round((double) balanceResponse.balance, 2);
                _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, _balance));                                                            //유저머니갱신

                try
                {
                    EnterGameRequest  requestMsg  = new EnterGameRequest(gameID, _agentDBID, _strUserID, Self, _balance, _currency);
                    EnterGameResponse responseMsg = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<EnterGameResponse>(requestMsg, Constants.RemoteTimeOut);
                    if (responseMsg.Ack == 0)
                    {
                        userConn.GameActor      = responseMsg.GameActor;
                        userConn.GameID         = gameID;
                        enterGameSucceeded      = true;
                        _dbWriter.Tell(new UserGameStateItem(_userDBID, 2, (int)gameID));

                        enterResponse = new HTTPEnterGameResponse(JsonConvert.DeserializeObject<PGGameConfig>(responseMsg.GameConfig),
                            responseMsg.LastResult, _balance, _currency);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning("{0} enter slot game {1} Failed : Exception {2}", _strGlobalUserID, gameID, ex);
                }
            } while (false);
            
            if(enterGameSucceeded)
            {
                foreach (KeyValuePair<string, UserConnection> pair in _userConnections)
                {
                    if (pair.Key == userConn.Token)
                        continue;

                    UserConnection conn = pair.Value;
                    Self.Tell(new CloseHttpSession(conn.Token));
                }
            }
            if (enterResponse != null)
                Sender.Tell(enterResponse);
            else
                Sender.Tell(new HTTPEnterGameResponse(HTTPEnterGameResults.INVALIDGAMEID));
        }

        private void procGetGameName(GITMessage message, UserConnection userConn)
        {
            string strGameNameData = "{\"0\":\"Lobby\",\"1\":\"Honey Trap of Diao Chan\",\"2\":\"Gem Saviour\",\"3\":\"Fortune Gods\",\"6\":\"Medusa 2: the Quest of Perseus\",\"7\":\"Medusa 1: the Curse of Athena\",\"17\":\"Wizdom Wonders\",\"18\":\"Hood vs Wolf\",\"20\":\"Reel Love\",\"24\":\"Win Win Won\",\"25\":\"Plushie Frenzy\",\"26\":\"Tree of Fortune\",\"28\":\"Hotpot\",\"29\":\"Dragon Legend\",\"31\":\"Baccarat Deluxe\",\"33\":\"Hip Hop Panda\",\"34\":\"Legend of Hou Yi\",\"35\":\"Mr. Hallow-Win!\",\"36\":\"Prosperity Lion\",\"37\":\"Santa\'s Gift Rush\",\"38\":\"Gem Saviour Sword\",\"39\":\"Piggy Gold\",\"40\":\"Jungle Delight\",\"41\":\"Symbols Of Egypt\",\"42\":\"Ganesha Gold\",\"43\":\"Three Monkeys\",\"44\":\"Emperor\'s Favour\",\"48\":\"Double Fortune\",\"50\":\"Journey to the Wealth\",\"53\":\"The Great Icescape\",\"54\":\"Captain\'s Bounty\",\"57\":\"Dragon Hatch\",\"59\":\"Ninja vs Samurai\",\"60\":\"Leprechaun Riches\",\"61\":\"Flirting Scholar\",\"62\":\"Gem Saviour Conquest\",\"63\":\"Dragon Tiger Luck\",\"64\":\"Muay Thai Champion\",\"65\":\"Mahjong Ways\",\"67\":\"Shaolin Soccer\",\"68\":\"Fortune Mouse\",\"69\":\"Bikini Paradise\",\"70\":\"Candy Burst\",\"71\":\"CaiShen Wins\",\"73\":\"Egypt\'s Book of Mystery\",\"74\":\"Mahjong Ways 2\",\"75\":\"Ganesha Fortune\",\"79\":\"Dreams of Macau\",\"80\":\"Circus Delight\",\"82\":\"Phoenix Rises\",\"83\":\"Wild Fireworks\",\"84\":\"Queen of Bounty\",\"85\":\"Genie\'s 3 Wishes\",\"86\":\"Galactic Gems\",\"87\":\"Treasures of Aztec\",\"88\":\"Jewels of Prosperity\",\"89\":\"Lucky Neko\",\"90\":\"Secrets of Cleopatra\",\"91\":\"Guardians of Ice & Fire\",\"92\":\"Thai River Wonders\",\"93\":\"Opera Dynasty\",\"94\":\"Bali Vacation\",\"95\":\"Majestic Treasures\",\"97\":\"Jack Frost\'s Winter\",\"98\":\"Fortune Ox\",\"100\":\"Candy Bonanza\",\"101\":\"Rise of Apollo\",\"102\":\"Mermaid Riches\",\"103\":\"Crypto Gold\",\"104\":\"Wild Bandito\",\"105\":\"Heist Stakes\",\"106\":\"Ways of the Qilin\",\"107\":\"Legendary Monkey King\",\"108\":\"Buffalo Win\",\"109\":\"Sushi Oishi\",\"110\":\"Jurassic Kingdom\",\"111\":\"Groundhog Harvest\",\"112\":\"Oriental Prosperity\",\"113\":\"Raider Jane\'s Crypt of Fortune\",\"114\":\"Emoji Riches\",\"115\":\"Supermarket Spree\",\"116\":\"Farm Invaders\",\"117\":\"Cocktail Nights\",\"118\":\"Mask Carnival\",\"119\":\"Spirited Wonders\",\"120\":\"The Queen\'s Banquet\",\"121\":\"Destiny of Sun & Moon\",\"122\":\"Garuda Gems\",\"123\":\"Rooster Rumble\",\"124\":\"Battleground Royale\",\"125\":\"Butterfly Blossom\",\"126\":\"Fortune Tiger\",\"127\":\"Speed Winner\",\"128\":\"Legend of Perseus\",\"129\":\"Win Win Fish Prawn Crab\",\"130\":\"Lucky Piggy\",\"132\":\"Wild Coaster\",\"135\":\"Wild Bounty Showdown\",\"1312883\":\"Prosperity Fortune Tree\",\"1338274\":\"Totem Wonders\",\"1368367\":\"Alchemy Gold\"}";
            Dictionary<string, string> dt = JsonConvert.DeserializeObject<Dictionary<string, string>>(strGameNameData);

            Sender.Tell(new GetGameNameResponse(dt));
        }
        private void procGetGameWallet(GITMessage message, UserConnection userConn)
        {
            string strTemplate = "{\"cc\":\"USD\",\"tb\":0.42,\"pb\":0.00,\"cb\":0.42,\"tbb\":0.00,\"tfgb\":0.00,\"rfgc\":0,\"inbe\":false,\"infge\":false,\"iebe\":false,\"iefge\":false,\"ch\":{\"k\":\"0_C\",\"cid\":0,\"cb\":0.42},\"p\":null,\"ocr\":null}";
            JToken dt       = JToken.Parse(strTemplate);
            dt["cb"]        = Math.Round(_balance, 2);
            dt["cc"]        = _currency.ToString();
            dt["ch"]["cb"]  = Math.Round(_balance, 2);
            dt["tb"]        = Math.Round(_balance, 2);
            Sender.Tell(new GetGameWalletResponseData(JsonConvert.SerializeObject(dt)));
        }
        private async void procGetGameRule(GITMessage message, UserConnection userConn)
        {
            try
            {
                int     gameId  = (int)message.Pop();
                string  traceId = (string)message.Pop();

                if (userConn.GameActor == null)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new GetGameRuleResponse(1302, "Invalid player session", traceId)));
                    return;
                }
                string strGameRule  = await userConn.GameActor.Ask<string>(new FromUserMessage(_strUserID, _agentDBID, _balance, Self, message, null, _currency, _isAffiliate), TimeSpan.FromSeconds(5.0));
                dynamic dt          = JsonConvert.DeserializeObject<dynamic>(strGameRule);

                Sender.Tell(JsonConvert.SerializeObject(new GetGameRuleResponse(dt)));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::procGetGameRule {0}", ex);
            }
        }

        private void procGetGameResource(GITMessage message, UserConnection userConn)
        {
            string strGameResource = "[{\"rid\":0,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/0/default_icon.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:24\"},{\"rid\":1,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/1/HoneyTrap_of_DiaoChan_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:24\"},{\"rid\":2,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/2/GemSaviour_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:24\"},{\"rid\":3,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/3/FortuneGods_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:24\"},{\"rid\":6,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/6/Medusa2_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:24\"},{\"rid\":7,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/7/Medusa1_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:24\"},{\"rid\":17,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/17/WizdomWonders_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:25\"},{\"rid\":18,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/18/HoodWolf_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:25\"},{\"rid\":24,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/24/WinWinWon_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:25\"},{\"rid\":25,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/25/PlushieFrenzy_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:25\"},{\"rid\":26,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/26/TreeofFortune_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:26\"},{\"rid\":28,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/28/Hotpot_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:26\"},{\"rid\":29,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/29/DragonLegend_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:26\"},{\"rid\":31,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/31/BaccaratDeluxe_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:26\"},{\"rid\":33,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/33/HipHopPanda_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:26\"},{\"rid\":34,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/34/LegendofHouYi_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:26\"},{\"rid\":35,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/35/Mr.Hallow_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:26\"},{\"rid\":36,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/36/ProsperityLion_168x168_.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:26\"},{\"rid\":37,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/37/SantasGiftRush_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:26\"},{\"rid\":38,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/38/GemSaviourSword_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:26\"},{\"rid\":39,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/39/PiggyGold_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:27\"},{\"rid\":40,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/40/JungleDelight_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:27\"},{\"rid\":41,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/41/SymbolsofEgypt_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:27\"},{\"rid\":42,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/42/GaneshaGold_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:27\"},{\"rid\":43,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/43/ThreeMonkeys_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:27\"},{\"rid\":44,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/44/EmperorsFavour_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:27\"},{\"rid\":48,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/48/DoubleFortune_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:27\"},{\"rid\":50,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/50/JourneytotheWealth_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:28\"},{\"rid\":53,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/53/TheGreatIcescape_168x168_.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:28\"},{\"rid\":54,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/54/CaptainsBounty_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:28\"},{\"rid\":59,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/59/NinjavsSamurai_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:28\"},{\"rid\":60,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/60/LeprechaunRiches_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:28\"},{\"rid\":61,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/61/FlirtingScholar_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T02:33:28\"},{\"rid\":0,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/0/default_icon.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:28\"},{\"rid\":1,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/1/HoneyTrap_of_DiaoChan_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:29\"},{\"rid\":2,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/2/GemSaviour_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:29\"},{\"rid\":3,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/3/FortuneGods_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:29\"},{\"rid\":6,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/6/Medusa2_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:29\"},{\"rid\":7,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/7/Medusa1_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:29\"},{\"rid\":17,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/17/WizdomWonders_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:30\"},{\"rid\":18,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/18/HoodWolf_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:30\"},{\"rid\":24,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/24/WinWinWon_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:30\"},{\"rid\":25,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/25/PlushieFrenzy_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:30\"},{\"rid\":26,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/26/TreeofFortune_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:30\"},{\"rid\":28,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/28/Hotpot_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:30\"},{\"rid\":29,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/29/DragonLegend_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:31\"},{\"rid\":31,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/31/BaccaratDeluxe_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:31\"},{\"rid\":33,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/33/HipHopPanda_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:31\"},{\"rid\":34,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/34/LegendofHouYi_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:31\"},{\"rid\":35,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/35/Mr.Hallow_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:31\"},{\"rid\":36,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/36/ProsperityLion_168x168_.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:31\"},{\"rid\":37,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/37/SantasGiftRush_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:31\"},{\"rid\":38,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/38/GemSaviourSword_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:31\"},{\"rid\":39,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/39/PiggyGold_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:31\"},{\"rid\":40,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/40/JungleDelight_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:32\"},{\"rid\":41,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/41/SymbolsofEgypt_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:32\"},{\"rid\":42,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/42/GaneshaGold_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:32\"},{\"rid\":43,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/43/ThreeMonkeys_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:32\"},{\"rid\":44,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/44/EmperorsFavour_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:32\"},{\"rid\":48,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/48/DoubleFortune_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:33\"},{\"rid\":50,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/50/JourneytotheWealth_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:33\"},{\"rid\":53,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/53/TheGreatIcescape_168x168_.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:33\"},{\"rid\":54,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/54/CaptainsBounty_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:33\"},{\"rid\":59,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/59/NinjavsSamurai_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:34\"},{\"rid\":61,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/61/FlirtingScholar_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:34\"},{\"rid\":60,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/60/LeprechaunRiches_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T02:33:34\"},{\"rid\":62,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/62/GemSaviourConquest_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-01T10:09:32\"},{\"rid\":62,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/62/GemSaviourConquest_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-01T10:09:32\"},{\"rid\":64,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/64/MuayThai_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-10-03T08:17:00\"},{\"rid\":64,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/64/MuayThai_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-10-03T08:17:00\"},{\"rid\":63,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/63/DragonTigerLuck_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-11-01T11:44:06\"},{\"rid\":63,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/63/DragonTigerLuck_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-11-01T11:44:06\"},{\"rid\":65,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/65/MahjongWays_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-11-15T08:47:33\"},{\"rid\":65,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/65/MahjongWays_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-11-15T08:47:34\"},{\"rid\":20,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/20/ReelLove_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-11-22T10:16:08\"},{\"rid\":20,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/20/ReelLove_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-11-22T10:16:08\"},{\"rid\":57,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/57/DragonHatch_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2019-12-20T07:03:49\"},{\"rid\":57,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/57/DragonHatch_168x168.png\",\"l\":\"en-US\",\"ut\":\"2019-12-20T07:03:50\"},{\"rid\":68,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/68/FortuneMouse_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2020-01-13T04:07:28\"},{\"rid\":68,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/68/FortuneMouse_168x168.png\",\"l\":\"en-US\",\"ut\":\"2020-01-13T04:07:28\"},{\"rid\":67,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/67/ShaolinSoccer_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2020-02-19T09:00:03\"},{\"rid\":67,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/67/ShaolinSoccer_168x168.png\",\"l\":\"en-US\",\"ut\":\"2020-02-19T09:00:03\"},{\"rid\":71,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/71/caishen-wins_168_168.png\",\"l\":\"zh-CN\",\"ut\":\"2020-02-28T07:18:49\"},{\"rid\":71,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/71/caishen-wins_168_168.png\",\"l\":\"en-US\",\"ut\":\"2020-02-28T07:18:49\"},{\"rid\":70,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/70/Candy Burst 168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2020-03-04T04:37:28\"},{\"rid\":70,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/70/Candy Burst 168x168.png\",\"l\":\"en-US\",\"ut\":\"2020-03-04T04:37:28\"},{\"rid\":69,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/69/BikiniParadise_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2020-03-24T04:56:06\"},{\"rid\":69,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/69/BikiniParadise_168x168.png\",\"l\":\"en-US\",\"ut\":\"2020-03-24T04:56:06\"},{\"rid\":74,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/74/MahjongWaysTwo_168x168.png\",\"l\":\"zh-CN\",\"ut\":\"2020-04-06T03:48:21\"},{\"rid\":74,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/74/MahjongWaysTwo_168x168.png\",\"l\":\"en-US\",\"ut\":\"2020-04-06T03:48:22\"},{\"rid\":73,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/73/EgyptsBook_168_168.png\",\"l\":\"zh-CN\",\"ut\":\"2020-04-10T02:11:30\"},{\"rid\":73,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/73/EgyptsBook_168_168.png\",\"l\":\"en-US\",\"ut\":\"2020-04-10T02:11:30\"},{\"rid\":75,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/75/GaneshaFortune_168_168.png\",\"l\":\"zh-CN\",\"ut\":\"2020-04-17T06:02:33\"},{\"rid\":75,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/75/GaneshaFortune_168_168.png\",\"l\":\"en-US\",\"ut\":\"2020-04-17T06:02:33\"},{\"rid\":82,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/82/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-05-29T02:54:27\"},{\"rid\":82,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/82/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-05-29T02:54:27\"},{\"rid\":79,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/79/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-06-09T02:09:08\"},{\"rid\":79,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/79/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-06-09T02:09:08\"},{\"rid\":83,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/83/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-06-18T02:29:58\"},{\"rid\":83,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/83/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-06-18T02:29:58\"},{\"rid\":85,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/85/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-07-13T02:32:00\"},{\"rid\":85,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/85/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-07-13T02:32:00\"},{\"rid\":80,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/80/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-07-20T02:47:24\"},{\"rid\":80,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/80/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-07-20T02:47:24\"},{\"rid\":84,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/84/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-07-24T02:30:28\"},{\"rid\":84,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/84/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-07-24T02:30:28\"},{\"rid\":86,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/86/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-08-03T02:31:14\"},{\"rid\":86,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/86/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-08-03T02:31:14\"},{\"rid\":87,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/87/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-08-13T03:07:53\"},{\"rid\":87,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/87/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-08-13T03:07:53\"},{\"rid\":92,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/92/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-09-03T02:51:34\"},{\"rid\":92,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/92/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-09-03T02:51:34\"},{\"rid\":90,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/90/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-09-10T06:06:28\"},{\"rid\":90,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/90/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-09-10T06:06:28\"},{\"rid\":88,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/88/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-09-25T02:22:39\"},{\"rid\":88,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/88/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-09-25T02:22:39\"},{\"rid\":93,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/93/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-10-09T08:33:37\"},{\"rid\":93,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/93/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-10-09T08:33:37\"},{\"rid\":89,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/89/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-10-16T07:03:31\"},{\"rid\":89,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/89/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-10-16T07:03:31\"},{\"rid\":94,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/94/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-10-23T07:39:02\"},{\"rid\":94,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/94/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-10-23T07:39:02\"},{\"rid\":91,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/91/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-11-25T08:28:15\"},{\"rid\":91,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/91/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-11-25T08:28:15\"},{\"rid\":97,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/97/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-11-25T08:59:25\"},{\"rid\":97,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/97/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-11-25T08:59:25\"},{\"rid\":100,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/100/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-12-29T10:04:37\"},{\"rid\":100,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/100/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-12-29T10:04:37\"},{\"rid\":103,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/103/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-12-30T06:52:21\"},{\"rid\":103,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/103/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-12-30T06:52:21\"},{\"rid\":98,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/98/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2020-12-30T07:26:30\"},{\"rid\":98,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/98/SGS.png\",\"l\":\"en-US\",\"ut\":\"2020-12-30T07:26:30\"},{\"rid\":101,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/101/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-01-28T08:29:13\"},{\"rid\":101,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/101/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-01-28T08:29:13\"},{\"rid\":95,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/95/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-01-29T02:32:21\"},{\"rid\":95,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/95/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-01-29T02:32:21\"},{\"rid\":104,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/104/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-03-01T04:26:41\"},{\"rid\":104,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/104/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-03-01T04:26:41\"},{\"rid\":105,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/105/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-03-05T03:27:52\"},{\"rid\":105,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/105/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-03-05T03:27:52\"},{\"rid\":106,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/106/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-03-17T09:27:39\"},{\"rid\":106,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/106/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-03-17T09:27:39\"},{\"rid\":102,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/102/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-03-23T04:37:45\"},{\"rid\":102,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/102/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-03-23T04:37:45\"},{\"rid\":109,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/109/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-04-28T04:08:03\"},{\"rid\":109,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/109/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-04-28T04:08:03\"},{\"rid\":110,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/110/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-05-11T06:33:20\"},{\"rid\":110,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/110/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-05-11T06:33:20\"},{\"rid\":111,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/111/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-06-02T03:35:20\"},{\"rid\":111,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/111/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-06-02T03:35:20\"},{\"rid\":108,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/108/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-06-11T07:24:52\"},{\"rid\":108,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/108/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-06-11T07:24:52\"},{\"rid\":113,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/113/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-06-22T07:32:00\"},{\"rid\":113,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/113/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-06-22T07:32:00\"},{\"rid\":115,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/115/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-07-22T06:04:54\"},{\"rid\":115,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/115/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-07-22T06:04:54\"},{\"rid\":116,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/116/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-08-11T07:56:06\"},{\"rid\":116,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/116/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-08-11T07:56:06\"},{\"rid\":114,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/114/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-08-13T06:15:46\"},{\"rid\":114,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/114/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-08-13T06:15:46\"},{\"rid\":107,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/107/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-08-13T06:55:39\"},{\"rid\":107,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/107/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-08-13T06:55:39\"},{\"rid\":117,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/117/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-09-09T14:01:35\"},{\"rid\":117,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/117/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-09-09T14:01:35\"},{\"rid\":119,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/119/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-09-22T06:43:35\"},{\"rid\":119,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/119/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-09-22T06:43:35\"},{\"rid\":112,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/112/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-09-27T02:20:57\"},{\"rid\":112,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/112/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-09-27T02:20:57\"},{\"rid\":118,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/118/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-10-13T07:23:51\"},{\"rid\":118,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/118/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-10-13T07:23:51\"},{\"rid\":121,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/121/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-12-01T02:14:23\"},{\"rid\":121,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/121/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-12-01T02:14:23\"},{\"rid\":122,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/122/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-12-01T06:15:16\"},{\"rid\":122,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/122/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-12-01T06:15:16\"},{\"rid\":126,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/126/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2021-12-02T07:59:06\"},{\"rid\":126,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/126/SGS.png\",\"l\":\"en-US\",\"ut\":\"2021-12-02T07:59:06\"},{\"rid\":124,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/124/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-01-14T07:30:30\"},{\"rid\":124,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/124/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-01-14T07:30:30\"},{\"rid\":130,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/lucky-piggy/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-02-24T10:04:55\"},{\"rid\":130,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/lucky-piggy/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-02-24T10:04:55\"},{\"rid\":125,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/125/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-03-04T02:45:56\"},{\"rid\":125,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/125/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-03-04T02:45:56\"},{\"rid\":129,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/win-win-fpc/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-03-11T07:22:49\"},{\"rid\":129,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/win-win-fpc/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-03-11T07:22:49\"},{\"rid\":123,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/123/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-03-22T07:59:39\"},{\"rid\":123,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/123/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-03-22T07:59:39\"},{\"rid\":128,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/legend-perseus/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-03-23T03:16:08\"},{\"rid\":128,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/legend-perseus/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-03-23T03:16:08\"},{\"rid\":120,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/120/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-03-23T06:25:15\"},{\"rid\":120,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/120/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-03-23T06:25:15\"},{\"rid\":127,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/speed-winner/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-05-31T09:21:44\"},{\"rid\":127,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/speed-winner/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-05-31T09:21:44\"},{\"rid\":135,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/wild-bounty-sd/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-06-03T07:08:48\"},{\"rid\":135,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/wild-bounty-sd/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-06-03T07:08:48\"},{\"rid\":132,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/wild-coaster/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-06-17T07:07:45\"},{\"rid\":132,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/wild-coaster/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-06-17T07:07:45\"},{\"rid\":1312883,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/prosper-ftree/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-07-08T10:09:01\"},{\"rid\":1312883,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/prosper-ftree/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-07-08T10:09:01\"},{\"rid\":1338274,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/totem-wonders/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-07-13T04:30:27\"},{\"rid\":1338274,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/totem-wonders/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-07-13T04:30:27\"},{\"rid\":1368367,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/alchemy-gold/SGS.png\",\"l\":\"zh-CN\",\"ut\":\"2022-07-20T07:01:19\"},{\"rid\":1368367,\"rtid\":14,\"url\":\"https://public.pg-nmga.com/pages/static/image/en/SocialGameSmall/alchemy-gold/SGS.png\",\"l\":\"en-US\",\"ut\":\"2022-07-20T07:01:19\"}]";
            Sender.Tell(new GetByResourcesResponseData(strGameResource));
        }

        private async Task procGetBetHistoryItems(GITMessage message, UserConnection userConn)
        {
            try
            {
                int gameID          = (int) message.Pop();
                long startTime      = (long)message.Pop();
                long endTime        = (long)message.Pop();
                int pageNumber      = (int) message.Pop();
                int countPerPage    = (int) message.Pop();

                GetBetHistoryResponseData response = await _dbReader.Ask<GetBetHistoryResponseData>(new PGBetHistoryRequest(_agentDBID, _strUserID, gameID, startTime, endTime, pageNumber, countPerPage), TimeSpan.FromSeconds(15));
                Sender.Tell(new GetBetHistoryResponseMessage(JsonConvert.SerializeObject(new GetBetHistoryResponse(response))));
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::procGetBetHistorySummary {0}", ex);
            }
            Sender.Tell(new GetBetHistoryResponseMessage(JsonConvert.SerializeObject(new GetBetHistoryResponse(new GetBetHistoryResponseData()))));
        }
        private async Task procGetBetHistoryDetail(GITMessage message, UserConnection userConn)
        {
            try
            {
                int gameID          = (int) message.Pop();
                long spinRoundID    = (long) message.Pop();

                GetBetHistoryDetailResponseData response = await _dbReader.Ask<GetBetHistoryDetailResponseData>(new PGBetHistoryDetailRequest(_agentDBID, _strUserID, gameID, spinRoundID), TimeSpan.FromSeconds(15));
                Sender.Tell(new GetBetHistoryDetailResponseMessage(JsonConvert.SerializeObject(new GetBetHistoryDetailResponse(response))));
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::procGetBetHistoryDetail {0}", ex);
            }
            Sender.Tell(new GetBetHistoryDetailResponseMessage(JsonConvert.SerializeObject(new GetBetHistoryDetailResponse(new GetBetHistoryDetailResponseData()))));
        }
        private async Task procGetBetHistorySummary(GITMessage message, UserConnection userConn)
        {
            try
            {
                int gameID = (int)message.Pop();
                long startTime = (long)message.Pop();
                long endTime = (long)message.Pop();

                GetBetSummaryResponseData response = await _dbReader.Ask<GetBetSummaryResponseData>(new PGBetSummaryRequest(_agentDBID, _strUserID, gameID, startTime, endTime), TimeSpan.FromSeconds(15));
                Sender.Tell(new GetBetSummaryResponse(response));
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::procGetBetHistorySummary {0}", ex);
            }
            Sender.Tell(new GetBetSummaryResponse(new GetBetSummaryResponseData()));
        }

        #region Callback API관련처리
        public static string createDataSign(string key, string message)
        {
            var hmac    = System.Security.Cryptography.HMAC.Create("HMACSHA256");
            hmac.Key    = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
        }

        private async Task<GetBalanceResponse> callGetBalance(int gameID)
        {
            try
            {
                AgentConfig agentConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
                if (agentConfig == null)
                    return null;

                GetBalanceRequest request = new GetBalanceRequest();
                request.agentID = _agentID;
                request.userID  = _strUserID;
                request.gameID  = gameID;
                request.sign    = createDataSign(agentConfig.SecretKey, string.Format("{0}{1}{2}", _agentID, _strUserID, gameID));

                string      strURL          = string.Format("{0}/GetBalance", agentConfig.CallbackURL);
                HttpResponseMessage message = await _httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();

                string strContent = await message.Content.ReadAsStringAsync();
                GetBalanceResponse response = JsonConvert.DeserializeObject<GetBalanceResponse>(strContent);
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::callGetBalance {0} {1} {2}", ex, _agentID, _strUserID);
                return null;
            }
        }

        private async Task<WithdrawResponse> callWithdraw(int gameID, double amount, string roundID, string transactionID)
        {
            AgentConfig agentConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
            if (agentConfig == null)
                return null;

            try
            {

                WithdrawRequest request = new WithdrawRequest();
                request.agentID         = _agentID;
                request.userID          = _strUserID;
                request.gameID          = gameID;
                request.amount          = (decimal)Math.Round(amount, 2);
                request.roundID         = roundID;
                request.transactionID   = transactionID;
                request.sign = createDataSign(agentConfig.SecretKey, string.Format("{0}{1}{2}{3}{4}{5}", _agentID, _strUserID, request.amount.ToString("0.00"),
                    transactionID, roundID, gameID));

                string strURL = string.Format("{0}/Withdraw", agentConfig.CallbackURL);

                HttpResponseMessage message = await _httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();
                string strContent = await message.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<WithdrawResponse>(strContent);
                if(response.code == 0)
                    _dbWriter.Tell(new ApiTransactionItem(_agentID, _strUserID, gameID, amount, 0.0, transactionID, "", response.platformTransactionID, roundID, false, TransactionTypes.Withdraw, DateTime.UtcNow));

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::callWithdraw {0} {1} {2}", ex, _agentID, _strUserID);
            }

            Context.System.ActorSelection("/user/retryWorkers").Tell(new RollbackRequestWithRetry(agentConfig.CallbackURL,
                _agentID, _strUserID, agentConfig.SecretKey, gameID, 50, DateTime.Now.Subtract(TimeSpan.FromSeconds(10.0)), transactionID, amount, 0.0));

            return null;
        }
        private async Task<DepositResponse> callDeposit(int gameID, double amount, string roundID, string betTransactionID, string transactionID, bool endTransaction)
        {
            AgentConfig agentConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
            if (agentConfig == null)
                return null;

            try
            {

                DepositRequest request = new DepositRequest();
                request.agentID          = _agentID;
                request.userID           = _strUserID;
                request.gameID           = gameID;
                request.amount           = (decimal)Math.Round(amount, 2);
                request.roundID          = roundID;
                request.transactionID    = transactionID;
                request.refTransactionID = betTransactionID;
                request.endRound         = endTransaction;

                request.sign = createDataSign(agentConfig.SecretKey, string.Format("{0}{1}{2}{3}{4}{5}{6}", _agentID, _strUserID, request.amount.ToString("0.00"),
                    betTransactionID, transactionID, roundID, gameID));

                string strURL = string.Format("{0}/Deposit", agentConfig.CallbackURL);

                HttpResponseMessage message = await _httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();
                string strContent = await message.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<DepositResponse>(strContent);
                if (response.code == 0 || response.code == 11)
                    _dbWriter.Tell(new ApiTransactionItem(_agentID, _strUserID, gameID, 0.0, amount, transactionID, betTransactionID, response.platformTransactionID, roundID, endTransaction, TransactionTypes.Deposit, DateTime.UtcNow));
                else
                    _dbWriter.Tell(new FailedTransactionItem(_agentID, _strUserID, TransactionTypes.Deposit, transactionID, betTransactionID, endTransaction, 0.0, amount, gameID, DateTime.UtcNow));
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::callDeposit {0} {1} {2}", ex, _agentID, _strUserID);
            }

            _dbWriter.Tell(new ApiTransactionItem(_agentID, _strUserID, gameID, 0.0, amount, transactionID, betTransactionID, "", roundID, endTransaction, TransactionTypes.Deposit, DateTime.UtcNow));
            Context.System.ActorSelection("/user/retryWorkers").Tell(new DepositRequestWithRetry(agentConfig.CallbackURL,
                _agentID, _strUserID, agentConfig.SecretKey, gameID, 50, DateTime.Now.Subtract(TimeSpan.FromSeconds(10.0)), transactionID, betTransactionID, roundID, endTransaction, amount));

            return null;
        }
        private async Task<WithdrawResponse> callBetWin(int gameID, double betAmount, double winAmount, string roundID, string transactionID)
        {
            AgentConfig agentConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
            if (agentConfig == null)
                return null;

            try
            {

                BetWinRequest request   = new BetWinRequest();
                request.agentID         = _agentID;
                request.userID          = _strUserID;
                request.gameID          = gameID;
                request.betAmount       = (decimal)Math.Round(betAmount, 2);
                request.winAmount       = (decimal)Math.Round(winAmount, 2);
                request.roundID         = roundID;
                request.transactionID   = transactionID;
                request.sign            = createDataSign(agentConfig.SecretKey, string.Format("{0}{1}{2}{3}{4}{5}{6}", _agentID, _strUserID, request.betAmount.ToString("0.00"),
                    request.winAmount.ToString("0.00"), transactionID, roundID, gameID));

                string strURL = string.Format("{0}/BetWin", agentConfig.CallbackURL);

                HttpResponseMessage message = await _httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();
                string strContent = await message.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<WithdrawResponse>(strContent);
                if (response.code == 0)
                    _dbWriter.Tell(new ApiTransactionItem(_agentID, _strUserID, gameID, betAmount, winAmount, transactionID, "", response.platformTransactionID, roundID, true, TransactionTypes.BetWin, DateTime.UtcNow));

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::callBetWin {0} {1} {2}", ex, _agentID, _strUserID);
            }

            Context.System.ActorSelection("/user/retryWorkers").Tell(new RollbackRequestWithRetry(agentConfig.CallbackURL,
                _agentID, _strUserID, agentConfig.SecretKey, gameID, 50, DateTime.Now.Subtract(TimeSpan.FromSeconds(10.0)), transactionID, betAmount, winAmount));

            return null;
        }

        public class GetBalanceRequest
        {
            public string   agentID   { get; set; }
            public string   sign      { get; set; }
            public string   userID    { get; set; }
            public int      gameID    { get; set; }
        }
        public class GitDebitCreditRequest
        {
            public string userid { get; set; }
            public decimal debitamount { get; set; }
            public decimal creditamount { get; set; }
            public int vendor { get; set; }
            public string game { get; set; }
            public string transactionid { get; set; }

        }
        public class GitDebitCreditResponse
        {
            public int status { get; set; }
            public double balance { get; set; }
            public string error { get; set; }
        }
        public class GetBalanceResponse
        {
            public int      code    { get; set; }
            public string   message { get; set; }
            public decimal  balance { get; set; }
        }
        #endregion
        private async Task procSlotGameMsg(GITMessage message, UserConnection userConn)
        {
            //게임에 입장한 상태가 아니라면
            if (userConn.GameActor == null || userConn.GameID == 0)
            {
                Self.Tell(new CloseHttpSession(userConn.Token));
                Sender.Tell("invalidaction");
                return;
            }

            UserBonus waitingBonus = pickUserBonus(userConn.GameID);
            ToUserMessage   toUserMessage   = null;
            try
            {
                toUserMessage = await userConn.GameActor.Ask<ToUserMessage>(new FromUserMessage(_strUserID, _agentDBID, _balance, Self, message, waitingBonus, _currency, _isAffiliate), Constants.RemoteTimeOut);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::procSlotGameMsg {0} gameid: {1}, message code: {2}, {3}",
                    _strGlobalUserID, userConn.GameID, message.MsgCode, ex);
            }
            if (toUserMessage != null)
                await procToUserMessage(toUserMessage, waitingBonus, message, userConn);
            else
                Sender.Tell("nomessagefromslotnode");
        }
        private async Task procToUserMessage(ToUserMessage message, UserBonus askedBonus, GITMessage gameMessage, UserConnection userConn)
        {
            //만일 게임결과처리와 관련된 메세지라면
            if (message is ToUserResultMessage)
            {
                bool isSuccess = await processResultMessage(message as ToUserResultMessage);                
                if(!isSuccess)
                {
                    Self.Tell(new CloseHttpSession(userConn.Token));
                    Sender.Tell("invalidaction");
                    GITMessage errorResultMsg = new GITMessage(MsgCodes.NOTPROCDRESULT);
                    userConn.GameActor.Tell(new FromUserMessage(_strUserID, _agentDBID, _balance, Self, errorResultMsg, null, _currency, _isAffiliate));
                    return;
                }
            }

            if (askedBonus != null && message.IsRewardedBonus)
            {
                _waitingUserBonuses.Remove(askedBonus);
                onRewardCompleted(askedBonus, message.RewardBonusMoney, message.GameID);
            }

            string strResult = (string)message.Messages[0].Pop();
            if (message.Messages[0].MsgCode == MsgCodes.UPDATEGAMEINFO)
            {
                var dt = JsonConvert.DeserializeObject<dynamic>(strResult);
                Sender.Tell(new SpinResponseData(strResult));
            }
            else if (message.Messages[0].MsgCode == MsgCodes.NOTENOUGHBALANCE)
            {
                SpinResponseData response   = new SpinResponseData(3202, "Not enough cash.", genRandomId(8));
                response.dt                 = strResult;
                Sender.Tell(response);
            }
            else
            {
                var dt = new { si = JsonConvert.DeserializeObject<dynamic>(strResult) };
                Sender.Tell(new SpinResponseData(JsonConvert.SerializeObject(dt)));
            }
        }
        protected string genRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Pcg.Default.Next(s.Length)]).ToArray());
        }
        private void onRewardCompleted(UserBonus completedBonus, double rewardedMoney, int gameID)
        {
            if (completedBonus is UserRangeOddEventBonus)
                fetchNewUserRangeEvent(rewardedMoney, gameID);
        }
        private void addPoint(int gameID, double betMoney, double winMoney)
        {
            DateTime nowTime   = DateTime.UtcNow.AddHours(9.0);
            DateTime dateTime  = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day);

            if(betMoney > 0.0)
                _dbWriter.Tell(new UserBetMoneyUpdateItem(_userDBID, betMoney));

            if(betMoney != 0.0 || winMoney != 0.0)
                _dbWriter.Tell(new GameReportItem(gameID, _agentDBID, betMoney, winMoney, dateTime)); 
        }
        private async Task<bool> processResultMessage(ToUserResultMessage resultMessage)
        {
            DateTime        nowReportTime     = DateTime.UtcNow;
            DateTime        nowDayReportTime  = new DateTime(nowReportTime.Year, nowReportTime.Month, nowReportTime.Day);
            //보유머니를 검사한다.

            double betMoney  = Math.Round(resultMessage.BetMoney, 2);
            double winMoney  = Math.Round(resultMessage.WinMoney, 2);

            double beforeBalance = _balance;
            if(betMoney > 0.0 && resultMessage.EndTransaction)
            {
                var response = await callBetWin(resultMessage.GameID, betMoney, winMoney, resultMessage.RoundID, resultMessage.BetTransactionID);
                if (response == null || response.code != 0)
                    return false;
                _balance = Math.Round((double)response.balance, 2);
            }
            else if (betMoney > 0.0)
            {
                var response = await callWithdraw(resultMessage.GameID, betMoney, resultMessage.RoundID, resultMessage.BetTransactionID);
                if (response == null || response.code != 0)
                    return false;
                _balance = Math.Round((double)response.balance, 2);

                if(!string.IsNullOrEmpty(resultMessage.TransactionID))
                {
                    var depositResponse = await callDeposit(resultMessage.GameID, winMoney, resultMessage.RoundID, resultMessage.BetTransactionID, resultMessage.TransactionID, resultMessage.EndTransaction);
                    if (depositResponse == null || (depositResponse.code != 0 && depositResponse.code != 11))
                        _balance = _balance + winMoney;
                    else
                        _balance = Math.Round((double)depositResponse.balance, 2);
                }
            }
            else if (!string.IsNullOrEmpty(resultMessage.TransactionID))
            {
                var response = await callDeposit(resultMessage.GameID, winMoney, resultMessage.RoundID, resultMessage.BetTransactionID, resultMessage.TransactionID, resultMessage.EndTransaction);
                if (response == null || (response.code != 0 && response.code != 11))
                    _balance = _balance + winMoney;
                else
                    _balance = Math.Round((double)response.balance, 2);
            }
            if (betMoney != 0.0 || winMoney != 0.0)
            {
                if (!_isAffiliate)
                    addPoint(resultMessage.GameID, betMoney, winMoney);

                _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, _balance));           
                if(!_isAffiliate)
                   _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowDayReportTime, betMoney, winMoney));
   
                _dbWriter.Tell(new GameLogItem(_strUserID, (int)resultMessage.GameID, resultMessage.GameLog.GameName, 
                                    betMoney, winMoney, beforeBalance, _balance, resultMessage.GameLog.LogString, (int) resultMessage.BetType, nowReportTime, _agentDBID));
            }
            if(resultMessage.HistoryItem != null)
                _dbWriter.Tell(resultMessage.HistoryItem);
            return true;
        }

        private string buildActivePromos()
        {
            return "{\"error\":0,\"description\":\"OK\",\"serverTime\":1676462782,\"tournaments\":[],\"races\":[]}";
        }
#endregion

        private async Task replaceSlotGameNode(string strSlotNodePath, UserConnection userConn)
        {
            int remainServerCount = 0;
            try
            {
                var routees = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<Routees>(new GetRoutees());
                foreach (Routee routee in routees.Members)
                    remainServerCount++;

                _logger.Info("{0} Exiting from slot game node {1}", _strGlobalUserID, strSlotNodePath);
                ExitGameResponse response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _agentDBID, _balance, false, remainServerCount > 0), TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::onSlotGameServerShuttingdown {0}", ex);
            }
            try
            {
                _logger.Info("{0} Reentering slot game node", _strGlobalUserID);
                if (remainServerCount == 0)
                {
                    _logger.Info("{0} no more slot game node found", _strGlobalUserID);
                    Self.Tell(new HttpSessionClosed(userConn.Token));
                    userConn.GameActor      = null;
                    userConn.GameID         = 0;
                    return;
                }

                //다른 슬롯게임노드에 입장한다.
                EnterGameRequest  requestMsg  = new EnterGameRequest(userConn.GameID, _agentDBID, _strUserID, Self, _balance, _currency, false);
                EnterGameResponse responseMsg = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<EnterGameResponse>(requestMsg, Constants.RemoteTimeOut);

                //게임입장성공
                if (responseMsg.Ack == 0)
                    userConn.GameActor = responseMsg.GameActor;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::replaceSlotGameNode {0}", ex);
            }
        }
        private async Task onSlotGameServerShuttingdown(SlotGamesNodeShuttingdown message)
        {
            if (_userConnections.Count == 0)
                return;

            for(int i = 0; i < _userConnections.Count;i++)
            {
                string          strToken  = _userConnections.Keys.ElementAt(i);
                UserConnection  userConn  = _userConnections[strToken];

                if (userConn.GameActor == null || userConn.GameID == 0)
                    continue;

                if (!userConn.GameActor.Path.ToString().Contains(message.Path))
                    continue;

                await replaceSlotGameNode(message.Path, userConn);
            }            
        }

#region Messages
        public class UserLoggedIn
        {
        }
#endregion

        //유저의 연결객체(tcp, websocket, http session)
        public class UserConnection
        {
            public string       Token           { get; set; } //HTTP Session Token
            public IActorRef    GameActor       { get; set; } //입장한 게임액터(null: 게임에 입장하지 않은 상태)
            public int          GameID          { get; set; } //입장한 게임아이디(0: 게임에 입장하지 않은 상태)            
            public DateTime     LastActiveTime  { get; set; } //유저가 마지막으로 서버와 접촉한 시간

            public UserConnection(string sessionToken)
            {
                this.Token          = sessionToken;
                this.GameActor      = null;
                this.GameID         = 0;
                this.LastActiveTime = DateTime.Now;
            }

            public void resetGame()
            {
                this.GameActor          = null;
                this.GameID             = 0;
            }
        }
    }

}
