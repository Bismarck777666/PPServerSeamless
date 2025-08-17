using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.Database
{
    public class SpinDBReader : ReceiveActor
    {
        private readonly ILoggingAdapter _logger        = Logging.GetLogger(Context);
        private string                   _dbName        = "ppspindb";

        public SpinDBReader() 
        {
            ReceiveAsync<ReadInfoCollectionRequest>     (onReadInfoCollection);
            ReceiveAsync<SelectSpinDataByIDRequest>     (onSelectSpinDataByID);
            ReceiveAsync<SelectPurchaseSpinRequest>     (onSelectPurchaseSpin);
            ReceiveAsync<SelectSpinTypeOddRangeRequest> (onSelectSpinTypeOddRange);
            ReceiveAsync<SelectFloorOddRangeRequest>    (onSelectFloorOddRange);
            ReceiveAsync<SelectSelFreeSpinRangeRequest> (onSelectSelFreeSpinRange);
            ReceiveAsync<SelectSpinTypeIDRangeRequest>  (onSelectSpinTypeIDRange);
            ReceiveAsync<SelectSelFreeMinStartRequest>  (onSelectSelFreeMinStart);
            ReceiveAsync<SelectAllSequenceRequest>      (onSelectAllSequence);
            ReceiveAsync<ReadPurchaseSamplesRequest>    (onReadPurchaseSamples);
            ReceiveAsync<ReadGambleOddsRequest>         (onReadGambleOdds);
        }
        private async Task onSelectSpinDataByID(SelectSpinDataByIDRequest request)
        {
            try
            {
                var dbClient    = new MongoClient("mongodb://127.0.0.1");
                var spinDB      = dbClient.GetDatabase(_dbName);
                var collection  = spinDB.GetCollection<BsonDocument>(request.GameName);
                var filter      = Builders<BsonDocument>.Filter.Eq("_id", request.ID);

                var spinData    = await collection.Find(filter).FirstAsync();
                Sender.Tell(spinData);
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onSelectSpinDataByID {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onSelectPurchaseSpin(SelectPurchaseSpinRequest request)
        {
            try
            {
                var dbClient    = new MongoClient("mongodb://127.0.0.1");
                var spinDB      = dbClient.GetDatabase(_dbName);
                var collection  = spinDB.GetCollection<BsonDocument>(request.GameName);

                BsonDocument document = null;
                if (request.SearchType == StartSpinSearchTypes.GENERAL)
                    document = await collection.AsQueryable().Where(x => x["spintype"] == 1).Sample(1).FirstAsync();
                else if(request.SearchType == StartSpinSearchTypes.SELFREE)
                    document = await collection.AsQueryable().Where(x => x["spintype"] == 100).Sample(1).FirstAsync();
                else if(request.SearchType == StartSpinSearchTypes.SPECIFIC)
                    document = await collection.AsQueryable().Where(x => x["puri"] == 0).Sample(1).FirstAsync();
                else if(request.SearchType == StartSpinSearchTypes.MULTISPECIFIC)
                    document = await collection.AsQueryable().Where(x => x["puri"] == request.SearchParam).Sample(1).FirstAsync();

                Sender.Tell(document);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onSelectPurchaseSpin {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onSelectSpinTypeIDRange(SelectSpinTypeIDRangeRequest request)
        {
            try
            {
                var dbClient = new MongoClient("mongodb://127.0.0.1");
                var spinDB = dbClient.GetDatabase(_dbName);
                var collection = spinDB.GetCollection<BsonDocument>(request.GameName);

                BsonDocument document = null;
                if (request.MaxID < 0)
                    document = await collection.AsQueryable().Where(x => (x["spintype"] == request.SpinType)).Sample(1).FirstAsync();
                else
                    document = await collection.AsQueryable().Where(x => (x["_id"] <= request.MaxID && x["spintype"] == request.SpinType)).Sample(1).FirstAsync(); ;

                Sender.Tell(document);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onSelectSpinTypeOddRange {0} {1}", request.GameName, ex);
                Sender.Tell(null);
            }
        }
        private async Task onSelectFloorOddRange(SelectFloorOddRangeRequest request)
        {
            try
            {
                var dbClient    = new MongoClient("mongodb://127.0.0.1");
                var spinDB      = dbClient.GetDatabase(_dbName);
                var collection  = spinDB.GetCollection<BsonDocument>(request.GameName);

                BsonDocument document = await collection.AsQueryable().Where(x => (x["beginfloor"] == request.Floor && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd)).Sample(1).FirstAsync();
                Sender.Tell(document);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onSelectFloorOddRange {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onSelectSpinTypeOddRange(SelectSpinTypeOddRangeRequest request)
        {
            try
            {
                var dbClient    = new MongoClient("mongodb://127.0.0.1");
                var spinDB      = dbClient.GetDatabase(_dbName);
                var collection  = spinDB.GetCollection<BsonDocument>(request.GameName);

                BsonDocument document = null;
                if (request is SelectSpinTypeOddRangeRequestWithBetType)
                {
                    int betType = (request as SelectSpinTypeOddRangeRequestWithBetType).BetType;
                    if (request.Puri >= 0)
                        document = await collection.AsQueryable().Where(x => (x["puri"] == request.Puri && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd && x["bettype"] == betType)).Sample(1).FirstAsync();
                    else if (request.SpinType >= 0)
                        document = await collection.AsQueryable().Where(x => (x["spintype"] == request.SpinType && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd && x["bettype"] == betType)).Sample(1).FirstAsync();
                    else
                        document = await collection.AsQueryable().Where(x => (x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd && x["bettype"] == betType)).Sample(1).FirstAsync();
                }
                else if(request is SelectSpinTypeOddRangeRequestWithPuri)
                {
                    document = await collection.AsQueryable().Where(x => (x["spintype"] == request.SpinType && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd && x["puri"] == request.Puri)).Sample(1).FirstAsync();
                }
                else if(request is SelectSpinTypeOddRangeRequestSkipSpecificPuri)
                {
                    document = await collection.AsQueryable().Where(x => (x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd && x["puri"] != request.Puri)).Sample(1).FirstAsync();
                }
                else
                {

                    var filter = Builders<BsonDocument>.Filter.Exists("puri", false);
                    if (request.IsPuriNull)
                        document = await collection.AsQueryable().Where(x => (filter.Inject() && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd)).Sample(1).FirstAsync();
                    else if (request.Puri >= 0)
                        document = await collection.AsQueryable().Where(x => (x["puri"] == request.Puri && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd)).Sample(1).FirstAsync();
                    else if (request.SpinType >= 0)
                        document = await collection.AsQueryable().Where(x => (x["spintype"] == request.SpinType && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd)).Sample(1).FirstAsync();
                    else
                        document = await collection.AsQueryable().Where(x => (x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd)).Sample(1).FirstAsync();

                }
                Sender.Tell(document);
            }
            catch(Exception ex)
            {
                //_logger.Error("Exception has been occurred in SpinDBReader::onSelectSpinTypeOddRange {0} {1}", ex, request.GameName);
                Sender.Tell(null);
            }
        }

        private async Task onSelectSelFreeMinStart(SelectSelFreeMinStartRequest request)
        {
            try
            {
                var dbClient    = new MongoClient("mongodb://127.0.0.1");
                var spinDB      = dbClient.GetDatabase(_dbName);
                var collection  = spinDB.GetCollection<BsonDocument>(request.GameName);


                BsonDocument document = null;
                if (request.Puri == -1)
                    document = await collection.AsQueryable().Where(x => (x["spintype"] == 100 && (x["ranges"] as BsonArray).Contains(0))).Sample(1).FirstAsync();
                else
                    document = await collection.AsQueryable().Where(x => (x["spintype"] == 100 && x["puri"] == request.Puri && (x["ranges"] as BsonArray).Contains(0))).Sample(1).FirstAsync();

                Sender.Tell(document);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onSelectSelFreeMinStart {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onSelectSelFreeSpinRange(SelectSelFreeSpinRangeRequest request)
        {
            try
            {
                var dbClient    = new MongoClient("mongodb://127.0.0.1");
                var spinDB      = dbClient.GetDatabase(_dbName);
                var collection  = spinDB.GetCollection<BsonDocument>(request.GameName);

                BsonDocument document = null;
                if (request is SelectSelFreeGroupSpinRangeRequest)
                {
                    SelectSelFreeGroupSpinRangeRequest groupReq = request as SelectSelFreeGroupSpinRangeRequest;
                    document = await collection.AsQueryable().Where(x => (x["groupid"] == groupReq.GroupID && x["spintype"] == 0 && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd) ||
                        (x["groupid"] == groupReq.GroupID && x["spintype"] == 100 && (x["ranges"] as BsonArray).Contains(request.RangeID))).Sample(1).FirstOrDefaultAsync(); ;
                }
                else if(request is SelectSelFreeOnlyPuri0SpinRangeRequest)
                {
                    var filter = Builders<BsonDocument>.Filter.Exists("puri", false);
                    document = await collection.AsQueryable().Where(x => (filter.Inject() && x["spintype"] == 0 && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd) ||
                        ((filter.Inject() || x["puri"] == 0) && x["spintype"] == 100 && (x["ranges"] as BsonArray).Contains(request.RangeID))).Sample(1).FirstOrDefaultAsync(); ;
                }
                else
                {
                    document = await collection.AsQueryable().Where(x => (x["spintype"] == 0 && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd) ||
                        (x["spintype"] == 100 && (x["ranges"] as BsonArray).Contains(request.RangeID))).Sample(1).FirstOrDefaultAsync(); ;
                }
                Sender.Tell(document);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onSelectSpinTypeOddRange {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onReadInfoCollection(ReadInfoCollectionRequest request)
        {
            try
            {
                var dbClient    = new MongoClient("mongodb://127.0.0.1");
                var spinDB      = dbClient.GetDatabase(_dbName);
                var collection  = spinDB.GetCollection<BsonDocument>("infos");
                var documents   = await collection.Find(_ => true).ToListAsync();

                Sender.Tell(documents);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onReadInfoCollection {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onReadGambleOdds(ReadGambleOddsRequest request)
        {
            try
            {
                var dbClient    = new MongoClient("mongodb://127.0.0.1");
                var spinDB      = dbClient.GetDatabase(_dbName);
                var collection  = spinDB.GetCollection<BsonDocument>(request.GameName + "_gambleodds");
                var documents   = await collection.Find(_ => true).ToListAsync();
                Sender.Tell(documents);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onReadGambleOdds {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onReadPurchaseSamples(ReadPurchaseSamplesRequest request)
        {
            try
            {
                var dbClient    = new MongoClient("mongodb://127.0.0.1");
                var spinDB      = dbClient.GetDatabase(_dbName);
                var collection  = spinDB.GetCollection<BsonDocument>(request.GameName + "_pursamples");
                var documents   = await collection.Find(_ => true).ToListAsync();

                Sender.Tell(documents);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onReadPurchaseSamples {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onSelectAllSequence(SelectAllSequenceRequest request)
        {
            try
            {
                var dbClient = new MongoClient("mongodb://127.0.0.1");
                var spinDB = dbClient.GetDatabase(_dbName);
                var collection = spinDB.GetCollection<BsonDocument>(request.GameName + "_sequences");

                var documents = await collection.Find(_ => true).ToListAsync();

                Sender.Tell(documents);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onSelectAllSequence {0}", ex);
                Sender.Tell(null);
            }
        }
        
    }

    public class ReadInfoCollectionRequest : IConsistentHashable
    {
        public object ConsistentHashKey
        {
            get
            {
                return "Info";
            }
        }
    }
    public class SelectSpinDataByIDRequest : IConsistentHashable
    {
        public string GameName  { get; set; }
        public int    ID        { get; set; }
        public SelectSpinDataByIDRequest(string strGameName, int id)
        {
            this.GameName   = strGameName;
            this.ID         = id;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.GameName;
            }
        }

    }

    public enum StartSpinSearchTypes
    {
        GENERAL         = 0,
        SELFREE         = 1,
        SPECIFIC        = 2,
        MULTISPECIFIC   = 3,
    }
    public class SelectPurchaseSpinRequest : IConsistentHashable
    {
        public string               GameName    { get; set; }
        public StartSpinSearchTypes SearchType  { get; private set; }
        public int                  SearchParam { get; private set; }

        public SelectPurchaseSpinRequest(string strGameName, StartSpinSearchTypes searchType, int searchParam = 0)
        {
            GameName    = strGameName;
            SearchType  = searchType;
            SearchParam = searchParam;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.GameName;
            }
        }
    }
    public class SelectSpinTypeOddRangeRequest : IConsistentHashable
    {
        public string   GameName    { get; private set; }
        public int      SpinType    { get; private set; }
        public double   MinOdd      { get; private set; }
        public double   MaxOdd      { get; private set; }
        public int      Puri        { get; private set; }
        public bool     IsPuriNull  { get; private set; }
        public SelectSpinTypeOddRangeRequest(string strGameName, int spinType, double minOdd, double maxOdd, int puri = -1, bool isPuriNull = false)
        {
            GameName    = strGameName;
            SpinType    = spinType;
            MinOdd      = minOdd;
            MaxOdd      = maxOdd;
            Puri        = puri;
            IsPuriNull  = isPuriNull;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.GameName;
            }
        }
    }

    public class SelectSpinTypeOddRangeRequestWithBetType : SelectSpinTypeOddRangeRequest
    {
        public int BetType { get; private set; }
        public SelectSpinTypeOddRangeRequestWithBetType(string strGameName, int spinType, double minOdd, double maxOdd, int puri, int betType):
            base(strGameName, spinType, minOdd, maxOdd, puri)
        {
            this.BetType = betType;
        }
    }
    public class SelectSpinTypeOddRangeRequestWithPuri : SelectSpinTypeOddRangeRequest
    {
        public SelectSpinTypeOddRangeRequestWithPuri(string strGameName, int spinType, double minOdd, double maxOdd, int puri) :
            base(strGameName, spinType, minOdd, maxOdd, puri)
        {
        }
    }
    public class SelectSpinTypeOddRangeRequestSkipSpecificPuri : SelectSpinTypeOddRangeRequest
    { 
        public SelectSpinTypeOddRangeRequestSkipSpecificPuri(string strGameName, double minOdd, double maxOdd, int puri) :
            base(strGameName, -1, minOdd, maxOdd, puri)
        {
        }
    }
    public class SelectFloorOddRangeRequest : IConsistentHashable
    {
        public string   GameName { get; private set; }
        public int      Floor    { get; private set; }
        public double   MinOdd   { get; private set; }
        public double   MaxOdd   { get; private set; }
        public SelectFloorOddRangeRequest(string strGameName, int floor, double minOdd, double maxOdd)
        {
            GameName    = strGameName;
            Floor       = floor;
            MinOdd      = minOdd;
            MaxOdd      = maxOdd;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.GameName;
            }
        }
    }
    public class SelectSpinTypeIDRangeRequest : IConsistentHashable
    {
        public string   GameName    { get; private set; }
        public int      SpinType    { get; private set; }
        public int      MaxID       { get; private set; }
        public SelectSpinTypeIDRangeRequest(string strGameName, int spinType, int maxID)
        {
            GameName    = strGameName;
            SpinType    = spinType;
            MaxID       = maxID;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.GameName;
            }
        }
    }
    public class SelectSelFreeSpinRangeRequest : IConsistentHashable
    {
        public string GameName  { get; private set; }
        public double MinOdd    { get; private set; }
        public double MaxOdd    { get; private set; }
        public int    RangeID   { get; private set; }
        public SelectSelFreeSpinRangeRequest(string strGameName, double minOdd, double maxOdd, int rangeID)
        {
            GameName = strGameName;
            RangeID  = rangeID;
            MinOdd   = minOdd;
            MaxOdd   = maxOdd;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.GameName;
            }
        }
    }
    public class SelectSelFreeGroupSpinRangeRequest : SelectSelFreeSpinRangeRequest
    {
        public int GroupID { get; private set; }
        public SelectSelFreeGroupSpinRangeRequest(string strGameName, double minOdd, double maxOdd, int rangeID, int groupID) :
            base(strGameName, minOdd, maxOdd, rangeID)
        {
            this.GroupID = groupID;
        }
    }
    public class SelectSelFreeOnlyPuri0SpinRangeRequest : SelectSelFreeSpinRangeRequest
    {
        public SelectSelFreeOnlyPuri0SpinRangeRequest(string strGameName, double minOdd, double maxOdd, int rangeID) :
            base(strGameName, minOdd, maxOdd, rangeID)
        {
        }
    }
    public class SelectSelFreeMinStartRequest : IConsistentHashable
    {
        public string GameName  { get; private set; }
        public int    Puri      { get; private set; }
        public SelectSelFreeMinStartRequest(string strGameName, int puri)
        {
            GameName = strGameName;
            Puri     = puri;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.GameName;
            }
        }
    }

    public class SelectAllSequenceRequest : IConsistentHashable
    {
        public string GameName { get; private set; }
        public SelectAllSequenceRequest(string strGameName)
        {
            GameName = strGameName;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.GameName;
            }
        }
    }
    public class ReadPurchaseSamplesRequest : IConsistentHashable
    {
        public string GameName { get; private set; }
        public ReadPurchaseSamplesRequest(string strGameName)
        {
            GameName = strGameName;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.GameName;
            }
        }
    }
    public class ReadGambleOddsRequest : IConsistentHashable
    {
        public string GameName { get; private set; }
        public ReadGambleOddsRequest(string strGameName)
        {
            GameName = strGameName;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.GameName;
            }
        }
    }
    public class GambleOdd
    {
        public double MinOdd  { get; set; }
        public double MaxOdd  { get; set; }
        public double RealOdd { get; set; }
        public double Percent { get; set; }

        public GambleOdd()
        {

        }
        public GambleOdd(double minOdd, double maxOdd, double realOdd, double percent)
        {
            MinOdd = minOdd;
            MaxOdd = maxOdd;
            RealOdd = realOdd;
            Percent = percent;
        }   
    }
}
