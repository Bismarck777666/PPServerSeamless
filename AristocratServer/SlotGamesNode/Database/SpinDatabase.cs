using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using SlotGamesNode.GameLogics;

namespace SlotGamesNode.Database
{
    public class SpinDatabase : ReceiveActor
    {
        private readonly ILoggingAdapter _logger                = Logging.GetLogger(Context);
        private string                   _strGameName           = "";
        private string                  _strConnectionString    = null;
        public SpinDatabase(string strGameName)
        {
            _strGameName = strGameName;
            ReceiveAsync<string>                                        (onInitialize);
            ReceiveAsync<ReadSpinInfoRequest>                           (onReadSpinInfoRequest);
            ReceiveAsync<SelectSpinDataByIDRequest>                     (onSelectSpinDataByIDRequest);
            ReceiveAsync<SelectRangeSpinDataRequest>                    (onSelectRangeSpinData);

            //ReceiveAsync<ReadFreeOptSpinInfoRequest>                    (onReadFreeOptSpinInfoRequest);
            //ReceiveAsync<ReadSpinInfoPurEnabledRequest>                 (onReadSpinInfoPurchasedEnabled);
            //ReceiveAsync<ReadSpinInfoMultiPurEnabledRequest>            (onReadSpinInfoMultiPurEnabled);
            //ReceiveAsync<ReadFreeOptGroupedSpinInfoRequest>             (onReadFreeOptGroupedSpinInfoRequest);
            //ReceiveAsync<ReadPurchaseSamplesRequest>                    (onReadPurchaseFreeSamples);
            //ReceiveAsync<ReadFreeOptPurSpinInfoRequest>                 (onReadFreeOptPurSpecificSpinInfoRequest);
            //ReceiveAsync<ReadMultiFreeOptPurSpinInfoRequest>            (onReadMultiFreeOptPurSpecificSpinInfoRequest);
            //ReceiveAsync<ReadGambleOddsRequest>                         (onReadGambleOdds);

        }

        private async Task onInitialize(string _)
        {
            try
            {
                string appPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                string strFilePath = Path.Combine(appPath, string.Format("slotdata\\{0}.db", _strGameName));
                _strConnectionString = @"Data Source=" + strFilePath;

                if (!File.Exists(strFilePath))
                {
                    Sender.Tell(false);
                    return;
                }
                SQLiteConnection connection = new SQLiteConnection(_strConnectionString);
                await connection.OpenAsync();
                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in SpinDatabase::onInitialize {0}", ex.ToString());
                Sender.Tell(false);
            }
        }

        //private async Task onReadFreeOptGroupedSpinInfoRequest(ReadFreeOptGroupedSpinInfoRequest request)
        //{
        //    try
        //    {
        //        using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
        //        {
        //            await connection.OpenAsync();
        //            string strCommand = "SELECT * FROM info";
        //            SQLiteCommand command = new SQLiteCommand(strCommand, connection);

        //            int normalMaxID = 0;
        //            double defaultBet = 0.0;

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    normalMaxID = (int)(long)reader["normalmaxid"];
        //                    defaultBet  = Math.Round((double)reader["defaultbet"], 2);
        //                }
        //                else
        //                {
        //                    Sender.Tell(null);
        //                    return;
        //                }
        //            }

        //            List<GroupedSpinBaseData> spinBaseDatas = new List<GroupedSpinBaseData>();
        //            strCommand = "SELECT id, groupid, islast, spintype, odd FROM spins";
        //            command = new SQLiteCommand(strCommand, connection);
        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    int     id          = (int)(long)reader["id"];
        //                    byte    spinType    = (byte)(long)reader["spintype"];
        //                    double  odd         = Math.Round((double)reader["odd"], 2);
        //                    int     group       = (int) (long) reader["groupid"];
        //                    bool    isLast      = ((int)(long)reader["islast"] == 1);

