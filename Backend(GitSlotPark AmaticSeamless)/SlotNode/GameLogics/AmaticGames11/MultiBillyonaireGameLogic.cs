using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;
using Akka.Actor;
using Newtonsoft.Json;
using Akka.Configuration;
using SlotGamesNode.Database;
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    class MultiBillyonaireGameLogic : BaseAmaticSpecAnteGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "MultiBillyonaire";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 40 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "052416066655773223322666767447744119773335555444477778833155592266888823207770055666655116663333aaaa77779444555511122228888238663363394440000554488004444555111144777119228822aaaa55882470001100022222aaaa77a77722221331113955533511aaaaa3336366556655444884488824304474475500550004411221122665505000113311333366676777925222533838885241606766557322332267666744774411977335535445477478873351955822668882320776700556665511666333aaaa477737454955511128228882238664336339440004055448800445551714114477119228822aaaa55882560001100022222aaaa77a72772221331113955533511aaaaa333636655665544884488846688878876766672430144747550045500044112201226065553001131133336667637779252225383888030101010101010427101000115186a02810101010101010282828110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d029101010101010101010101010101010101010101010101010101010101010101010101010101010101010282282282ff000000000";
            }
        }
        protected override string ExtraAntePurString => "10282282282";
        protected override string ExtraString => "ff000000000";
        protected override bool SupportMoreBet => true;
        protected double[] MoreBetMultiples => new double[] { 1.3, 1.3, 1.3 };
        #endregion

        public MultiBillyonaireGameLogic()
        {
            _gameID     = GAMEID.MultiBillyonaire;
            GameName    = "MultiBillyonaire";
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet             = (double)infoDocument["defaultbet"];
                _normalMaxIDs                   = new int[4];
                _naturalSpinCounts              = new int[4];
                _emptySpinCounts                = new int[4];
                _startIDs                       = new int[4];
                _totalFreeSpinWinRates          = new double[4];
                _minFreeSpinWinRates            = new double[4];

                var defaultBetsArray            = infoDocument["defaultbet"] as BsonArray;
                var normalMaxIDArray            = infoDocument["normalmaxid"] as BsonArray;
                var emptyCountArray             = infoDocument["emptycount"] as BsonArray;
                var normalSelectCountArray      = infoDocument["normalselectcount"] as BsonArray;
                var startIDArray                = infoDocument["startid"] as BsonArray;

                for (int i = 0; i < 4; i++)
                {
                    _normalMaxIDs[i]            = (int)normalMaxIDArray[i];
                    _emptySpinCounts[i]         = (int)emptyCountArray[i];
                    _naturalSpinCounts[i]       = (int)normalSelectCountArray[i];
                    _startIDs[i]                = (int)startIDArray[i];
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override double getMoreBetMultiple(BaseAmaticSlotBetInfo betInfo)
        {
            return this.MoreBetMultiples[betInfo.MoreBet];
        }
    }
}
