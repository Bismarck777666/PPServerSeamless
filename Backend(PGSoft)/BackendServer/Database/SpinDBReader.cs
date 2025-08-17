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
        private string                      _strMongoDBURL  = "mongodb://127.0.0.1";
        private string                      _strMongoDBName = "pgspindb";
        private readonly ILoggingAdapter    _logger         = Logging.GetLogger(Context);
        public SpinDBReader()
        {
            ReceiveAsync<ReadInfoCollectionRequest>     (onReadInfoCollection);
            ReceiveAsync<SelectSpinDataByIDRequest>     (onSelectSpinDataByID);
            ReceiveAsync<SelectPurchaseSpinRequest>     (onSelectPurchaseSpin);
            ReceiveAsync<SelectSpinTypeOddRangeRequest> (onSelectSpinTypeOddRange);
            ReceiveAsync<SelectSpinTypeRandomRequest>   (onSelectSpinTypeRandom);
            ReceiveAsync<SelectSelFreeMinStartRequest>  (onSelectSelFreeMinStart);
        }
        private async Task onReadInfoCollection(ReadInfoCollectionRequest request)
        {
            try
            {
                var dbClient    = new MongoClient(_strMongoDBURL);
                var spinDB      = dbClient.GetDatabase(_strMongoDBName);
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
        private async Task onSelectSpinDataByID(SelectSpinDataByIDRequest request)
        {
            try
            {
                var dbClient    = new MongoClient(_strMongoDBURL);
                var spinDB      = dbClient.GetDatabase(_strMongoDBName);
                var collection  = spinDB.GetCollection<BsonDocument>(request.GameName);
                var filter      = Builders<BsonDocument>.Filter.Eq("_id", request.ID);
                var spinData    = await collection.Find(filter).FirstAsync();
                Sender.Tell(spinData);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onSelectSpinDataByID {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onSelectPurchaseSpin(SelectPurchaseSpinRequest request)
        {
            try
            {
                var dbClient    = new MongoClient(_strMongoDBURL);
                var spinDB      = dbClient.GetDatabase(_strMongoDBName);
                var collection  = spinDB.GetCollection<BsonDocument>(request.GameName);

                BsonDocument document = null;
                if (request.SearchType == StartSpinSearchTypes.GENERAL)
                    document = await collection.AsQueryable().Where(x => x["spintype"] == 1).Sample(1).FirstAsync();
                else if (request.SearchType == StartSpinSearchTypes.SELFREE)
                    document = await collection.AsQueryable().Where(x => x["spintype"] == 100).Sample(1).FirstAsync();
                else if (request.SearchType == StartSpinSearchTypes.SPECIFIC)
                    document = await collection.AsQueryable().Where(x => x["puri"] == 0).Sample(1).FirstAsync();
                else if (request.SearchType == StartSpinSearchTypes.MULTISPECIFIC)
                    document = await collection.AsQueryable().Where(x => x["puri"] == request.SearchParam).Sample(1).FirstAsync();
                Sender.Tell(document);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onSelectPurchaseSpin {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onSelectSpinTypeOddRange(SelectSpinTypeOddRangeRequest request)
        {
            try
            {
                var dbClient   = new MongoClient(_strMongoDBURL);
                var spinDB     = dbClient.GetDatabase(_strMongoDBName);
                var collection = spinDB.GetCollection<BsonDocument>(request.GameName);

                BsonDocument document = null;
                if (request.Puri >= 0)
                    document = await collection.AsQueryable().Where(x => (x["puri"] == request.Puri && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd)).Sample(1).FirstAsync();
                else if (request.SpinType >= 0)
                    document = await collection.AsQueryable().Where(x => (x["spintype"] == request.SpinType && x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd)).Sample(1).FirstAsync();
                else
                    document = await collection.AsQueryable().Where(x => (x["odd"] >= request.MinOdd && x["odd"] <= request.MaxOdd)).Sample(1).FirstAsync(); ;

                Sender.Tell(document);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onSelectSpinTypeOddRange {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onSelectSelFreeMinStart(SelectSelFreeMinStartRequest request)
        {
            try
            {
                var dbClient = new MongoClient("mongodb://127.0.0.1");
                var spinDB = dbClient.GetDatabase(_strMongoDBName);
                var collection = spinDB.GetCollection<BsonDocument>(request.GameName);

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
        private async Task onSelectSpinTypeRandom(SelectSpinTypeRandomRequest request)
        {
            try
            {
                var dbClient = new MongoClient(_strMongoDBURL);
                var spinDB = dbClient.GetDatabase(_strMongoDBName);
                var collection = spinDB.GetCollection<BsonDocument>(request.GameName);

                BsonDocument document = await collection.AsQueryable().Where(x => (x["spintype"] == request.SpinType)).Sample(1).FirstAsync();
                Sender.Tell(document);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDBReader::onSelectSpinTypeRandom {0}", ex);
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
        public string GameName { get; set; }
        public int ID { get; set; }
        public SelectSpinDataByIDRequest(string strGameName, int id)
        {
            this.GameName = strGameName;
            this.ID = id;
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
        GENERAL = 0,
        SELFREE = 1,
        SPECIFIC = 2,
        MULTISPECIFIC = 3,
    }
    public class SelectPurchaseSpinRequest : IConsistentHashable
    {
        public string GameName { get; set; }
        public StartSpinSearchTypes SearchType { get; private set; }
        public int SearchParam { get; private set; }

        public SelectPurchaseSpinRequest(string strGameName, StartSpinSearchTypes searchType, int searchParam = 0)
        {
            GameName = strGameName;
            SearchType = searchType;
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
        public string   GameName { get; private set; }
        public int      SpinType { get; private set; }
        public double   MinOdd   { get; private set; }
        public double   MaxOdd   { get; private set; }
        public int      Puri     { get; private set; }
        public SelectSpinTypeOddRangeRequest(string strGameName, int spinType, double minOdd, double maxOdd, int puri = -1)
        {
            GameName = strGameName;
            SpinType = spinType;
            MinOdd   = minOdd;
            MaxOdd   = maxOdd;
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
    public class SelectSpinTypeRandomRequest : IConsistentHashable
    {
        public string   GameName { get; private set; }
        public int      SpinType { get; private set; }
        public SelectSpinTypeRandomRequest(string strGameName, int spinType)
        {
            GameName = strGameName;
            SpinType = spinType;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.GameName;
            }
        }
    }
    public class SelectSelFreeMinStartRequest : IConsistentHashable
    {
        public string GameName { get; private set; }
        public int    Puri     { get; private set; }
        public SelectSelFreeMinStartRequest(string strGameName, int puri = -1)
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
}
