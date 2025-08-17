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

namespace SlotGamesNode.GameLogics
{
   
    class Hot40GameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "Hot40";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 10, 20, 30, 40 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0524d6666444444440000555555336633445544111117551155663333332222244557224433224411624e6224466111155222255556666112222334455766661115544220000444444555555333335573362516226622446666666644444411111443333333447330000222225555553366336655113366557115562516111112222225555555333336622444444422666666611335544662233755441155000044665544762516666622442222255225533333376633441166334411111444444400005511555555554466755335560030101010101010427101000115186a0281010101010101028280a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0291010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        protected override int LineTypeCnt => 4;
        #endregion

        public Hot40GameLogic()
        {
            _gameID     = GAMEID.Hot40;
            GameName    = "Hot40";
        }

        protected override int getLineTypeFromPlayLine(int playline)
        {
            switch (playline)
            {
                case 10:
                    return 0;
                case 20:
                    return 1;
                case 30:
                    return 2;
                case 40:
                    return 3;
                default:
                    return 0;
            }
        }
    }
}