        //                    spinBaseDatas.Add(new GroupedSpinBaseData(id, spinType, odd, group, isLast));
        //                }
        //            }
        //            List<GroupSequence> sequences = new List<GroupSequence>();
        //            strCommand = "SELECT sequence, count FROM sequence";
        //            command = new SQLiteCommand(strCommand, connection);
        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    string strSeq = (string) reader["sequence"];
        //                    int count = (int)(long)reader["count"];
        //                    sequences.Add(new GroupSequence(strSeq, count));
        //                }
        //            }
        //            Sender.Tell(new ReadFreeOptGroupedSpinInfoResponse(normalMaxID, defaultBet, spinBaseDatas, sequences));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("Exception has been occurred in SpinDatabase::onReadSpinInfoRequest {0}", ex);
        //        Sender.Tell(null);
        //    }

        //}
        //private async Task onReadFreeOptSpinInfoRequest(ReadFreeOptSpinInfoRequest request)
        //{
        //    try
        //    {
        //        using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
        //        {
        //            await connection.OpenAsync();
        //            string strCommand = "SELECT * FROM info";
        //            SQLiteCommand command = new SQLiteCommand(strCommand, connection);

        //            int     normalMaxID = 0;
        //            double  defaultBet  = 0.0;

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    normalMaxID = (int)(long)reader["normalmaxid"];
        //                    defaultBet  = Math.Round((double)reader["defaultbet"], 2);
        //                }
        //                else
        //                {
        //                    Sender.Tell(null);
        //                    return;
        //                }
        //            }

        //            List<SpinBaseData> spinBaseDatas = new List<SpinBaseData>();
        //            strCommand  = "SELECT id, spintype, odd, minrate, allfreewinrate FROM spins";
        //            command     = new SQLiteCommand(strCommand, connection);
        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    int     id          = (int)(long)reader["id"];
        //                    byte    spinType    = (byte)(long)reader["spintype"];
        //                    double  odd         = Math.Round((double)reader["odd"], 2);

        //                    if (spinType != 100)
        //                    {
        //                        spinBaseDatas.Add(new SpinBaseData(id, spinType, line, odd));
        //                    }
        //                    else
        //                    {
        //                        double minRate = 0.0;
        //                        if (!(reader["minrate"] is DBNull))
        //                            minRate = (double)reader["minrate"];
        //                        spinBaseDatas.Add(new Start2ndSpinBaseData(id, spinType, odd, minRate, (double) reader["allfreewinrate"]));
        //                    }
        //                }
        //            }
        //            Sender.Tell(new ReadSpinInfoResponse(normalMaxID, defaultBet, spinBaseDatas));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("Exception has been occurred in SpinDatabase::onReadSpinInfoRequest {0}", ex);
        //        Sender.Tell(null);
        //    }

        //}
        private async Task onReadSpinInfoRequest(ReadSpinInfoRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    string strCommand = "SELECT * FROM info";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);

                    int normalMaxID = 0;
                    double defaultBet = 0.0;
                    double freeSpinRate = 0.0;
                    int normalMaxID2 = 0;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            normalMaxID = (int)(long)reader["normalmaxid"];
                            defaultBet = Math.Round((double)reader["defaultbet"], 2);

                            if (request.SpinDataType == SpinDataTypes.MultiBetType)
                                normalMaxID2 = (int)(long)reader["normalmaxid2"];

