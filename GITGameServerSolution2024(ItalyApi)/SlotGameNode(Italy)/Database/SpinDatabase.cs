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
        private string                  _providerName           = "";
        private string                  _strGameName            = "";
        private string                  _strConnectionString    = null;
        public SpinDatabase(string providerName, string strGameName)
        {
            _providerName   = providerName;
            _strGameName    = strGameName;
            ReceiveAsync<string>                                        (onInitialize);
            ReceiveAsync<ReadSpinInfoRequest>                           (onReadSpinInfoRequest);
            ReceiveAsync<SelectSpinDataByIDRequest>                     (onSelectSpinDataByIDRequest);
            ReceiveAsync<SelectRangeSpinDataRequest>                    (onSelectRangeSpinData);

            ReceiveAsync<ReadFreeOptSpinInfoRequest>                    (onReadFreeOptSpinInfoRequest);
            ReceiveAsync<ReadSpinInfoPurEnabledRequest>                 (onReadSpinInfoPurchasedEnabled);
            ReceiveAsync<ReadSpinInfoMultiPurEnabledRequest>            (onReadSpinInfoMultiPurEnabled);
            ReceiveAsync<ReadFreeOptGroupedSpinInfoRequest>             (onReadFreeOptGroupedSpinInfoRequest);
            ReceiveAsync<ReadPurchaseSamplesRequest>                    (onReadPurchaseFreeSamples);
            ReceiveAsync<ReadSpinInfoMutiBaseAnteRequest>               (onReadSpinInfoMultiBaseAnte);
        }

        private async Task onInitialize(string _)
        {
            try
            {
                string appPath          = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                string strFilePath      = Path.Combine(appPath, string.Format("slotdata\\{0}\\{1}.db", _providerName,_strGameName));
                _strConnectionString    = @"Data Source=" + strFilePath;

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

        private async Task onReadFreeOptGroupedSpinInfoRequest(ReadFreeOptGroupedSpinInfoRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    string strCommand       = "SELECT * FROM info";
                    SQLiteCommand command   = new SQLiteCommand(strCommand, connection);

                    int normalMaxID     = 0;
                    double defaultBet   = 0.0;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            normalMaxID = (int)(long)reader["normalmaxid"];
                            defaultBet  = Math.Round((double)reader["defaultbet"], 2);
                        }
                        else
                        {
                            Sender.Tell(null);
                            return;
                        }
                    }

                    List<GroupedSpinBaseData> spinBaseDatas = new List<GroupedSpinBaseData>();
                    strCommand = "SELECT id, groupid, islast, spintype, odd FROM spins";
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int     id          = (int)(long)reader["id"];
                            byte    spinType    = (byte)(long)reader["spintype"];
                            double  odd         = Math.Round((double)reader["odd"], 2);
                            int     group       = (int) (long) reader["groupid"];
                            bool    isLast      = ((int)(long)reader["islast"] == 1);

                            spinBaseDatas.Add(new GroupedSpinBaseData(id, spinType, odd, group, isLast));
                        }
                    }
                    
                    List<GroupSequence> sequences = new List<GroupSequence>();
                    strCommand = "SELECT sequence, count FROM sequence";
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strSeq = (string) reader["sequence"];
                            int count = (int)(long)reader["count"];
                            sequences.Add(new GroupSequence(strSeq, count));
                        }
                    }
                    
                    Sender.Tell(new ReadFreeOptGroupedSpinInfoResponse(normalMaxID, defaultBet, spinBaseDatas, sequences));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onReadSpinInfoRequest {0}", ex);
                Sender.Tell(null);
            }
        }
        
        private async Task onReadFreeOptSpinInfoRequest(ReadFreeOptSpinInfoRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    string strCommand       = "SELECT * FROM info";
                    SQLiteCommand command   = new SQLiteCommand(strCommand, connection);

                    int normalMaxID     = 0;
                    double defaultBet   = 0.0;
                    double freeSpinRate = 0.0;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            normalMaxID = (int)(long)reader["normalmaxid"];
                            defaultBet = Math.Round((double)reader["defaultbet"], 2);
                            if (reader.FieldCount == 3)
                                freeSpinRate = (double)reader["freespinrate"];
                        }
                        else
                        {
                            Sender.Tell(null);
                            return;
                        }
                    }

                    List<SpinBaseData> spinBaseDatas = new List<SpinBaseData>();
                    strCommand  = "SELECT id, spintype, odd, minrate, allfreewinrate FROM spins";
                    command     = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id          = (int)(long)reader["id"];
                            byte spinType   = (byte)(long)reader["spintype"];
                            double odd      = Math.Round((double)reader["odd"], 2);

                            if (spinType != 100)
                            {
                                spinBaseDatas.Add(new SpinBaseData(id, spinType, odd));
                            }
                            else
                            {
                                double minRate = 0.0;
                                if (!(reader["minrate"] is DBNull))
                                    minRate = (double)reader["minrate"];
                                spinBaseDatas.Add(new Start2ndSpinBaseData(id, spinType, odd, minRate, (double) reader["allfreewinrate"]));
                            }
                        }
                    }
                    
                    Sender.Tell(new ReadSpinInfoResponse(normalMaxID, defaultBet, spinBaseDatas));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onReadSpinInfoRequest {0}", ex);
                Sender.Tell(null);
            }

        }
        
        private async Task onReadSpinInfoRequest(ReadSpinInfoRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    string              strCommand  = "SELECT * FROM info";
                    SQLiteCommand       command     = new SQLiteCommand(strCommand, connection);

                    int     normalMaxID     = 0;
                    double  defaultBet      = 0.0;
                    double  freeSpinRate    = 0.0;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            normalMaxID = (int) (long)          reader["normalmaxid"];
                            defaultBet  = Math.Round((double)   reader["defaultbet"], 2);
                            if(reader.FieldCount == 3)
                                freeSpinRate = (double)reader["freespinrate"];
                        }
                        else
                        {
                            Sender.Tell(null);
                            return;
                        }
                    }

                    List<SpinBaseData> spinBaseDatas = new List<SpinBaseData>();
                    if(request.SpinDataType == SpinDataTypes.SelFreeSpin)
                        strCommand  = "SELECT id, spintype, odd, minrate FROM spins";
                    else if(request.SpinDataType == SpinDataTypes.Normal)
                        strCommand = "SELECT id, spintype, odd FROM spins";
                    else if(request.SpinDataType == SpinDataTypes.MultiPurFreeSpin)
                        strCommand = "SELECT id, spintype, odd, freespintype FROM spins";
                    else if(request.SpinDataType == SpinDataTypes.NormalWithFloor)
                        strCommand = "SELECT id, spintype, odd, beginfloor FROM spins";
                    else if (request.SpinDataType == SpinDataTypes.MultiBase)
                        strCommand = "SELECT id, spintype, odd, extra FROM spins";
                    else if (request.SpinDataType == SpinDataTypes.SelFreeMultiBase)
                        strCommand = "SELECT id, spintype, odd, minrate, extra FROM spins";

                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            int     id          = (int)(long)reader["id"];
                            byte    spinType    = (byte)(long)reader["spintype"];
                            double  odd         = Math.Round((double)reader["odd"], 2);

                            if(request.SpinDataType == SpinDataTypes.MultiPurFreeSpin)
                            {
                                if(spinType == 0)
                                    spinBaseDatas.Add(new SpinBaseData(id, spinType, odd));
                                else
                                    spinBaseDatas.Add(new NormalFreeSpinWithType(id, spinType, odd, (int) (long) reader["freespintype"]));
                            }
                            else if (request.SpinDataType == SpinDataTypes.NormalWithFloor)
                            {
                                spinBaseDatas.Add(new SpinBaseDataWithFloor(id, spinType, odd, (int)(long)reader["beginfloor"]));
                            }
                            else if (request.SpinDataType == SpinDataTypes.MultiBase)
                            {
                                spinBaseDatas.Add(new SpinExtraData(id, spinType, odd, (int)(long)reader["extra"]));
                            }
                            else if (request.SpinDataType == SpinDataTypes.Normal/* || spinType != 100*/)
                            {
                                spinBaseDatas.Add(new SpinBaseData(id, spinType, odd));
                            }
                            else if (request.SpinDataType == SpinDataTypes.SelFreeSpin)
                            {
                                if (reader["minrate"] is DBNull || ((double)reader["minrate"] == 0.0))
                                {
                                    spinBaseDatas.Add(new SpinBaseData(id, spinType, odd));
                                }
                                else
                                {
                                    spinBaseDatas.Add(new StartSpinBaseData(id, spinType, odd, (double)reader["minrate"]));
                                }
                            }
                            else if (request.SpinDataType == SpinDataTypes.SelFreeMultiBase)
                            {
                                int extra = (int)(long)reader["extra"];
                                if (reader["minrate"] is DBNull || ((double)reader["minrate"] == 0.0))
                                {
                                    spinBaseDatas.Add(new SpinExtraData(id, spinType, odd, extra));
                                }
                                else
                                {
                                    spinBaseDatas.Add(new StartSpinExtraBaseData(id, spinType, odd, (double)reader["minrate"], extra));
                                }
                            }
                        }
                    }
                    
                    Sender.Tell(new ReadSpinInfoResponse(normalMaxID, defaultBet, spinBaseDatas));
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onReadSpinInfoRequest {0}", ex);
                Sender.Tell(null);
            }
        }
        
        private async Task onReadPurchaseFreeSamples(ReadPurchaseSamplesRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    List<string> sampleDatas = new List<string>();
                    string strCommand       = "SELECT data FROM pursamples";
                    SQLiteCommand command   = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            sampleDatas.Add((string) reader["data"]);
                        }
                    }
                    Sender.Tell(sampleDatas);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onReadSpinInfoPurchasedEnabled {0}", ex);
                Sender.Tell(null);
            }
        }
        
        private async Task onReadSpinInfoMultiPurEnabled(ReadSpinInfoMultiPurEnabledRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    List<SpinBaseData> spinBaseDatas = new List<SpinBaseData>();
                    string strCommand       = "SELECT id, odd FROM spins WHERE spintype=1 and puri=@puri";
                    SQLiteCommand command   = new SQLiteCommand(strCommand, connection);
                    command.Parameters.AddWithValue("@puri", request.PurIndex);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = (int)(long)reader["id"];
                            double odd = Math.Round((double)reader["odd"], 2);
                            spinBaseDatas.Add(new SpinBaseData(id, 1, odd));
                        }
                    }
                    Sender.Tell(spinBaseDatas);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onReadSpinInfoMultiPurEnabled {0}", ex);
                Sender.Tell(null);
            }
        }
        
        private async Task onReadSpinInfoPurchasedEnabled(ReadSpinInfoPurEnabledRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();                   
                    List<SpinBaseData> spinBaseDatas = new List<SpinBaseData>();
                    string strCommand       = "SELECT id, odd FROM spins WHERE spintype=1 and purenabled=1";
                    SQLiteCommand command   = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int     id = (int)(long)reader["id"];
                            double odd = Math.Round((double)reader["odd"], 2);
                            spinBaseDatas.Add(new SpinBaseData(id, 1, odd));
                        }
                    }
                    Sender.Tell(spinBaseDatas);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onReadSpinInfoPurchasedEnabled {0}", ex);
                Sender.Tell(null);
            }
        }

        private async Task onReadSpinInfoMultiBaseAnte(ReadSpinInfoMutiBaseAnteRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = "SELECT * FROM info";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);

                    int normalMaxID     = 0;
                    double defaultBet   = 0.0;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            normalMaxID = (int)(long)reader["normalmaxid"];
                            defaultBet  = Math.Round((double)reader["defaultbet"], 2);
                        }
                        else
                        {
                            Sender.Tell(null);
                            return;
                        }
                    }

                    List<SpinBaseData> spinBaseDatas = new List<SpinBaseData>();
                    strCommand = "SELECT id, spintype, odd, extra,isante FROM spins";
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id          = (int)(long)reader["id"];
                            int spintype    = (int)(long)reader["spintype"];
                            double odd      = Math.Round((double)reader["odd"], 2);
                            int extra       = (int)(long)reader["extra"];
                            int isante      = (int)(long)reader["isante"];
                            spinBaseDatas.Add(new SpinExtraAnteData(id, (byte)spintype, odd, extra, isante));
                        }
                    }
                    Sender.Tell(new ReadSpinInfoResponse(normalMaxID, defaultBet, spinBaseDatas));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpinDatabase::onReadSpinInfoMultiBaseAnte {0}", ex);
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
                            List<string>    spinResponses   = new List<string>(strSpinData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));

                            BasePPSlotSpinData spinData = null;
                            if (spinType == 100)
                            {
                                BasePPSlotStartSpinData startSpinData = new BasePPSlotStartSpinData();
                                startSpinData.StartOdd      = (double)reader["realodd"];
                                startSpinData.FreeSpinGroup = (int)(long)reader["freespintype"];
                                startSpinData.SpinStrings   = spinResponses;
                                spinData = startSpinData;
                            }
                            else
                            {
                                if(request is SelectCongoSpinDataByIDRequest)
                                {
                                    bool isSpecial = ((long)reader["reeltype"] == 1);
                                    spinData = new CongoCashSpinData(spinType, spinOdd, spinResponses, isSpecial);
                                }    
                                else
                                {
                                spinData = new BasePPSlotSpinData(spinType, spinOdd, spinResponses);
                            }
                            }
                            Sender.Tell(spinData);
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
        MultiBase           = 4,
        SelFreeMultiBase    = 5,
    }
    
    public class ReadSpinInfoRequest    
    {
        public SpinDataTypes SpinDataType { get; private set; }
        public ReadSpinInfoRequest(SpinDataTypes spinDataType)
        {
            this.SpinDataType = spinDataType;
        }
    }

    public class ReadSpinInfoMutiBaseAnteRequest
    {
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

    public class SpinExtraData: SpinBaseData
    {
        public int      Extra       { get; set; }
        public SpinExtraData(int id, byte spinType, double odd,int extra) : base(id,spinType,odd)
        {
            this.ID         = id;
            this.SpinType   = spinType;
            this.Odd        = odd;
            this.Extra      = extra;
        }
    }

    public class SpinExtraAnteData : SpinExtraData
    {
        public int IsAnte { get; set; }
        public SpinExtraAnteData(int id, byte spinType, double odd, int extra, int isAnte) : base(id, spinType, odd, extra)
        {
            this.ID         = id;
            this.SpinType   = spinType;
            this.Odd        = odd;
            this.Extra      = extra;
            this.IsAnte     = isAnte;
        }
    }

    public class StartSpinExtraBaseData : SpinExtraData
    {
        public double MinRate { get; set; }
        public StartSpinExtraBaseData(int id, byte spinType, double odd, double minRate, int extra) : base(id, spinType, odd, extra)
        {
            this.MinRate = minRate;
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
    public class SelectCongoSpinDataByIDRequest : SelectSpinDataByIDRequest
    {
        public SelectCongoSpinDataByIDRequest(int id) : base(id)
        {
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
        public SpinBaseData(int id, byte spinType, double odd)
        {
            this.ID         = id;
            this.SpinType   = spinType;
            this.Odd        = odd;
        }
    }
    public class SpinBaseDataWithFloor : SpinBaseData
    {
        public int      Floor       { get; set; }
        public SpinBaseDataWithFloor(int id, byte spinType, double odd, int floor) : base(id, spinType, odd)
        {
            this.Floor = floor;
        }
    }
    public class GroupedSpinBaseData : SpinBaseData
    {
        public int  Group   { get; set; }
        public bool IsLast  { get; set; }

        public GroupedSpinBaseData(int id, byte spinType, double odd, int group, bool isLast) : base(id, spinType, odd)
        {
            this.Group  = group;
            this.IsLast = isLast;
        }
    }
    public class StartSpinBaseData : SpinBaseData
    {
        public double MinRate { get; set; }
        public StartSpinBaseData(int id, byte spinType, double odd, double minRate) : base(id, spinType, odd)
        {
            this.MinRate = minRate;
        }
    }
    public class NormalFreeSpinWithType : SpinBaseData
    {
        public int FreeSpinType { get; set; }
        public NormalFreeSpinWithType(int id, byte spinType, double odd, int freeSpinType) : base(id, spinType, odd)
        {
            this.FreeSpinType = freeSpinType;
        }
    }
    public class Start2ndSpinBaseData : SpinBaseData
    {
        public double MinFreeRate { get; set; }
        public double AllFreeRate { get; set; }
        public Start2ndSpinBaseData(int id, byte spinType, double odd, double minRate, double allFreeRate) : base(id, spinType, odd)
        {
            this.MinFreeRate = minRate;
            this.AllFreeRate = allFreeRate;
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
}
