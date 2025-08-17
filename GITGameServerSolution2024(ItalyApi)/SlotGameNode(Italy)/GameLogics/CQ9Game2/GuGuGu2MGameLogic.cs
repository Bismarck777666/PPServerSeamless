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
using PCGSharp;
using SlotGamesNode.Database;


namespace SlotGamesNode.GameLogics
{
   
    class GuGuGu2MGameLogic : BaseSelFreeCQ9RespinSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "129";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 15;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 300, 600, 1250, 1800, 3000, 2, 4, 8, 10, 20, 30, 50, 100 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 1250;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "92a435faf3f8ec38S1C4ZqQIv6gqfjCLajPg53wdVw9nkUEb1IjlnTfqER6CkcF8I02sKBMhciuKSI4OCQ725MMw43nJ41h37r6gP2hZTbukihziFpLacWks6DSvq/8sAOAoHt9cX/QZgUc2UKrinMIghYhLPQIPlJXlOcSMOcaIhy8L25Gk3P1ZElxKDOwyB8USjvdYQn93cGmYRIh6QLU5HcusuvUJDxUetd6x+s03UNTT8+WnL/u8rCWwTv3gjDG6iZUEcIxWRYswQoaLbmDyz99OSQPMA1BTg74AszhbgnunGyTA8mpbGGUD3ObnvtaoPUILA3u7SGHUA1FlPtLAv1GYykprizgphF9iy0c7AL7knziE5vCXNLBmucKYTE2HePWSPCStqKDSXOADqYfhQhPpa89vjRCPP7A0X7s7MS28HWHd3LWFm68gqXQ2iSzL5WCYnFMG5+owyHVsJba4lwckuvhWrHD6rSJoDVanMENAgwOsoKhl47U0AwwV7MfK+nwON+0OolHyCnjfisPUAymfjqbMoCte5ZFIJfB0wir4rGh3vQQhwfW7CURlR8toD+1t9CiSJ3Me6sPpHaCFEu7BWD9Pt2fOdoqraG5/1A5HvtI3Fft4FUIOrm+eF9lczjNqxZh93WiDq0cxuq0LffUp/7rEqEgp/NlO/bA57HoQDsMRahuq4qhjWrlWFT7ux1Z9DY9TaKksgkqDKYb2tATvjYgNxPeEOUBoRAP4G8zJaNXXb9mEmw+T4yowuJ0d4jKmRT4P//VcLzHvzeapXCfbDwzYvKh/KF75MBJHBLsVKMGbWcMMdf0fbRSOQhtf3vBpwndAwohSXGTcI19NRfPTefDwz5gXSuCe9WSSEpOI8PIHm5P/Ijj22+qIL5sQth1tNIwKwXLxZKvjxmONO9zk7rR1753FmFLTLtTyroiPD1RbvyeI6zJxdlTBJSPUuICg3KQ5g1R2/JM7UcUKP/DMiHm1VabgPABPnXljXmPwHleLJcfgayMCjjXhCH2+OmTfn08cmKymfsT3Kyz+WnO4c22U81Vw5oQ12G2w5m6nXDYf8RIp8/4ugEJsWzsnh3IEJMapnmC34tKxJNw/YP646emimAdjmykY9d3jrihqjunwHo2itXs8GqtANeZXTxnBZomNKAwdD8TTO8LIqnA2XsWvjN2zDjZ22NxuCqE+SemW/uDMDZWbjiMXco3FR/kIT3X66AF5zKqYXtIWUF6NKAKpqUyZxFglSg8cAXaTIWp8Pr63tb+UswmUEpfjlW2IoE+2Zo3gT32PqAL0C5fUhskYREFR9RLvAGGH0XZefFgYrIHjmPfba3tiVueN2NdWuZwvoygdVTO3eh4KAJQlk3gJb7RdaRMYlbQGuLj9XqXsJXQs37ITyCepMeiNFxPS0PACshIrgB98LKk3nMe+TDZsQJhw42DjWK6e7XLK5YJxSG2k4QlmtQmuQri8liXF4hsZwcpQCqdUGyC6zmRQwlt/+rHdbQ5wqHEo7m33xUVFjxg98OTbOuOKjrfQrqEIKaMuKXrTNXUR3DJTH29HiEerpLAFFQloeOw27a7AGJ6emzrsOe//gvD0JoCAOk0ySMAtP6O6Z1WDm+KBzCZXb1kjPQQuXXVI+mE0gU60OCodO5DXSHjaWx4nWLIU4vUdJhXLPBchzhw8CKNq+n9mRzKBrXvJTQr9HgwtEKrWEx6ifh+Dcd1zjYuqZ5ye79P12/pTSAjwxCFTrJsR8d8dstyjQkfzMBGayh/WR5pKG10phC+A4q7Vm3nqVfVGhNRuBJbIFwqIvX28Q7aUnwyXL6Ir4hEAqbiJSA6S3X1yxlGwdZ4T96jCAlOEc543ykuEBcm6f/BppKY1fVhqSaWyej+z0/RuF/FPgNXZ59ITnWQ6qFbKXMpnCMOMl6ObsM3QPLmQei3UplfuiVOurb/AoETPamkjxONueBJiMzsHLZWlYx47Ldm2hjj3RDss7nxuDlNE7DO+HeLDKyg5E8shUYt1P+j1edibff9sPAMCeBv2HuC7qjGWVry16W3a+ai5oGeBKtedLuf60dPxDhkD690dLUDK2e2z0tN9Y5YzLQU/aXxvevfPtNytEj8jo866rb4egRThbc8miNf0/ziFdOrcsewwTf1BEHdd3/zJ0aKS0IWKUWK7BwQorUomznkFPPjIWbac4USJpwCx9L1Icj6nJz67Nlwhauus0Z379/2IPFjE+n2uC4kCyUpEdLZ50R0HGZs/QCqAVC38VRai+1z6Vw/LVGb0nNizqI6Jt/4QbvDnYs+1qKfvjPZGEhbmKwsV1hJzE0P+wuMUqu7BxfIU7CCUdTUQyk0pEoflg+mRJFoXLESuWh0CE078FdZIyJdpKr9KDAbV4vMsm3dnRUY2Rktxbajj7shDD7r01scCj5zsRQs+1iz6LG6FdRUwUuW2Anyu/nH2prNNR0f5+A0PCc+2vp3eF8KZ0OOZdMVwYDokRFEH2jaRznKP/ETBGL28lj8nLdAQGM2vqF35vTkK4BDhCGde6IJi/T4TRkhwnCCX1u0QFBK6VLkkn6PCurIfewxO6H2FRjX9u97UiABySUPfMeQ6K5az/P6YRpi3qhNBUBJ316BgsVzu76N7EU4/krCADRLFSytMdy4E6dH6zVDOUASLsit/+qbPbkH32IkbYSy2OZYYJFKEE8wDKC7atCXZDvPWPIsmBhde0Cl/WlnNnzXRNH6tQ7GQa0Ljajppi0zuXUSONvqMUOUiImOSvwFouBGzfS8CwDsGZQIkEZstvpGCFLfYJ3HxSGJhI9dStzWpKcuUS8mKBxuXLbdbUxpI9oSL4Iv2/4giZD2beOnosq0FRNLGakX5mwKJACvNlHSyDBRE8buqX4b0Jklc0iE77GZFxXHSG3JBwD+Z6KJzwE2BlpWPG85nDkoT/oYv6aTilwtC52y4bF2fp/nTwJro/uNoj8TKfz35wVVSqLl6mrq3djnw64OiCbUkvbUemyJlcZ1jIAievL9OBnp6thuLj8pHzlwIDpFCqKzeed3O32PzF5rzu12ZSJQjaLQCgDfc4JEdB9hFHeVP45f+uGRJYe6blNbHvPZkcrvvKogU4zYmGYop/ebmpZBex/7Ezz/oay3jYtTSuiUz2gLMmZ8OtCc/0t+ZGM+t0cQeLYgxWFffv9MksVfGRYzVBZcy2MWa5XIckPLaH89OA/cPS2aYItbCq5I6T9YCYl6Jp67rNiB/BQaxGD+zUpyxVUqcLrx9vqtIwnNcY5vD2Vad53ZCsDgRGZ3NbRltUan4tTv90NlG+wQaN5JAXj/5R81vzKSiF5kmcw8VFmDE4DGhzb9Ou+QEJiUZznCfjw6E0UH7V7HpJmQ7skHYs8A6eiJMAbeL2TfuaEzUU97gvhyY9D3RaoP2QECbwXBYzWl84ToMehJUcu/4dQmdDCkreybAT4ZvPsULasvsozt4QkqGSTCNPXhkOs4/t7djJpcHNhVhs8WF0s2cbxQRgNdMqPG/pwO9Gw4koz7JmcTDcSg9+vNsfkxge4EyavnchtEe9n7AlVIq3D0SysMOcug3NV4bDCN+tFITYG2oLqrOfzgV95MUapXhLx/Ta/ZLdKg6zwqVp+fwjvvexe19YNbx7tQhP8L2Mq0wqI7fW3T/1oqVCUy2KEkiJU/+Tlzi0PTDhG4RA1EjYNMGrBhXB1V3D7u9Ty6/lOnq/6v28TocdgMNhqQU6sigRAvmB1tNA4pDXJ8nNT3kD2pNz2srwogLNHRXCyUT+QKqbUcy+HNYMV+jllrFzDvB60KqHPP6+dIxwN7AY4abuFHjRCjwBIE8JGQuJ+GjAt9IGwapuvo48qFSzW+m7aKE9EigN6S7X4AV5RRMIwPshejTBTuKpRoCrICq7rUgu3EaxcjeG5jh4yVxOiXuxsHCWzC9iHmf4mFYPNqjp31r4+KIV+iOn31ZMH2+9bl2MLcDpTOY2gwishFBF/i9U0OrVO6cfa9QWi4EZ4P6Y9HbT6R1sF4+jYJewUZUEcn3dhf0darIDUS3V/TbLUs+K4ApWx9/+Id/SbK5EEiSrMP5rYeo/fxW1JmBdCoCvMKBG3uFknH+lzDUfkUylxxTjdCikC1vU7p9TgZvlovMwaaXRxoZ5ewI18Kwl/6eYa1XXLPQAahvkbiBMp9wmXYMy6YfHymh4xC+qNXCsuj5SjMOz3a99fSuos2cqgkEzCVj6LvqtOXPRg6xvDLK+6DY4vCkeh8ZeDjBe8PbhyJrJRqwbbSZivD5NOp9uexi5f3aIKgtG0DWfq/S+pIgTwSW3OHRBAyg/f4xQrIfDciIOO6N9PG11tG6JKuKNI+0V9dfIEWlmPDQ/LsPzUKvjI5+WOYrGfbLaaOfEdSUMwztlphbfTeA2LDiEMxgSW7np/iDr50N67nhcj2y+lh03QahevTrOrLeZu2WhJq8TIlg02dwLsJRthEy9hxv/0dHLs5tAzeJixqWLlZnESWSykQc+tvmlSYrLj4SmbBF5CSws5E5CrQmwS4U8mzSyA+ZBYWH3bOUEzRwfhOhUGU34G1eIp768w8v6JwFCi+3mDCCVfkZf5vpO9I6wU752dUkYncEn0JgFnh74+szQOmxqnqorHv8XzzVH4GnRuEpwDJjKcvCgRmM/ivcrJCkBX7HKh+6/lL1MjqutCZILlGD/iEG8arOm8S+cUDBn7VmzY4Jetpj58SjxYMTJt2ugBfP7kwn9vTcaEJ9tGG6cYiiMGlxFF0eiuSsak/u8JtiGjyPUDyQr86aB0VGjpiUzNC5iPukB79mkjqBsXSeklz2O3W6kaOBwldCA0n+sIzAhDOy7nJOlr8eknschHGPlK03aw1SHGshabYzoM+DtVYJhlipTC9HU81byBLrxtLESGQLvi6SxwZ4DL9AQJWmDTmm+NjvPa2xG6suOIQIBpyNKMG3i8gqgEn8+pDnV0LsZVRpxiz7vjVxY1XihbXGpIZhcGm0QuLw86yeS/sxE7wM3o+m1NhQNHdo839u2mASoONVwQqKr5CVzdorKTfFd/hfdiVcg2dKzASBhUXOEK9/X3gAsV6KxqNtHEZh/XB1vZ4ixp2Ljea0ZKB4Ct5QWiC2c5gobbMUHUbYn9D/0UwMW7jOPb6KefxDn+HvBtsUQU82RDKVX33CkstouShYKfTOxLXs+3WlVdnynnyhp7cI2nOqGELdhmuAz79NC71DB55WWOvJOuFQsEDIQIVQQs+LDuqfgyWH9c+9YoV+rmqmLjX7DH14vPE5qy9uNkF9vqISumjahlUOhn3GCBbxdMEXIdvbfpitKiTLY8HBdzbPiTrCyFz+qcTaiu7CwB3YChyDsk84ezpCykJL+c1rX8IAdCiwD3O6/orIqteC5xGA4UiMgqh9wD1E93BYQWefz5DCNRuTfIJXu2hBwCr6/Yb+XZKF7QPPv0w4Xr1bH1SdOGf0VPKF86k0U0yxmKrMjO3SiV3s92Pr80rcWMnJKjWvjI2vevWo8r2XqzchhcD+lzXcVtGUTUiNXDJJopyzLO+n+7w6JmPA5X69joKNyYWE9GwCgnsqU7IIfSxYzOdpa03wBS6v0apCgLQ6UOTDFyNIAvGFlUcAcaKzQWdeCMNOh/UYtl/I0h+VSmE7zqnw3SiU9bH2exJFERKHTE+Z3+H7R0IH8zXg5uuCZUtuLAjdJFKO/vN4rDnZ55OmB0EyfYrL1lgujKJQxMq5PTu8Y67uwZxDjXxG6zniD+R7adTal3HzPU+mgwI8KlkDSkZfk6CQIxnv3TzuQ6XX4TqrVmhqvy0ip1+iWNmFpiSiDD/1b13JOQcPQodV4mjAoUnoVUW8rpO4yapJkij6xOsfsNMkaF6/WiZOhbUiuS/ixAKHt1HWaGLYjVskN+l00KgququQ8OLjJM44Jf+XSXmgsfIdeVO0N5lzYsscJKWFh2nQU81V4+QbPczqzBiisSBDw19xSip2yln5NYKr7t3ezv4IWl3kCcfQ8JhdbHkf8l4A4PPs0q7zGl2JcjUmp7MYJyt9j9NAATgcM4KBIc9kFF1Nk7mwpk0G0F1HT/EkWtjbqZQxJEcvtYBDkbJFp1OpEQsfjd+WOvLlpVWkl6irOFAGfdSk8Mfrm8xLHdj9NzNJC67wuOvbhq/vaJrVcHr7DQW1ZHlaY+hM0pLU8wbZwXK9NqTqQ/EY7IS0h5uA9jzVoqvbpjDgcRYT/GEF9IemClZe6WBD0/T/ohsRaC6U/Dr0vyN/6LkiOpUONXJ67U5ZMjwfvfdhzCZsXEBFRwOece3VNgTTPuC40LjjFr/5y1Idmt+faP0Y4C95umagYKQebUY02X0JehzOG0bGtf9P16s4n/GxGmotQdgl3K0OQ0mMV4nN9JIyfrIU4h5rzgP3EQLp5EL4HhyZ3R91PBCHpp+AAuiutayAE9hgXM+jJmIW1hl1ChRl3NE6R0aGLEd3aW6dmwJLoM8kIJ6r4oX1MUK9yDjwMx6x3Xub9YL99raZzjZGyRsYYFOTlefu0gjLCKU7snx3Z8iYhKV5ELdHnQbWS2+I0vtaG3Q4phcVCZAA99WgcWok63ZKy3tzkkQ/XTtWM4IK/KXVmKpdcOPWVI6LrMnajWaU/oNRzE4gRWBWT4vCqstrEUErxsZIyft90qZubq9QfKtnfadOBuuetdFjXKAI8y8Bl5dPDMRlsSSsgzbiryweAmAFT/NJwhqdfTLAVbihI50X6ZDcP4Hkfah/5OvKjL4UC0vu1/McfZpvT62rzrFVIDiCGor/QYi/35jHuwjTuUQzKq+YTg+KDH6uJxl0FXqUOq++OTnniTmFuc+j9JdMoYLo1ElCbJYuaPXKLEKpw2LsyABIXCtqr741FOuooh7hs9z/8NBT852nvcQWFKszLPiTI/e298MvHTrWRQrd9JFPmoZ447XVoeLcEaD6Kc9RiV2NAEo6e5P3GyskaovNPtHNpni24QYVL2Uo5Yo4+u+8J8WTH0/yk20fwnaBFwvUTcuCnz2fnNZx7RqW7TRqx6nT4WCPUVrHvBfIZ29VrzahsctSi+mskyxr+nLjthYDIPM2UgdCa8tDl5SbzW0uBeQvBWOQS17gKE/H1tS8CI37lYBLzyv5BG1jR5PxXmfv3y0KSw/skdq8iekQYjnRcEE2y5d/dzovU37mamQiBVZolTY+5CuPhQdcxUyTKh2Jdqapk1kxjA//f1mNeyKTGfF1Y/eUWA/dimOU6EFRLTGKv7zGiZKVJUj68URgv64XbWgCKo+cxjh0Zz9Cmya0DgzfW1FYrZ13VOT8yHnelqrK3ks0p87I2NNSDr2/iLYkg1CEyweM0S4gAmsszRVO6diNX1Xj01nP6ir89pwXShHEv9Za5SWC5O+YTeGJGcjkXLBi90rr8IpBdj6WElhWQfmg8wMCQj4ajYYuC+Qeo+tRmHwEK3UfnL6obJEEtDSqxOH3H4wNIYiuHzfV8pRSAiZkayXzDABT5nokEC2AiJMt1QJe3DYlpbHpNnhUkqfur/4oSNyV2vPrtOzLOemophSvuC8V4rpZ8jtGZ9DmI+617qt44njlrBw4Vr7Mjhlsa4gguOS2QX7OwAbiXYWK8y2RKHwf8KUpZKJuCh56MGBYVHuzyI5XuKnSTt+oGpTs/6uaMtXcglD7kgijypSsnORam1EcD+OKXWmF28fJnM8LQTavAX0Zo7JDKXRoHvYMkcl8sxczNSZlayHVtoK++QHdA0TgMqyAXaijHzDyvdxasaOM3ARfSSLKFqVRg5nO51CkuA8RlhIixFJgUnrskp2SOgpQrDtfFoGQ72ft2agtLV4v3G1hKqKEdIBxVbb+E4KYH8/Odc0ah1WY1slC0s+ycXgsaAK4nbZak1QdACsZmfH6xnsfvWceLYpyCdMfiTARxqNPTNAk7VG5JMbNds3adNl1YNepXTjGV5tbcljgkvuBAP8WYWb9UsihouzqaQOzeSBjpGMWDnVSASdJPXJ+bujzymlW6yFw9meHN+tldyrOT7yYce96SGy3/T3Lyol8TOrfG64fOS620BW8qrCXtLyBNGwonaFp8AQHzEraOTD5CI7z19AWm8SdO2QTAbNdt1mIeq4IQf44ns+M9yVI7pnBu9uRmnOHq7g+qmMdap0FYvl8LvYn62dKdfn829GGWJQ3/LGsLPd8c6MRP9OJV9DxGCHLXWD7xt4SuPUHUV728uVR9dw8AaEipKlxVG8j1O+lvGZnIVFzIpIsWaMA9xZwlXWfZOq+AHnN1uvN8mk9V535iLEGKBcs6TC7GBaaXOJg+sJVHI2eBgB677hXGttMmZHeiNcY03BEVzhES/7UPkqHERXkdJXKMJUnU8uWRa4H4sQDPuTiDoH3Wd15nmHWSl6WE3FEUZUnkKC9NwxxUvbJsHtiUTQwClQYULn7jtVWwv2plB7ekulsbec4VML+gw8C7WKcqAcLVwcQfYtqOsx+kXd1iOEV9c7tuTAr6frOcT0N+lglovtM/T67lVs2p8mpuHvczKv4y/NtrYm3/QdM6k2OOpynqXeYsbrENbPojTLFJU9vPpyAzQacHF7NiG2W/ZtdHAytUIHpHcfJwUre3hBJnRt2mXakGDkaIi3frQAvV2Lw2wy0fxehGE6XWT6jKKVehTKON3EMZ0tBxIJqPNRx5CvXpR3+cb4oimZIuuBY9OVZ0qwHM8jny14zzFZag+MIfV+oXJc9aMywLvZJWjd4R2RLhVQaFZn02FRSG2hair3cUACb8vjXqQiLFdUNgc8m4tZpxwSQnTBAnz9kMz9BA984/qqSPI6zKO2veYJbvPvNoj5BzNdHhFOdAiOXOaUP10cVBDYpKja7SEMxnKix4ntgb018l5tT1Cfw8vlioix/KbvRxEUgiPu6pdRv/MTT26F4vKRHT1qUpmHp4vGX31mmqQk2kwX+xTrj8hwkBJnOK4N7Aj+vMzQbhPXM/llGDA+sviXIPH9nCXEXxn8On97sfntkzyHDUksO3DivxcDsNaM2LPBss1zAO0L2zROMmi9bAm2jr7F/SkrgTNT+UjFRJF3IV58kwvP4aZRnZMdAezgRafRZBHiTmw48MzyP2y8OKXqOJ76f4CqGtabgc2wTOFgftFACu3ksjxoaPZlsNIa+ksx0gsvOCc+ats/tkkO7ZjJgCvCW3nPqerEGYMOoYHQijbAM7CLaFxNbpThcfvmfuXZLWbrPPQyL8tz2D0GOsm0Bu/LJBnH2BGymy7Zvkt+GCtqCT1vRm+llJN5ZGDGTR6oAwLEI0d9ertWX3GrGrwZUSau4/cycfDPSpDCKNS1JkmTSuIJAkzvkOeU9BMq2zGvvAUf5jIb1CBRtM6C4rNcrYnD0jDVjnVJNK4l9KX7Yd+SZ2I/aRO324F6lwQjtGO5q6wxBjh//se7jgP4Y+bbty5TBrvWaMxSPdOjbv9MQqIoBUbgqb7+GfQ==";
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 3; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202 };
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Gu Gu Gu 2 M";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{lang: \"zh-cn\", \"name\": \"直式金鸡报喜2\"}," +
                    "{ \"lang\": \"en\", \"name\": \"Gu Gu Gu 2 M\"}," +
                    "{ \"lang\": \"ko\", \"name\": \"Gu Gu Gu 2 M\"}," +
                    "{ \"lang\": \"es\", \"name\": \"Gu Gu Gu 2 M\"}]";
            }
        }
        private string FreeSpinSecondLine
        {
            get
            {
                return "{\"Type\":3,\"ID\":144,\"Version\":0,\"ErrorCode\":0,\"PlayerBet\":5,\"AccumlateWinAmt\":0,\"ScatterPayFromBaseGame\":0,\"MaxRound\":1,\"CurrentRound\":1,\"udcDataSet\":{\"SelExtraData\":[],\"SelMultiplier\":[],\"SelSpinTimes\":[],\"SelWin\":[],\"PlayerSelected\":[0]}}";
            }
        }
        #endregion

        protected override string[][] BaseReelSet
        {
            get
            {
                return new string[][]
                {
                    new string[]{ "11", "15", "2", "12", "15", "4", "14", "11", "3", "13", "15", "5", "14", "1", "F", "12", "2", "15", "12", "4", "15", "11", "3", "13", "14", "1", "F", "12", "14", "5", "15", "2", "14", "4", "15", "1", "14", "11", "3", "14", "12", "W" },
                    new string[]{ "12", "14", "4", "12", "11", "1", "15", "14", "3", "12", "15", "5", "14", "11", "2", "15", "3", "13", "4", "F", "15", "11", "4", "12", "14", "1", "12", "14", "3", "12", "15", "2", "14", "11", "5", "F", "15", "13", "14", "1", "15", "W" },
                    new string[]{ "12", "14", "1", "12", "13", "4", "15", "14", "5", "12", "15", "2", "12", "11", "3", "14", "2", "15", "F", "13", "12", "1", "11", "F", "14", "4", "F", "15", "12", "5", "15", "14", "2", "12", "11", "3", "14", "1", "F", "15", "4", "F", "15", "11", "14", "3", "W" },
                    new string[]{ "13", "12", "2", "14", "15", "3", "12", "5", "13", "1", "14", "4", "15", "12", "5", "13", "1", "12", "14", "4", "11", "13", "3", "15", "4", "14", "2", "11", "5", "12", "14", "11", "3", "12", "11", "1", "14", "15", "4", "12", "5", "13", "12", "5", "11", "15", "12", "11", "4", "15", "14", "3", "11", "12", "15", "14", "4", "15", "13", "5", "14", "13", "3", "15", "12", "14", "5", "11", "14", "15", "W" },
                    new string[]{ "14", "13", "5", "11", "12", "1", "13", "15", "2", "11", "12", "4", "13", "15", "3", "11", "13", "1", "12", "14", "4", "13", "15", "5", "14", "11", "2", "14", "5", "11", "12", "1", "13", "14", "2", "11", "4", "14", "15", "3", "11", "13", "3", "12", "14", "1", "13", "15", "5", "14", "15", "2", "14", "15", "11", "2", "14", "15", "2", "13", "12", "5", "13", "1", "12", "11", "4", "15", "3", "11", "4", "15", "3", "12", "13", "5", "W" }
                };
            }
        }

        protected double _naturalFreeSpinWinRate = 0.0;
        
        public GuGuGu2MGameLogic()
        {
            _initData.BetButton         = BetButton;
            _initData.DenomDefine       = DenomDefine;
            _initData.MaxBet            = MaxBet;
            _gameID                     = GAMEID.GuGuGu2M;
            GameName                    = "GuGuGu2M";
        }

        protected override async Task<bool> loadSpinData()
        {
            bool result = await base.loadSpinData();
            if (result)
                calculateConstants();

            return result;
        }
        
        protected BasePPSlotStartSpinData selectRandomStartSpinData()
        {
            BasePPSlotStartSpinData startSpinData = new BasePPSlotStartSpinData();
            startSpinData.FreeSpins               = new List<OddAndIDData>();
            int[] freeSpinTypes                   = PossibleFreeSpinTypes(startSpinData.FreeSpinGroup);

            double maxFreeOdd = 0.0;
            for (int i = 0; i < freeSpinTypes.Length; i++)
            {
                int     freeSpinType    = freeSpinTypes[i] - 200;
                double  odd             = selectOddFromProbs(_naturalChildFreeSpinOddProbs[freeSpinType], _naturalChildFreeSpinCounts[freeSpinType]);
                int     id              = _totalChildFreeSpinIDs[freeSpinType][odd][Pcg.Default.Next(0, _totalChildFreeSpinIDs[freeSpinType][odd].Length)];

                OddAndIDData childFreeSpin = new OddAndIDData(id, odd);

                if (childFreeSpin.Odd > maxFreeOdd)
                    maxFreeOdd = childFreeSpin.Odd;

                startSpinData.FreeSpins.Add(childFreeSpin);
            }
            startSpinData.MaxOdd =  maxFreeOdd;
            return startSpinData;
        }

        protected List<BaseCQ9SlotSpinResult> dicToList(SortedDictionary<double, List<BaseCQ9SlotSpinResult>> dicResults)
        {
            List<BaseCQ9SlotSpinResult> results = new List<BaseCQ9SlotSpinResult>();
            foreach (KeyValuePair<double, List<BaseCQ9SlotSpinResult>> pair in dicResults)
            {
                foreach (BaseCQ9SlotSpinResult result in pair.Value)
                    results.Add(result);
            }
            return results;
        }
        
        protected void calculateConstants()
        {
            double averageSum = 0.0;
            for (int i = 0; i < FreeSpinTypeCount; i++)
            {
                int    count = 0;
                double sum   = 0.0;
                foreach (KeyValuePair<double, int> pair in _naturalChildFreeSpinOddProbs[i])
                {
                    sum   += pair.Key * pair.Value;
                    count += pair.Value;
                }
                averageSum += (sum / count);
            }
            _naturalFreeSpinWinRate = averageSum / FreeSpinTypeCount;

        }
        
        protected override async Task<BaseCQ9SlotSpinResult> generateSpinResult(BaseCQ9SlotBetInfo betInfo, string strUserID, int companyID, UserBonus userBonus, bool usePayLimit, bool isMustLose)
        {
            //프리스핀이거나 일반스핀에서 릴이 선택되지 않았으면 일반방식으로 처리
            if (betInfo.HasRemainResponse || betInfo.ReelPay == 0 || _dicUserResultInfos[strUserID] == null)
                return await base.generateSpinResult(betInfo, strUserID, companyID, userBonus, usePayLimit, isMustLose);

            //유저의 총 베팅액을 얻는다.
            float totalBet      = (float)betInfo.TotalBet;
            double realBetMoney = betInfo.ReelPay;

            SortedDictionary<double, List<BaseCQ9SlotSpinResult>> dicAllEmptyResults            = getAllAvailableSpinResults(2, _dicUserResultInfos[strUserID], betInfo,false);
            SortedDictionary<double, List<BaseCQ9SlotSpinResult>> dicAllFreeStartSpinResults    = getAllAvailableSpinResults(2, _dicUserResultInfos[strUserID], betInfo,true);


            List<BaseCQ9SlotSpinResult> emptyResults = dicToList(dicAllEmptyResults);
            List<BaseCQ9SlotSpinResult> freeResults  = dicToList(dicAllFreeStartSpinResults);

            double                  relativeWinRate =  betInfo.ReelPay / (betInfo.PlayBet * betInfo.MiniBet * _naturalFreeSpinWinRate);
            BaseCQ9SlotSpinResult   emptyResult     =  emptyResults[Pcg.Default.Next(0, emptyResults.Count)];

            double payoutRate = _config.PayoutRate;
            if (_agentPayoutRates.ContainsKey(companyID))
                payoutRate = _agentPayoutRates[companyID];

            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);
            if (randomDouble >= (payoutRate * relativeWinRate) || payoutRate == 0.0)
            {
                sumUpCompanyBetWin(companyID, realBetMoney, 0);
                return calculateResult(betInfo, emptyResult.ResultString, true);
            }

            BaseCQ9SlotSpinResult freeStartResult = freeResults[Pcg.Default.Next(0, freeResults.Count)];
            BasePPSlotStartSpinData spinData      = selectRandomStartSpinData();
            spinData.SpinStrings                  = new List<string>();
            spinData.SpinStrings.Add(freeStartResult.ResultString);
            spinData.SpinStrings.Add(this.FreeSpinSecondLine);
            double totalWin                       = totalBet * spinData.MaxOdd;

            if (!checkCompanyPayoutRate(companyID, realBetMoney, totalWin))
            {
                sumUpCompanyBetWin(companyID, realBetMoney, 0);
                return calculateResult(betInfo, emptyResult.ResultString, true);
            }

                betInfo.SpinData = spinData;
            freeStartResult  = calculateResult(betInfo, spinData.SpinStrings[0], true);
            if (spinData.SpinStrings.Count > 1)
                betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);

            return freeStartResult;
        }

        /// <summary>
        /// 리스핀돌릴때 나올수있는 가능한 모든 조합을 배당별로 얻는 함수
        /// </summary>
        /// <param name="respinColNo">구구구2게임에서는 이값이 항상 2로 들어온다</param>
        /// <param name="lastResult"></param>
        /// <param name="betInfo"></param>
        /// <returns></returns>
        protected override SortedDictionary<double, List<BaseCQ9SlotSpinResult>> getAllAvailableSpinResults(int respinColNo, BaseCQ9SlotSpinResult lastResult, BaseCQ9SlotBetInfo betInfo,bool isFreeSpin)
        {
            int[] oldReelStops  = new int[5];
            long[] reelPay      = new long[5];


            SortedDictionary<double, List<BaseCQ9SlotSpinResult>> dicResults = new SortedDictionary<double, List<BaseCQ9SlotSpinResult>>();
            for (int i = 0; i < BaseReelSet[respinColNo].Length; i++)
            {
                dynamic oldResultContext = JsonConvert.DeserializeObject<dynamic>(lastResult.ResultString);

                JArray oldReelStopArray = oldResultContext["RngData"];
                JArray oldReelPayArray  = oldResultContext["ReelPay"];
                for (int j = 0; j < 5; j++)
                {
                    oldReelStops[j] = Convert.ToInt32(oldReelStopArray[j]);
                    reelPay[j]      = Convert.ToInt32(oldReelPayArray[j]);
                }
                dynamic newResultContext    = oldResultContext;
                int[] reelStops             = oldReelStops;
                JArray respinReels          = new JArray() { 0, 0, 0, 0, 0 };
                respinReels[respinColNo]    = 1;
                string[][] reelStatus       = new string[3][] {
                    new string[5],
                    new string[5],
                    new string[5],
                };

                reelStops[respinColNo] = i;
                int row = 0;
                do
                {
                    for (int j = 0; j < reelStops.Length; j++)
                        reelStatus[row][j] = BaseReelSet[j][(reelStops[j] - 2 + row + BaseReelSet[j].Length) % BaseReelSet[j].Length];
                    row++;
                }
                while (row < 3);

                int scatterCnt = findAllScatterCnt(reelStatus);
                //일반스핀에 스캐터3이상
                if (scatterCnt >= 3 && !isFreeSpin)
                    continue;

                //프리시작스핀에 스캐터3보다 적을때
                if (scatterCnt < 3 && isFreeSpin)
                    continue;

                string[] SymbolResult = new string[] {
                    string.Join(",", reelStatus[0]),
                    string.Join(",", reelStatus[1]),
                    string.Join(",", reelStatus[2])
                };

                JArray symJArray = newResultContext["SymbolResult"];
                JArray rngJArray = newResultContext["RngData"];
                for (int j = 0; j < 3; j++)
                    symJArray[j] = SymbolResult[j];
                for (int j = 0; j < 5; j++)
                    rngJArray[j] = reelStops[j];

                newResultContext["SymbolResult"]    = symJArray;
                newResultContext["RngData"]         = rngJArray;
                
                JArray newReelPay   = oldReelPayArray;
                bool newIsRespin    = true;
                for (int j = 0; j < 5; j++)
                {
                    if (j != respinColNo)
                    {
                        newReelPay[j] = 0;
                    }
                    else
                    {
                        if (reelPay[2] >= 1920.0 * (betInfo.PlayBet * betInfo.MiniBet) / (5 * 15))
                        {
                            newReelPay[j]   = 0;
                            newIsRespin     = false;
                        }
                        else
                        {
                            newReelPay[j] = 1920;
                        }
                    }
                }
                List<CQ9WinInfoItem> winInfos = calcWinInfoList(reelStatus, betInfo, new int[] { }, respinColNo);
                long totalwin = 0;
                foreach (CQ9WinInfoItem item in winInfos)
                    totalwin += item.LinePrize;

                newResultContext["BaseWin"]                 = totalwin;
                newResultContext["TotalWin"]                = totalwin;
                newResultContext["WinLineCount"]            = winInfos.Count;
                newResultContext["WinType"]                 = 0;

                if (isFreeSpin)
                {
                    newResultContext["IsTriggerFG"] = true;
                    newResultContext["NextModule"]  = 40;
                    newResultContext["WinType"]     = 2;
                    newResultContext["ReelPay"]     = new JArray { 0, 0, 0, 0, 0 };
                    newResultContext["IsRespin"]    = false;
                    newResultContext["RespinReels"] = new JArray { 0, 0, 0, 0, 0 };
                    CQ9WinInfoItem winItem = new CQ9WinInfoItem();
                    winItem.SymbolId        = "F";
                    winItem.LinePrize       = 0;
                    winItem.NumOfKind       = 3;
                    winItem.SymbolCount     = 3;
                    winItem.WinLineNo       = 999;
                    winItem.LineMultiplier  = 1;
                    winItem.LineExtraData   = new int[] { 0 };
                    winItem.LineType        = 0;
                    winItem.WinPosition = new int[][]
                    {
                        new int[]{0, 0, 0},
                        new int[]{0, 0, 0},
                        new int[]{0, 0, 0}
                    };
                    for(int j = 0; j < reelStatus.Length; j++)
                    {
                        for (int k = 0; k < reelStatus.Length; k++)
                        {
                            if(reelStatus[j][k] == "F")
                                winItem.WinPosition[j][k] = 1;
                            }
                        }
                    winInfos.Add(winItem);
                }
                else
                {
                    newResultContext["ReelPay"]     = newReelPay;
                    newResultContext["IsRespin"]    = newIsRespin;
                    newResultContext["RespinReels"] = respinReels;
                }
                JArray udsOutputWinLine = JArray.FromObject(winInfos);
                newResultContext["udsOutputWinLine"] = udsOutputWinLine;
                newResultContext["GamePlaySerialNumber"]    = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds / 10 + 455000000000);

                CQ9Actions currentAction = CQ9Actions.NormalSpin;

                BaseCQ9SlotSpinResult newResult = new BaseCQ9SlotSpinResult();
                newResult.TotalWin      = totalwin;
                newResult.ResultString  = JsonConvert.SerializeObject(newResultContext);
                newResult.MessageID     = lastResult.MessageID;
                newResult.Action        = currentAction;

                double odd = (double)totalwin / (betInfo.TotalBet);
                if (!dicResults.ContainsKey(odd))
                    dicResults.Add(odd, new List<BaseCQ9SlotSpinResult>());
                dicResults[odd].Add(newResult);
            }
            return dicResults;
        }
    }
}