                            var fieldNames = Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i)).ToArray();
                            if (fieldNames.Contains("freespinrate"))
                                freeSpinRate = (double)reader["freespinrate"];


                        }
                        else
                        {
                            Sender.Tell(null);
                            return;
                        }
                    }

                    List<SpinBaseData> spinBaseDatas = new List<SpinBaseData>();
                    strCommand = "SELECT id, spintype, line, odd FROM spins";

                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = (int)(long)reader["id"];
                            byte spinType = (byte)(long)reader["spintype"];
                            double odd = Math.Round((double)reader["odd"], 2);
                            int line = (int)(long)reader["line"];

                            spinBaseDatas.Add(new SpinBaseData(id, spinType, line, odd));
                        }
                    }
                    if (request.SpinDataType == SpinDataTypes.MultiBetType)
                        Sender.Tell(new ReadSpinInfoResponseWithBetType(normalMaxID, normalMaxID2, defaultBet, spinBaseDatas));
                    else
                        Sender.Tell(new ReadSpinInfoResponse(normalMaxID, defaultBet, spinBaseDatas));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onReadSpinInfoRequest {0}", ex);
                Sender.Tell(null);
            }
        }

        private async Task onSelectFreeRangeSpinData(SelectFreeRangeSpinDataRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = string.Format("SELECT * FROM spins WHERE (spintype=100 and ranges LIKE '%{0}%') ORDER BY RANDOM() LIMIT 1", request.RangeID);
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string strSpinData                      = (string)reader["data"];
                            List<string> spinStrings                = new List<string>(strSpinData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                            BasePPSlotStartSpinData startSpinData   = new BasePPSlotStartSpinData();
                            startSpinData.StartOdd                  = (double)reader["realodd"];
                            startSpinData.SpinStrings               = spinStrings;
                            startSpinData.FreeSpinGroup             = (int)(long)reader["freespintype"];
                            Sender.Tell(startSpinData);
                            return;
                        }
                    }
                    Sender.Tell(null);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onSelectFreeRangeSpinData {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onSelectNormalRangeSpinData(SelectRangeSpinDataRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = string.Format("SELECT * FROM spins WHERE (spintype=0 and odd >=@minodd and odd <= @maxodd) or (spintype=100 and ranges LIKE '%{0}%') ORDER BY RANDOM() LIMIT 1", request.RangeID);
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                    command.Parameters.AddWithValue("@minodd", request.MinOdd);
                    command.Parameters.AddWithValue("@maxodd", request.MaxOdd);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int spinType = (int)(long)reader["spintype"];
                            string strSpinData = (string)reader["data"];
                            List<string> spinStrings = new List<string>(strSpinData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                            if (spinType == 0)
                            {
                                Sender.Tell(new BasePPSlotSpinData(spinType, (double)reader["odd"], spinStrings));
                            }
                            else
                            {
                                BasePPSlotStartSpinData startSpinData = new BasePPSlotStartSpinData();
                                startSpinData.StartOdd = (double)reader["realodd"];
                                startSpinData.SpinStrings = spinStrings;
                                startSpinData.FreeSpinGroup = (int)(long)reader["freespintype"];
                                Sender.Tell(startSpinData);
                            }
                            return;
                        }
                    }
                    Sender.Tell(null);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onSelectNormalRangeSpinData {0}", ex);
                Sender.Tell(null);
            }
        }
        private async Task onSelectRangeSpinData(SelectRangeSpinDataRequest request)
        {
            if (request is SelectGroupRangeSpinDataRequest)
                await onSelectGroupRangeSpinData(request as SelectGroupRangeSpinDataRequest);
            else if(request is SelectFreeRangeSpinDataRequest)
                await onSelectFreeRangeSpinData(request as SelectFreeRangeSpinDataRequest);
            else
                await onSelectNormalRangeSpinData(request);            
        }

        private async Task onSelectGroupRangeSpinData(SelectGroupRangeSpinDataRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = "SELECT id FROM spins WHERE groupid=@groupid and islast=@islast and spintype=0 and odd >=@minodd and odd <= @maxodd";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                    command.Parameters.AddWithValue("@minodd",  request.MinOdd);
                    command.Parameters.AddWithValue("@maxodd",  request.MaxOdd);
                    command.Parameters.AddWithValue("@groupid", request.Group);
                    command.Parameters.AddWithValue("@islast",  request.IsLast ? 1 : 0);

                    List<int> idList = new List<int>();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            idList.Add((int)(long)reader["id"]);
                        }
                    }
                    strCommand = string.Format("SELECT id FROM spins WHERE groupid=@groupid and islast=@islast and spintype=100 and ranges LIKE '%{0}%'", request.RangeID);
                    command = new SQLiteCommand(strCommand, connection);
                    command.Parameters.AddWithValue("@groupid", request.Group);
                    command.Parameters.AddWithValue("@islast",  request.IsLast ? 1 : 0);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            idList.Add((int)(long)reader["id"]);
                        }
                    }
                    if (idList.Count == 0)
                    {
                        Sender.Tell(null);
                        return;
                    }
                    int id = idList[PCGSharp.Pcg.Default.Next(0, idList.Count)];

                    strCommand  = "SELECT * FROM spins WHERE id=@id";
                    command     = new SQLiteCommand(strCommand, connection);
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int         spinType    = (int)(long)reader["spintype"];
                            string      strSpinData = (string)reader["data"];
                            List<string> spinStrings = new List<string>(strSpinData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                            if (spinType == 0)
                            {
                                Sender.Tell(new BasePPSlotSpinData(spinType, (double)reader["odd"], spinStrings));
                            }
                            else
                            {
                                BasePPSlotStartSpinData startSpinData = new BasePPSlotStartSpinData();
                                startSpinData.StartOdd      = (double)reader["realodd"];
                                startSpinData.SpinStrings   = spinStrings;
                                startSpinData.FreeSpinGroup = (int)(long)reader["freespintype"];
                                Sender.Tell(startSpinData);
                            }
                            return;
                        }
                    }
                    Sender.Tell(null);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onSelectGroupRangeSpinData {0}", ex);
                Sender.Tell(null);
            }
        }

        private async Task onSelectSpinDataByIDRequest(SelectSpinDataByIDRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = "SELECT * FROM spins WHERE id=@id"; ;
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                    command.Parameters.AddWithValue("@id", request.ID);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int             spinType        = (int)(long)reader["spintype"];
                            string          strSpinData     = (string)reader["data"];
                            double          spinOdd         = (double)reader["odd"];
                            if (request is SelectGroupSpinDataByIDRequest)
                            {
                                string strSpinTypesData = (string)reader["spintypes"];
                                string strOddsData = (string)reader["odds"];

                                string[] strSpinDatas = strSpinData.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
                                string[] strSpinTypes = strSpinTypesData.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                string[] strSpinOdds = strOddsData.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                                List<BasePPSlotSpinData> spinDatas = new List<BasePPSlotSpinData>();
                                for (int i = 0; i < 10; i++)
                                {
                                    List<string> spinResponses = new List<string>(strSpinDatas[i].Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                                    int childSpinType = int.Parse(strSpinTypes[i]);
                                    double childSpinOdd = double.Parse(strSpinOdds[i]);

                                    spinDatas.Add(new BasePPSlotSpinData(childSpinType, childSpinOdd, spinResponses));
                                }
                                Sender.Tell(new GroupedSpinData(spinDatas, spinOdd));
                            }
                            else
                            {
                                List<string> spinResponses = new List<string>(strSpinData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                                BasePPSlotSpinData spinData = null;
                                if (spinType == 100)
                                {
                                    BasePPSlotStartSpinData startSpinData = new BasePPSlotStartSpinData();
                                    startSpinData.StartOdd = (double)reader["realodd"];
                                    startSpinData.FreeSpinGroup = (int)(long)reader["freespintype"];
                                    startSpinData.SpinStrings = spinResponses;
                                    spinData = startSpinData;
                                }
                                else
                                {
                                    spinData = new BasePPSlotSpinData(spinType, spinOdd, spinResponses);
                                }
                                Sender.Tell(spinData);
                            }
                            return;
                        }
                    }
                    Sender.Tell(null);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onSelectSpinDataByIDRequest {0}", ex);
                Sender.Tell(null);
            }
        }
    }
  
    
    public class ReadFreeOptSpinInfoRequest
    {

    }
    public class ReadFreeOptGroupedSpinInfoRequest
    {

    }

    public enum SpinDataTypes
    {
        Normal              = 0,
        SelFreeSpin         = 1,
        MultiPurFreeSpin    = 2,
        NormalWithFloor     = 3,
        MultiBetType        = 4,
        SkipPuriEnabled     = 5,
        GroupedSpinData     = 6,
        SkipPurEnabled      = 7,
        SkipPuri1Enabled    = 8,
        NormalWithLines     = 9,

    }
    public class ReadSpinInfoRequest    
    {
        public SpinDataTypes SpinDataType { get; private set; }
        public ReadSpinInfoRequest(SpinDataTypes spinDataType)
        {
            this.SpinDataType = spinDataType;
        }
    }
    public class ReadSpinInfoPurEnabledRequest
    {

    }
    public class ReadSpinInfoMultiPurEnabledRequest
    {
        public int PurIndex { get; private set; }

        public ReadSpinInfoMultiPurEnabledRequest(int puriIndex)
        {
            this.PurIndex = puriIndex;
        }
    }
    public class ReadPurchaseSamplesRequest
    {

    }
    
    public class ReadSpinInfoResponse
    {
     
        public int                  NormalMaxID     { get; private set; }
        public double               DefaultBet      { get; private set; }
        public List<SpinBaseData>   SpinBaseDatas   { get; private set; }
        public ReadSpinInfoResponse(int normalMaxId, double defaultBet, List<SpinBaseData> spinBaseDatas)
        {
            this.NormalMaxID    = normalMaxId;
            this.DefaultBet     = defaultBet;
            this.SpinBaseDatas  = spinBaseDatas;
        }
    }
    public class ReadFreeOptGroupedSpinInfoResponse
    {

        public int                          NormalMaxID     { get; private set; }
        public double                       DefaultBet      { get; private set; }
        public List<GroupedSpinBaseData>    SpinBaseDatas   { get; private set; }
        public List<GroupSequence>          Sequences       { get; private set; }
        public ReadFreeOptGroupedSpinInfoResponse(int normalMaxId, double defaultBet, List<GroupedSpinBaseData> spinBaseDatas, List<GroupSequence> sequences)
        {
            this.NormalMaxID    = normalMaxId;
            this.DefaultBet     = defaultBet;
            this.SpinBaseDatas  = spinBaseDatas;
            this.Sequences      = sequences;
        }
    }
    public class SelectSpinDataByIDRequest
    {
        public int ID { get; set; }
        public SelectSpinDataByIDRequest(int id)
        {
            this.ID = id;
        }
    }
    public class SelectRangeSpinDataRequest
    {
        public double MinOdd    { get; set; }
        public double MaxOdd    { get; set; }
        public int    RangeID   { get; set; }

        public SelectRangeSpinDataRequest(double minOdd, double maxOdd, int rangeID)
        {
            this.MinOdd     = minOdd;
            this.MaxOdd     = maxOdd;
            this.RangeID    = rangeID;
        }
    }
    public class SelectFreeRangeSpinDataRequest : SelectRangeSpinDataRequest
    {
        public SelectFreeRangeSpinDataRequest(double minOdd, double maxOdd, int rangeID) : base(minOdd, maxOdd, rangeID)
        {
           
        }
    }
    public class SelectGroupRangeSpinDataRequest : SelectRangeSpinDataRequest
    {
        public int      Group   { get; set; }
        public bool     IsLast  { get; set; }
        public SelectGroupRangeSpinDataRequest(double minOdd, double maxOdd, int rangeID, int group, bool isLast) : 
            base(minOdd, maxOdd, rangeID)
        {
            this.Group  = group;
            this.IsLast = isLast;
        }
    }
    public class SpinBaseData
    {
        public int      ID          { get; set; }
        public byte     SpinType    { get; set; }
        public double   Odd         { get; set; }
        public int      Line        { get; set; }
        public SpinBaseData(int id, byte spinType, int line, double odd)
        {
            this.ID         = id;
            this.SpinType   = spinType;
            this.Odd        = odd;
            this.Line       = line;
        }
    }
    public class SpinBaseDataWithFloor : SpinBaseData
    {
        public int      Floor       { get; set; }
        public SpinBaseDataWithFloor(int id, byte spinType, int line, double odd, int floor) : base(id, spinType, line, odd)
        {
            this.Floor = floor;
        }
    }
    public class GroupedSpinBaseData : SpinBaseData
    {
        public int  Group   { get; set; }
        public bool IsLast  { get; set; }

        public GroupedSpinBaseData(int id, byte spinType, int line, double odd, int group, bool isLast) : base(id, spinType, line, odd)
        {
            this.Group  = group;
            this.IsLast = isLast;
        }
    }
    public class StartSpinBaseData : SpinBaseData
    {
        public double MinRate { get; set; }
        public StartSpinBaseData(int id, byte spinType, int line, double odd, double minRate) : base(id, spinType, line, odd)
        {
            this.MinRate = minRate;
        }
    }
    public class NormalFreeSpinWithType : SpinBaseData
    {
        public int FreeSpinType { get; set; }
        public NormalFreeSpinWithType(int id, byte spinType, int line, double odd, int freeSpinType) : base(id, spinType, line, odd)
        {
            this.FreeSpinType = freeSpinType;
        }
    }
    public class Start2ndSpinBaseData : SpinBaseData
    {
        public double MinFreeRate { get; set; }
        public double AllFreeRate { get; set; }
        public Start2ndSpinBaseData(int id, byte spinType, int line, double odd, double minRate, double allFreeRate) : base(id, spinType, line, odd)
        {
            this.MinFreeRate = minRate;
            this.AllFreeRate = allFreeRate;
        }
    }
    public class MultiPurSpinData : SpinBaseData
    {
        public int Puri { get; set; }
        public MultiPurSpinData(int id, byte spinType, int line, double odd, int puri) :
            base(id, spinType, line, odd)
        {
            this.Puri = puri;
        }
    }
    public class MultiPurStartSpinData : Start2ndSpinBaseData
    {
        public int Puri { get; set; }
        public MultiPurStartSpinData(int id, byte spinType, int line, double odd, double minRate, double allFreeRate, int puri) : base(id, spinType, line, odd, minRate, allFreeRate)
        {
            this.Puri = puri;
        }
    }
    public class GroupSequence
    {
        public string Sequence  { get; set; }
        public int    Count     { get; set; }

        public GroupSequence(string strSeq, int count)
        {
            this.Sequence   = strSeq;
            this.Count      = count;
        }
    }
    public class ReadFreeOptPurSpinInfoRequest
    {

    }
    public class ReadSpinInfoResponseWithBetType : ReadSpinInfoResponse
    {
        public int NormalMaxID2 { get; private set; }
        public ReadSpinInfoResponseWithBetType(int normalMaxId, int normalMaxId2, double defaultBet, List<SpinBaseData> spinBaseDatas) :
            base(normalMaxId, defaultBet, spinBaseDatas)
        {
            NormalMaxID2 = normalMaxId2;
        }
    }
    public class PurEnabledStartSpinData : Start2ndSpinBaseData
    {
        public bool PurEnabled { get; set; }
        public PurEnabledStartSpinData(int id, byte spinType, int line, double odd, double minRate, double allFreeRate, bool purEnabled) : base(id, spinType, line, odd, minRate, allFreeRate)
        {
            this.PurEnabled = purEnabled;
        }
    }
    public class SpinBaseDataWithBetType : SpinBaseData
    {
        public int BetType { get; set; }
        public SpinBaseDataWithBetType(int id, byte spinType, int line, double odd, int betType) : base(id, spinType, line, odd)
        {
            this.BetType = betType;
        }
    }
    public class ReadMultiFreeOptPurSpinInfoRequest
    {

    }
    public class GambleOdd
    {
        public double MinOdd { get; set; }
        public double MaxOdd { get; set; }
        public double RealOdd { get; set; }
        public double Percent { get; set; }
    }
    public class GroupedSpinData
    {
        public List<BasePPSlotSpinData> SpinDatas { get; private set; }
        public double GroupOdd { get; private set; }

        public GroupedSpinData(List<BasePPSlotSpinData> spinDatas, double groupOdd)
        {
            SpinDatas = spinDatas;
            GroupOdd = groupOdd;
        }
    }
    public class SelectGroupSpinDataByIDRequest : SelectSpinDataByIDRequest
    {
        public SelectGroupSpinDataByIDRequest(int id) : base(id)
        {
        }
    }
    public class ReadGambleOddsRequest
    {

    }

}
