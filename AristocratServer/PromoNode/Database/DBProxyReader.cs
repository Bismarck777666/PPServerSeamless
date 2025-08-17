using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using System.Data.Common;
using Akka.Routing;
using System.Data.SqlClient;
using Akka.Event;
using GITProtocol;
using Newtonsoft.Json;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Diagnostics;

namespace PromoNode.Database
{
    public class DBProxyReader : ReceiveActor
    {
        private string                      _strConnString  = "";
        private readonly ILoggingAdapter    _logger         = Logging.GetLogger(Context);

        public DBProxyReader(string strConnString)
        {
            _strConnString = strConnString;

            ReceiveAsync<GetActiveTournamentsRequest>   (onGetActiveTournaments);
            ReceiveAsync<GetActiveRaceRequest>          (onGetActiveRaces);
            ReceiveAsync<GetLatestCurrencyMap>          (onGetCurrencyMap);
            ReceiveAsync<GetActiveFreespinsRequest>     (onGetActiveFreespins);
            ReceiveAsync<GetActiveCashbacksRequest>      (onGetActiveCashbacks);
        }

        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
        }
        private int getUnixTimestamp(DateTime dateTime)
        {
            return (int) dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
        private void readTournament(DbDataReader reader, PPTournament tournament)
        {
            tournament.id                           = (int) reader["id"];
            tournament.startDate                    = getUnixTimestamp((DateTime) reader["startdate"]);
            tournament.endDate                      = getUnixTimestamp((DateTime) reader["enddate"]);
            tournament.status                       = (string)reader["status"];
            tournament.name                         = (string) reader["name"];
            tournament.clientMode                   = (string) reader["clientmode"];
            tournament.optJurisdiction              = new List<string>(new string[] { "SE", "UK" });
            tournament.htmlRules                    = (string)reader["htmlrules"];
            tournament.htmlRules                    = tournament.htmlRules.Replace("\\r\\n", "\r\n");
            tournament.shortHtmlRules               = (string)reader["shorthtmlrules"];
            
            tournament.type                         = (int)reader["type"];
            var agentid                             = reader["agentid"];
            tournament.agentid                      = (agentid is System.DBNull) ? null : (string)agentid;
            tournament.currency                     = (int)reader["currency"];
            tournament.games                        = (string)reader["games"];
            tournament.playersinc                   = (string)reader["playersinc"];
            tournament.playersexc                   = (string)reader["playersexc"];

            tournament.prizePool                    = new PPTournamentPrizePool();
            Currencies currency                             = (Currencies)tournament.currency;
            tournament.prizePool.currency                   = currency.ToString();
            tournament.prizePool.currencyOriginal           = currency.ToString();
            tournament.prizePool.minBetLimit                = (double)(decimal)reader["minbetlimit"];
            tournament.prizePool.maxBetLimitByMultiplier    = (double)(decimal)reader["maxbetmultiplier"];
            tournament.prizePool.totalPrizeAmount           = (double)(decimal)reader["totalprizeamount"];
            tournament.prizePool.prizesList                 = JsonConvert.DeserializeObject<List<PPTournamentPrize>>((string)reader["prizelist"]);
        }
        private async Task onGetActiveTournaments(GetActiveTournamentsRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    //1. 진행중인 토너먼트가 있는가를 검사한다.
                    string      strQuery    = "SELECT * FROM pptournaments WHERE status=@status";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@status", "O");

                    List<PPTournament> currentTournaments = new List<PPTournament>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPTournament currentTournament = new PPTournament();
                            readTournament(reader, currentTournament);
                            currentTournament.status = "O";

                            currentTournaments.Add(JsonConvert.DeserializeObject<PPTournament>(JsonConvert.SerializeObject(currentTournament)));
                        }
                    }

