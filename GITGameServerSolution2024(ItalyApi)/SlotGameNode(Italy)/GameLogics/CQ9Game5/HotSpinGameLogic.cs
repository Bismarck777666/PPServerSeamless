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
   
    class HotSpinGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "19";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 20;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 100, 200, 400, 800, 1200, 2000, 4000, 8000, 10, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 8000;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "dB9ycAQHf7u4AnpHG+i8Ui5RN9/eFiv5WZ4XUWy0mSyJYdirlvOxA3PUPfL7YriPZx1JpmVU82w8NntqZqt3U0LxneikmOMY7dYLqE6tzMLpnXzBtRbSxlWmhB40B3SBGxMDp/BALX3iTzxky+u6koyVGnKw+IqLnpC6c4NU/pQswk5L/6ATwp4QhEMDzmVbH9ry03ObJxpCTtnJ787kmiYpGVB4vO1x8H5mvw8pMekRjsHNlxkfZjqPF2W7D0ICnV+cj8wjiV54NLAS4hc5OV25lhnB+K8+K1hXYRGJRJpaj2l2UmsfJGtRf+vUwr5WHiMSlrfmBaIeyTYWmAnqL4LbPvl+eOmAHO5zbN+/4TJ6hfIp3/FJy6JaNno56FKvIsWr2l5g76vE2IGlw2azjDRa7+Zz8uKCyY0W6IxF96mqWNEpPT6A5JwCZb44EpqGFhy53kkfCdNS4qg67Elq+o+GCuz/IleRB94pU6rq5Q8MICDXGQUe7dzSkbuZm1ExP/aOhqlENevUOFODpsIvSWacA8neL7UE08VfTyXCw3VEKRffwZOz9aC4E0VUgFSaFZAQOd8JUhi70JGii0i71JFU77KUaSQ5XqhL/lXJz0eV5xp8EilgkHvBdRSyDo+fHwEyNQKSBbjZewM5p5B8/zS/1rFUfg3xS5XiOm81TtPS/GYWWIU4A0S+xpWNsyr+H+hkrC9HemOghmkwfvwZp7AkQw5jSdv+YTv+8EaHl5erPV+ekdA2HkZHrqF/seFuzy72erNM9fAER4hrGN4MKdqmasKxC+1r0cFX5GlLJWkJZutlYUn4xeGMaMLiKntsCsydG3dOp6EoSXd2OtUl3xoG6JNMXFrwacNKxVFDTo4jsBIo3IKGNXxFEZRF5BAH3HLHL2PEvspPWY321DKMLJtqLMZgp3Ju/fjZQoEm/4bv7jA/MziF9yWdgbN5o9ElmnmGGlgZWu5ito1VPOWC2oPVB4XOpweOL9IVET1HmSOArm9FwRGs5v9mZ8vsGnWi8xAvINRIHPpKpzuu0xptggAfbMjsin3krBWQNtk+HbkLHM+GSaJ+LG7EYCOcRh9OasK7xGpdq8DDXSeG87/0BcCskUynCpq8GYF3D4o2Uh4RSpMzlDxmi5gPMCUoo16YA1NLt20yxVyjbHf09FkTo11dXpnX+USM4/VyJuH3AVjOVIWMFGQ4DB6HzWCiOegeO7Z6MIrVNy8xG09QFgsCsDvVjs+4IaO0/wgByTZKraekrn+Z+5HPt6uWaICz5yu7UpK5Nl3L7amSmcP9Wx5m3Fboin0Yo+ANhI/uZ2tdc10oPQT25CnHooQhKgzpdAJY7DUHb4hICz/7zF+RdyX82Q0bONxCl12hVKKmhzsJ7wFE9/69muquOtWN9FpMZ3J+H9QUwTvoiyd2Zjac9hclupCGEsPwwNmvCg/4QQ==";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Hot Spin";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Hot Spin\"}," +
                    "{\"lang\":\"ko\",\"name\":\"핫스핀\"}," +
                    "{\"lang\":\"th\",\"name\":\"ล้อไฟลม\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Giro Quente\"}," +
                    "{\"lang\":\"id\",\"name\":\"Putaran Seru\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Quay nóng\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"风火轮\"}]";
            }
        }
        #endregion

        public HotSpinGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.HotSpin;
            GameName                = "HotSpin";
        }
    }
}
