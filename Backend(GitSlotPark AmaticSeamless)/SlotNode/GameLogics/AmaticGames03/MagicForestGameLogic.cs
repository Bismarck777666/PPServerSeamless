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
   
    class MagicForestGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "MagicForest";
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
                return new long[] { 20 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "0521ea578368c98264a79a49365a08b7a6122ba62981795794b3947acb38647067a189b37947529b522ea4a2983648165b62a9562a0371759306b198b078bc8b0822da8025b394b95b74ac759165b47a0826903816594b9270227a267184b92793b47b16073849a1c4903561971b5222a08b4a71659b368c982649817a92c7a365226a47067acb3962a5935acb485a0894706791830227a3671b940b256bc8942649a80a307bc87105a8c227ac7593617582b637a082693c4ac75ba40689014227a52a7607381bc4967859384b65a267b1bc493cb030101010101010427101000112641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public MagicForestGameLogic()
        {
            _gameID     = GAMEID.MagicForest;
            GameName    = "MagicForest";
        }
    }
}