                    //2. 마지막으로 종료된 토너먼트를 구한다.
                    strQuery = "SELECT * FROM pptournaments WHERE (status=@statusC OR status=@statusCO) AND enddate > @fromlimittime ORDER BY enddate DESC";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@statusC", "C");
                    command.Parameters.AddWithValue("@statusCO", "CO");
                    command.Parameters.AddWithValue("@fromlimittime", DateTime.UtcNow.AddDays(-1));

                    List<PPTournament> lastTournaments = new List<PPTournament>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPTournament lastTournament = new PPTournament();
                            readTournament(reader, lastTournament);

                            lastTournaments.Add(JsonConvert.DeserializeObject<PPTournament>(JsonConvert.SerializeObject(lastTournament)));
                        }
                    }

                    //토너먼트가 끝났으면 즉시 상태을 정산됨으로 해준다
                    foreach(PPTournament item in lastTournaments)
                    {
                        if (item.status == "C")
                            await updateTournamentStatusCompleted(connection, item.id);
                    }

                    //3. 이제 시작될 토너먼트를 얻는다.
                    strQuery = "SELECT * FROM pptournaments WHERE status=@status AND startdate < @tolimittime ORDER BY startdate";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@status", "S");
                    command.Parameters.AddWithValue("@tolimittime", DateTime.UtcNow.AddHours(6));

                    List<PPTournament> futureTournaments = new List<PPTournament>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPTournament futureTournament = new PPTournament();
                            readTournament(reader, futureTournament);
                            futureTournament.status = "S";

                            futureTournaments.Add(JsonConvert.DeserializeObject<PPTournament>(JsonConvert.SerializeObject(futureTournament)));
                        }
                    }

                    List<PPTournament> tournaments = new List<PPTournament>();

                    if (lastTournaments != null && lastTournaments.Count > 0)
                    {
                        foreach (PPTournament tournament in lastTournaments)
                            tournaments.Add(tournament);
                    }
                    
                    if (currentTournaments != null && currentTournaments.Count > 0)
                    {
                        foreach (PPTournament tournament in currentTournaments)
                            tournaments.Add(tournament);
                    }
                    
                    if (futureTournaments != null && futureTournaments.Count > 0)
                    {
                        foreach (PPTournament tournament in lastTournaments)
                            tournaments.Add(tournament);
                    }

                    for (int i = 0; i < tournaments.Count; i++)
                        await readTounamentLeaderBoard(connection, tournaments[i]);

                    Sender.Tell(new GetActiveTournamentsResponse(tournaments));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyReader::onGetActiveTournaments {0}", ex);
                Sender.Tell(new GetActiveTournamentsResponse(null));
            }
        }
        private async Task readTounamentLeaderBoard(SqlConnection connection, PPTournament tournament)
        {
            try
            { 
                string strQuery         = "SELECT * FROM pptournamentleaders WHERE tournamentid=@tournamentid ORDER BY score DESC";
                SqlCommand sqlCommand   = new SqlCommand(strQuery, connection);
                sqlCommand.Parameters.AddWithValue("@tournamentid", tournament.id);

                tournament.leaderBoard = new List<PPTournamentLeaderItem>();

                using (DbDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        PPTournamentLeaderItem leader = new PPTournamentLeaderItem();

                        leader.playerID         = (string)reader["username"];
                        leader.totalbet         = (double)(decimal)reader["totalbet"];
                        leader.totalwin         = (double)(decimal)reader["totalwin"];
                        leader.score            = (double)(decimal)reader["score"];
                        leader.scoreBet         = (double)(decimal)reader["scorebet"];
                        leader.memberCurrency   = (string)reader["currency"];
                        leader.countryID        = (string)reader["country"];
                        leader.win              = (double)(decimal)reader["win"];
                        leader.effectiveBetForFreeRounds = leader.effectiveBetForBetMultiplier = leader.scoreBet;

                        tournament.leaderBoard.Add(leader);
                    }
                }

                for (int i = 0; i < tournament.leaderBoard.Count; i++) 
                {
                    int position = 0;
                    for (int j = 0; j < tournament.leaderBoard.Count; j++)
                    {
                        if(i == j)
                            continue;

                        if (tournament.leaderBoard[i].score < tournament.leaderBoard[j].score)
                            position++;
                    }
                    tournament.leaderBoard[i].position = position + 1;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyReader::readTounamentLeaderBoard {0}", ex);
            }
        }
        private async Task updateTournamentStatusCompleted(SqlConnection connection, int tournamentid)
        {
            do
            {
                try
                {
                    string strQuery = "UPDATE pptournaments SET status=@status WHERE id=@id";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@status",  "CO");
                    command.Parameters.AddWithValue("@id",      tournamentid);
                    await command.ExecuteNonQueryAsync();
                    
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in DBProxyReader::updateTournamentStatusCompleted {0}", ex);
                }
            }
            while(true);
        }
        private void readRace(DbDataReader reader, PPRace raceInfo)
        {
            raceInfo.id                                 = (int)reader["id"];
            raceInfo.startDate                          = getUnixTimestamp((DateTime)reader["startdate"]);
            raceInfo.endDate                            = getUnixTimestamp((DateTime)reader["enddate"]);
            raceInfo.name                               = (string)reader["name"];
            raceInfo.clientMode                         = (string)reader["clientmode"];
            raceInfo.clientStyle                        = (string)reader["clientstyle"];
            raceInfo.htmlRules                          = (string)reader["htmlrules"];
            raceInfo.htmlRules                          = raceInfo.htmlRules.Replace("\\r\\n", "\r\n");
            raceInfo.shortHtmlRules                     = (string)reader["shorthtmlrules"];
            var agentid = reader["agentid"];
            raceInfo.agentid                            = (agentid is System.DBNull) ? null : (string)agentid;
            raceInfo.prizesLimit                        = (int)reader["prizeslimit"];
            raceInfo.games                              = (string)reader["games"];
            raceInfo.playersinc                         = (string)reader["playersinc"];
            raceInfo.playersexc                         = (string)reader["playersexc"];
            
            Currencies currency                         = (Currencies)(int)reader["currency"];
            raceInfo.prizePool                          = new PPRacePrizePool();
            raceInfo.prizePool.currency                 = currency.ToString();
            raceInfo.prizePool.currencyOriginal         = currency.ToString();
            raceInfo.prizePool.minBetLimit              = (double)(decimal)reader["minbetlimit"];
            raceInfo.prizePool.maxBetLimitByMultiplier  = (double)(decimal)reader["maxbetmultiplier"];
            raceInfo.prizePool.prizesList               = JsonConvert.DeserializeObject<List<PPRacePrize>>((string)reader["prizelist"]);
            raceInfo.optin                              = true;
            raceInfo.showWinnersList                    = (bool)reader["showwinners"];
        }
        private void readCashback(DbDataReader reader, PPCashback raceInfo)
        {
            raceInfo.id = (int)reader["id"];
            var agentid = reader["agentid"];
            raceInfo.agentid = (agentid is System.DBNull) ? null : (string)agentid;
            raceInfo.name = (string)reader["name"];
            Currencies currency = (Currencies)(int)reader["currency"];
            raceInfo.startDate = getUnixTimestamp((DateTime)reader["startdate"]);
            raceInfo.endDate = getUnixTimestamp((DateTime)reader["enddate"]);
            raceInfo.showWinnersList = false;
            raceInfo.clientMode = (string)reader["clientmode"];
            raceInfo.clientStyle = (string)reader["clientstyle"];
            raceInfo.htmlRules = (string)reader["htmlrules"];
            raceInfo.htmlRules = raceInfo.htmlRules.Replace("\\r\\n", "\r\n");
            raceInfo.shortHtmlRules = (string)reader["shorthtmlrules"];
           
            
            raceInfo.games = (string)reader["games"];
            raceInfo.playersinc = (string)reader["playersinc"];
            raceInfo.playersexc = (string)reader["playersexc"];

           
            raceInfo.prizePool = new PPRacePrizePool();
            raceInfo.prizePool.currency = currency.ToString();
            raceInfo.prizePool.currencyOriginal = currency.ToString();
            raceInfo.prizePool.minBetLimit = (double)(decimal)reader["minbetlimit"];
            raceInfo.prizePool.totalPrizeAmount = 0;
            raceInfo.prizePool.prizesList = new List<PPRacePrize>();
            var prize = new PPRacePrize();
            prize.prizeID = 1;
            prize.count = 0;
            prize.type = "G";
            prize.gift = "Cashback";            
            prize.amount = 0.0;
            prize.betMultiplier = 0.0;
            raceInfo.prizePool.prizesList.Add(prize);                
            raceInfo.optin = true;

            raceInfo.type = 1;
            raceInfo.cType = (int)reader["type"];
            raceInfo.period = (int)reader["period"];
            raceInfo.minNet = (double)(decimal)reader["minnet"];
            raceInfo.cashback = (double)(decimal)reader["cashback"];
            raceInfo.rounds = (int)reader["rounds"];
            
        }
        private async Task readRaceWinners(SqlConnection connection, PPRace race, List<PPRace> lastRaces)
        {
            try
            { 
                string strQuery         = "SELECT * FROM ppracewinners WHERE raceid=@raceid and processed <> 2";
                SqlCommand sqlCommand   = new SqlCommand(strQuery, connection);
                sqlCommand.Parameters.AddWithValue("@raceid", race.id);

                race.winners            = new List<PPRaceWinner>();
                race.dicWinners         = new Dictionary<long, bool>();
                race.pendingPrizes      = new List<PPRaceWinner>();

                using (DbDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {

                        PPRaceWinner raceWinner     = new PPRaceWinner();
                        raceWinner.playerID         = (string)              reader["username"];
                        raceWinner.countryID        = (string)              reader["country"];
                        raceWinner.memberCurrency   = (string)              reader["currency"];
                        raceWinner.prizeID          = (int)                 reader["prizeID"];
                        raceWinner.bet              = (double) (decimal)    reader["bet"];
                        raceWinner.id               = (long)                reader["id"];
                        int processed               = (int)                 reader["processed"];

                        raceWinner.effectiveBetForBetMultiplier = raceWinner.effectiveBetForFreeRounds = raceWinner.bet;

                        if(processed == 1)
                        {
                            race.winners.Add(raceWinner);
                            race.dicWinners.Add(raceWinner.id, true);
                        }
                        else if(processed == 0)
                        { 
                            race.pendingPrizes.Add(raceWinner);
                        }
                    }
                }

                race.lbGuid = Guid.NewGuid().ToString();
                if (lastRaces != null)
                {
                    PPRace lastRace = null;
                    for (int i = 0; i < lastRaces.Count; i++)
                    {
                        if(lastRaces[i].id == race.id)
                        {
                            lastRace = lastRaces[i];
                            break;
                        }
                    }
                    if (lastRace != null)
                    {
                        bool isDifferent = false;
                        if(lastRace.winners.Count == race.winners.Count)
                        {
                            foreach(long winnerId in  race.dicWinners.Keys)
                            {
                                if(!lastRace.dicWinners.ContainsKey(winnerId))
                                {
                                    isDifferent = true;
                                    break;
                                }
                            }
                            if (!isDifferent)
                                race.lbGuid = lastRace.lbGuid;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyReader::readRaceLeaderBoard {0}", ex);
            }
        }
        private async Task onGetCurrencyMap(GetLatestCurrencyMap _)
        {
            Dictionary<string, double> currencyMap = new Dictionary<string, double>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT currency, rate FROM ppcurrencymap";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strCurrency  = (string)reader["currency"];
                            double rate         = (double)reader["rate"];
                            currencyMap[strCurrency] = rate;
                        }
                    }                   
                    Sender.Tell(currencyMap);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyReader::onGetCurrencyMap {0}", ex);
                Sender.Tell(currencyMap);
            }
        }
        private async Task onGetActiveRaces(GetActiveRaceRequest _)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    //1. 진행중인 레이스가 있는가를 검사한다.
                    string strQuery = "SELECT * FROM ppraces WHERE status=@status";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@currenttime", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@status", "O");

                    List<PPRace> currentRaces = new List<PPRace>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPRace currentRace = new PPRace();
                            readRace(reader, currentRace);
                            currentRace.status = "O";

                            currentRaces.Add(JsonConvert.DeserializeObject<PPRace>(JsonConvert.SerializeObject(currentRace)));
                        }
                    }

                    //2. 마지막으로 종료된 레이스를 구한다.
                    strQuery = "SELECT * FROM ppraces WHERE status=@status AND enddate > @fromlimittime ORDER BY enddate DESC";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@status", "C");
                    command.Parameters.AddWithValue("@fromlimittime", DateTime.UtcNow.AddDays(-1));
                    List<PPRace> lastRaces = new List<PPRace>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPRace lastRace = new PPRace();
                            readRace(reader, lastRace);
                            lastRace.status = "C";
                            lastRaces.Add(JsonConvert.DeserializeObject<PPRace>(JsonConvert.SerializeObject(lastRace)));
                        }
                    }

                    //3. 이제 시작될 레이스를 얻는다.
                    strQuery = "SELECT * FROM ppraces WHERE status=@status AND startdate < @tolimittime ORDER BY startdate";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@status", "S");
                    command.Parameters.AddWithValue("@tolimittime", DateTime.UtcNow.AddHours(6));
                    List<PPRace> futureRaces = new List<PPRace>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPRace futureRace = new PPRace();
                            readRace(reader, futureRace);
                            futureRace.status = "S";
                            futureRaces.Add(JsonConvert.DeserializeObject<PPRace>(JsonConvert.SerializeObject(futureRace)));
                        }
                    }

                    List<PPRace> races = new List<PPRace>();
                    if (lastRaces != null && lastRaces.Count > 0)
                    {
                        foreach (PPRace lastRace in lastRaces)
                        {
                            races.Add(lastRace);
                        }
                    }
                    if (currentRaces != null && currentRaces.Count > 0)
                    {
                        foreach (PPRace curRace in currentRaces) 
                        { 
                            races.Add(curRace);
                        }
                    }
                    if (futureRaces != null && futureRaces.Count > 0)
                    {
                        foreach (PPRace futureRace in futureRaces)
                        {
                            races.Add(futureRace);
                        }
                    }

                    for (int i = 0; i < races.Count; i++)
                        await readRaceWinners(connection, races[i], _.LastRaceList);

                    Sender.Tell(new GetActiveRaceResponse(races));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyReader::onGetActiveRaces {0}", ex);
                Sender.Tell(new GetActiveRaceResponse(null));
            }
        }
    
        private void readFreespin(DbDataReader reader, PPFreespin freespin)
        {
            freespin.id = (int)reader["id"];            
            freespin.name = (string)reader["name"];           
            var agentid = reader["agentid"];
            freespin.agentid = (agentid is System.DBNull) ? null : (string)agentid;
            freespin.startDate = getUnixTimestamp((DateTime)reader["startdate"]);
            freespin.endDate = getUnixTimestamp((DateTime)reader["enddate"]);
            freespin.status = (string)reader["status"];
            freespin.currency = (int)reader["currency"];
            freespin.level = (int)reader["level"];
            freespin.fscount = (int)reader["fscount"];
            freespin.games = (string)reader["games"];
            freespin.playersinc = (string)reader["playersinc"];
            freespin.playersexc = (string)reader["playersexc"];
        }

        private async Task onGetActiveFreespins(GetActiveFreespinsRequest _)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                   
                    string strQuery = "SELECT * FROM ppfreespins WHERE status=@status";
                    SqlCommand command = new SqlCommand(strQuery, connection);               
                    command.Parameters.AddWithValue("@status", "O");

                    List<PPFreespin> currentFreespins = new List<PPFreespin>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPFreespin currentFreespin = new PPFreespin();
                            readFreespin(reader, currentFreespin);                     

                            currentFreespins.Add(JsonConvert.DeserializeObject<PPFreespin>(JsonConvert.SerializeObject(currentFreespin)));
                        }
                    }

                   

                    List<PPFreespin> freespins = new List<PPFreespin>();
          
                    if (currentFreespins != null && currentFreespins.Count > 0)
                    {
                        foreach (PPFreespin curFreespin in currentFreespins)
                        {
                            freespins.Add(curFreespin);
                        }
                    }
                    Sender.Tell(new GetActiveFreespinsResponse(freespins));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyReader::onGetActiveFreespins {0}", ex);
                Sender.Tell(new GetActiveRaceResponse(null));
            }
        }

        private async Task onGetActiveCashbacks(GetActiveCashbacksRequest _)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    //1. 진행중인 레이스가 있는가를 검사한다.
                    string strQuery = "SELECT * FROM ppcashbacks WHERE status=@status";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@currenttime", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@status", "O");

                    List<PPCashback> currentCashbacks = new List<PPCashback>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPCashback currentCashback = new PPCashback();
                            readCashback(reader, currentCashback);
                            currentCashback.status = "O";

                            currentCashbacks.Add(JsonConvert.DeserializeObject<PPCashback>(JsonConvert.SerializeObject(currentCashback)));
                        }
                    }

                    //2. 마지막으로 종료된 레이스를 구한다.
                    strQuery = "SELECT * FROM ppcashbacks WHERE status=@status AND enddate > @fromlimittime ORDER BY enddate DESC";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@status", "C");
                    command.Parameters.AddWithValue("@fromlimittime", DateTime.UtcNow.AddDays(-1));
                    List<PPCashback> lastCashbacks = new List<PPCashback>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPCashback lastCashback = new PPCashback();
                            readCashback(reader, lastCashback);
                            lastCashback.status = "C";
                            lastCashbacks.Add(JsonConvert.DeserializeObject<PPCashback>(JsonConvert.SerializeObject(lastCashback)));
                        }
                    }

                    //3. 이제 시작될 레이스를 얻는다.
                    strQuery = "SELECT * FROM ppcashbacks WHERE status=@status AND startdate < @tolimittime ORDER BY startdate";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@status", "S");
                    command.Parameters.AddWithValue("@tolimittime", DateTime.UtcNow.AddHours(6));
                    List<PPCashback> futureCashbacks = new List<PPCashback>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPCashback futureCashback = new PPCashback();
                            readCashback(reader, futureCashback);
                            futureCashback.status = "S";
                            futureCashbacks.Add(JsonConvert.DeserializeObject<PPCashback>(JsonConvert.SerializeObject(futureCashback)));
                        }
                    }

                    List<PPCashback> cashbacks = new List<PPCashback>();
                    if (lastCashbacks != null && lastCashbacks.Count > 0)
                    {
                        foreach (PPCashback lastCashback in lastCashbacks)
                        {
                            cashbacks.Add(lastCashback);
                        }
                    }
                    if (currentCashbacks != null && currentCashbacks.Count > 0)
                    {
                        foreach (PPCashback curCashback in currentCashbacks)
                        {
                            cashbacks.Add(curCashback);
                        }
                    }
                    if (futureCashbacks != null && futureCashbacks.Count > 0)
                    {
                        foreach (PPCashback futureCashback in futureCashbacks)
                        {
                            cashbacks.Add(futureCashback);
                        }
                    }

                    Sender.Tell(new GetActiveCashbacksResponse(cashbacks));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyReader::onGetActiveRaces {0}", ex);
                Sender.Tell(new GetActiveRaceResponse(null));
            }
        }
    }
}
