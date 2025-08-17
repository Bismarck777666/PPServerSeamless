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
using Newtonsoft.Json.Linq;

namespace SlotGamesNode.GameLogics
{
   
    class DragonHeartGameLogic : BaseCQ9TembleGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "55";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 5;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 300, 500, 1000, 2000, 4000, 6000, 10000, 20000, 40000, 5, 10, 20, 40, 50, 100, 200 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 40000;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "33gyqjdneLCFrAeK5BQ2HrA1Bu4IKwrdyYjtyO8tN5U21a/cftifF1j0L+h3dZ1gZ3NIjuCELt7FQteHRwibJCCRuw1AIsxpkSf2NMLp2TvzFjYrGeFbxW/JT2J3LPU8chz64bh/tgWjNaIRLCxi3pIoe+yQdHn2FiM23MZtKk+FVVsPuCM5EbVhiPz4DqRVJ03Lekc+267SArcUnPt4U/wT+8ZijGBhL+eTBqtGdaHa/wPJQHL6MOEnIl8l4xIuMK90N6r6seexuKpOH7lCnFsNnIp669P7stFZ5A2itYx+y6ZMJeq3dLSDwB99Y2XlfjSPOqWVMLjdIp4uXrMHXlSY3CD2cce/kbiynY2Bi5QSFBS6BQoxky0Hi1EQtAX98ERKLpZ2G9zWxsSPRU2Rki5K0GqxwAJSKUUFOwMMq8i+UGRdf4B+orD0+WTJ/kNF8gT1np2UDC65uNkaK6SVySb9xsBSAB0SokCdyw884Rayv3z5av5vW2EjYzXcNpvZy2KPNSsMLo3lOdgDlqz6J8iChKsGQlqujwAqMdtal/shdQrlDAOKba0Z1onIXnsdo6fFkNbbI3keXgoxESFJxiJa53KY7KqMm9APdR53CaWvBOU1l0AgDPaucjtW/AexrJu7atiKPJNTvrAAJy5tnirWXnPEMX4W2zdK3OvaU+4M9UlPQSqg4heFxMoDWlhoMg6utkiiIsDO25bWSey11coThrrE64VMEKMX//BXtP9r3ky+Z8iiyJIkzueztwyuSGpqwYGdgKMIEqO59WNr+mphAD/pDPX76y/z8IdvoDt/KFOr84ixXPZxI09hOykVLsISOkGdo9IXRBN2Xafy2mk2Yhbc8x3NEcZzZ5G9uw8+XSOjfKZExoJcRZ59izT2JKsIzB7az6ik2HGrT+osTCyZzIlfWD70S67SQBcQorRWF8Cllg5Gu2LE21CWMX7ca+0IWpUsQcRYRZL4b9wh/iwzIsvYjzzCawnTVptyYTvjNdeH7Ti2K9JXGKehUpaYGb2SH4YSp1oHu3b8+R7N69oNeo1GFvAdRyxE///p76ssVedQzt/2lnjNsJHZTS0jWiMztovIbg97LTry/fjZBPNeoXmqpA+XHeUV+m/lnej9P+V8COPyj/IgumwLtWA1rmDN2btMiN4HHpLoViIz0Td/374KKLix89cNKweyGLgF7Ykc9hPmwZY8sWt1uShSqIP5RPJ4EyDm8AVAKjZ9MS8qRWfplN2WGp2NmzPe7NFOzzLka/ItO0LEBLWm4TjBYbLwtpZU3pJFXXzs";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Dragon Heart";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Dragon Heart\"}," +
                    "{\"lang\":\"ko\",\"name\":\"드래곤 하트\"}," +
                    "{\"lang\":\"th\",\"name\":\"หัวใจมังกร\"}," +
                    "{\"lang\":\"id\",\"name\":\"Dragonheart\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Coração de Dragão\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Tim rồng\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"魔龙传奇\"}]";
            }
        }
        #endregion

        public DragonHeartGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.DragonHeart;
            GameName                = "DragonHeart";
        }
    }
}
