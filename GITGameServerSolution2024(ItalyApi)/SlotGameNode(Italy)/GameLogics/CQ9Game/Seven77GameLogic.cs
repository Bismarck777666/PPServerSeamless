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
   
    class Seven77GameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "26";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 10;
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
                return new int[] { 300, 600, 1250, 1800, 3000, 6000, 12500, 30000, 60000, 2, 5, 10, 20, 30, 50, 100 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 60000;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "8e6c866d1049d87da+DwNilirSVk9r1L4ZTaPSR6iNf7qkXdF3VKecBvDKaMvZBU9jcWv/G16nBzCsrzXA88iawGIOtNBVAHwDTA+is9r7R+I1aITAyUsTIScPpJu+SOo9VjZlo8kT/Bcqr8O0FfGVyZ6VNR50eewkroPkZ9n3Ktc2MUv+2iGvJFjAmvea0N0IIm22w30pwlpmvd2bTVkAxIVAvOkcgLHdsfCMF9Z1kUlyaxLXpxWfXfycW3xnIuz88NjxjqxSsg7hAHcoFlnWMN67dByzZuKjH5Yd1RGpzw7DMqI8h6PORU517QQrmgvmH1HNMyoJbSjgs362FGYZCsDCX8QRpk+1Gc2NtcDa985vdscpr3TFG9CRE3r95CbB4WB6yLdeg+SupxXtY6nORix13XqR4JB1xQ7z+tl1L3quBgNOXzcCEDAHHqqiGcbtFQIBGCqpRCihSmSz3D6/ezNo7wzUGdnZSJV3I6/bv3nZp+OUUHuDcOtkn8Y8x6rk2c8ask9O56MNiAstAvwGPOFjU+xA8lhO+7B9/cMeNS9lfSzuZouqxVSzOvNayV5dCg1nsIrBa/dW2jlHbDxzCAv4KOAEJ1R4QqzIIziqc1s2rt3SLONPgm3GmFwCcrcsL5jApy0RbJTOWSHk49iE4Udjv8v/M52TDEM06W4T5PhHYH3MnyIeHO7hdhpyKNGaKgkey03JF8QlRar6dpP8SAmhTqNYiPl96SmPOTxRfaXGiqxu3H6dXaW1sf+Q1SDAkHEF+59K9Vc7P4yQio0pHmDUe7JQwboTk9zVF7Pd5nOrHC/vm+wUtQNLEajzY82RupriLdJ6Mk6BQBbltWrPU+Aydyqx1IB+5fcHNR3jjB9CQ2a4fr6mCeCX9hGwSnCxvxK2/wixzQWUlFfU4QD/Bn1bahJVMRRh/PUcA6teIv0BpeovFG4buZ71L83XTBs1cdyaBZ9EPiGSvsPGntVgeqFIKrEN6CHVpQacIpCD1z1nV7zSczX/YQJB8KTgsAtqEQAHiW7Arr5JoH/V1q0gDfLgWWoT/l//mzXqPWP/0hNZjmsiLc6eKdt1SGk8JCqSBC1E9zJsD6fUB5uyvTyZJ69T8D3SWiSrj6fyOkKOh7pF+kI7JGc7UlHEvpbJMDQUoIUJ8UsYltvJ6XsaXSZWuxPlr008PfMqBuJ4CIaB56dBMQYmKUpwGCena6vfC10Qjh2CkVzId9hOyisoQvMVPF0aMNywXE7OmZ0m+ND+Po83crCjP8B5vZSI35IAl513dLHWp5+hzSqaq49ySppB+VkEo+FlBBZfA5QXuQObrz1pwNHz83/4LiOMmDGBydqNIwZ056Ypj74gV55JMKrzlvW6Px6WFILvAGBrhZRSdiwq9lixFirPPvtXQgAm307F6g9ylF8YVDkYoaITcCeS1Oh4xT8hhs9QuGROEMgqYRhXExemDELILVH3d1/jvOogg7rXj2QFN28ehgxHH0C0Sqxn7qCwUiQOyDEba+CKyqjMohz2dMjH8cKWEqaZbaT+1F7BXXCyjSoZwAOJvR03T0IgaGpo92Kp5wqTgoCXecX4WtuTIm50nf1aV6jTg+CyWz1JEkO7afJTTpJ4YbNr9i4UvUXV3uuqnqPZFOIVDigFeOqeSzyLzZpVxnu/GwxH2jEWf0UFlZen7RGEimXvm0S8CoT3008gYVpQPGt9PsBXOP5VQF+lFbIRAWzo834GS2LiCdUUcRIE3DsXSMN0FM15/CCl7nyabZh3haY9nVdneBbjewFBz/C0Pv3CLJhFQK37n6bPrhX1nRHcVz/6poouSoxBACFIlDmZdr+BDCYQXf5HqpNbJ0nYF2ASbOSPapqkx1+9MxxHpy";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "777";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"zh-cn\",\"name\":\"777\"}," +
                    "{\"lang\":\"en\",\"name\":\"777\"}," +
                    "{\"lang\":\"ko\",\"name\":\"777\"}," +
                    "{\"lang\":\"th\",\"name\":\"777\"}," +
                    "{\"lang\":\"vn\",\"name\":\"777\"}," +
                    "{\"lang\":\"id\",\"name\":\"777\"}," +
                    "{\"lang\":\"es\",\"name\":\"777\"}]";
            }
        }
        #endregion

        public Seven77GameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Seven77;
            GameName                = "777";
        }
    }
}
