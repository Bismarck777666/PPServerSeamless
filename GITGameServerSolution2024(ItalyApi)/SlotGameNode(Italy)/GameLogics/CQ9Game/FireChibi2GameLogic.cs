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
   
    class FireChibi2GameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "140";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 50;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 60, 120, 250, 360, 600, 1200, 2500, 1, 2, 3, 5, 10, 20, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 2500;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "ed6ce8bf0c2d7bc1qv0wO3D27j+g8Xybe8jStHNqWolnjFkduizFB2FeRcrP9+uE+7Kt1dX5dyXTR6LSh5gKlIPeFkFx9ANK9hNhCk6RVqYCID7eXw0Xo2tNjkSbjEMVezydbWBjKjWiIMy07YS4pkj97FD+muDpmkYdfru0V6HpWrPPucB7M5R9t6fJnhGa5FeT8NVvoRnx/BsNofeZCaRyDiE4sn6zprYEjtAOFJEbw2MDu1CG7TAnqwDdXQBaSaSHPk9w0kakI6X8Xn4lvuZZxw2uxg+70fx+8Aqj9sdxRxgSnnpVCCe2Nj/SOjEXY4/rRLl+4nmXLMPvPDoWYSNxc2fcIW+KriCPEBgqc3KUWnSKJH1iq3hVqIO2OlNTfAJWCPoyVHnPpO73Ej60C91Kq993J+lSUV5mzePqqvNmaedJws2kWsAMUcGeH2abZ8EIpfBjIw62O8tXOL1F4S7HFjW8GDs/qNXw8AlMfUwkgJ6c5vxziW8B1ryK6WxK0uDoJb6XkZAilDBJuvZTUE9MLgxSgSbhf5bsPX6pkZ0tXghCpwia1DAq6FANTSTsuDpAdSyLPegh3l8UYqdx1zxIsliV2SPqeh93ojRctuEPk9Ez68sx66zzf/Ib3A7uqkjqLjiIkYPouadGxBBW/bbkPCCCsXYE13cc4vLDZXO7ib3exfpPJ7a4tOQt93PawzCEXvqhNqk/Kp5FCIMQk0Jk8XF6XLp7+vyD6S7+jV8esV5pxHYCcbk+XHTtR7AIL+Q9PcgAekme+HHS6h5MsItXo8aH+3uyI6zr1zuz+Bz5FIHnLQLK8QnmvDwJb6HUPVgRj+PE1oPus/TiULySc2q4Vjpcek1kxd11QWf6Nryk2NDnW8OzSof4tpmxryuxfP0KTKS7E7CjCaUCtCnde+OoWsk4EQTvrRZC6oTAs8pnBYpzmfT5xnjDobXN4ARtgSlcyOP/Ew4lpNuC2sQUVuZM1lHBK2ReZZIEp4dlELZ1NPt3sdurOzcGPoZ/hjrC3XRZ2HaeKAtweZ7UF+IvRhvE55QWXnmZ1zem2mlKU7oe3U7sKZJlO0DRgcCrd5Jq9rxmmwHml9HIFyx1Qjq1cXq4RK7uTLKVNudYYjuJevFMBvNI4RCyJIAuDp9aZDErryrfvdfGJIFKOoAHuucUUnwBVct2JtLUE6ZKxg1wF8aJva6YvlrQf9fSxYbhqAF7qZgyJRWL1KHlWcYS2yfMkkiCt8/77V7mWq7cDMS35ZpSvtzqlEQON6M5kG32dakqLbZIRXeMfX7NpmkoqS1H9XfNHCDQ312Zmv3g+mxeSIiqBxqdEKUUQUvVbHXtEFv0IqHg6Ko2rAfc9J6JGSog0GYxmr81jglnKkJSFqTPN1i4FYvuyC/b/3cX+M3t76KYGcM+R6r/nkMmgKp11eO0xjn7ss2Oqcn9yH1+i06QZcsOD0YBdO77/74paXNomNPG5XwdBgwoxPrmqrT1mssb01wZVb/sQnDu9E+4TagZNMkIlxQTxDFXGd4QpZVDDo2zcCVb5DFSBqRZxyP0TJiPXYkN4AdlvE5oaf4EGH4FZf147fZwlzyHU9ipH/DvoDDWsnXsODhcGh0Vx6jODaRPTtPsNd1j1GDST8Mylcy6WzAMbSiOB+L8JgNEaM5lEhf5+LLgIUD9DbTrZ75l4tFL7ApCVfUVxrvFykbjnWM2k3ka5BGDueeBE4G+1ypLq5jblJTo3qw2A0Vf+F57S4WiQBrZG4HOMZDZPamalt5RywRiTd/QuKFYQQT4+GRs3mOSNDzXl+QRsCNln/6pyyR/UVwYMQpVvlAdRvVbLVRrz5zy57NdrCXk4jiB36NfSBHhEdScO26fY/XwhLS9r2rQj3pYwdMH58kPyukkg5C1KiPVExc8Mmq5co7ko/8roobafiOsB3bLdkcxwrnEMincvBFroBiCT4LXpaZR5oW8/RiQtuBJmkr7C96eM/QTqVJP1IsrbYV3lPVqygUyTnm1z6Ivr9TB+DCFXd7tIEF8ZZQKAV3RdJYVMOu2FhzMNH6G1o9H4wOP8d1MGdP4ymY3sJmBFkisuJA6blRLnVWzRU+gmRn1XIYLKfGUUc9JN4tZ3DHMg5BOKPM4Pxs+ZSq9fIfGiF/0SAaS2+49tpZIocWh90f2CpRAYTNX1nVDQRcm1YS9QLuVgj/ThseNWalD0GQnOuvbsZ+xGjbqb6VbjMkJZb7yUnHvXvu+XFA/ECRgg3Mn1QKaDDRkDUiyAGq2RwDcoEbNGPzKZoQnckfcGyTGqE9QfTNL+J1Zbafyda5qnrTPjF4S2CXOROe6OXK5PmxhAIS/eOERGKYZDJTXy8qczVT8r4HxBYd2EjBfN1KZIlErT0u28GBfPdXH5VHfc3gh69oxY+ju5IsUvzWUBqyQ1DdGKnr8kTPhfsHxRyjguYOksaIsYzIB+Bxpuxz17IbZWi9s4KXTywuAHv8c885UPB/VmZJ3+iH8bfhJT7+jg7oL4yKDRSapQ3e8maYHB/Ja3fgV8gBzB9eWd+xnzzWl7f3Hxsh1WWKqU+mf+TfTSzov0K5tfgRdNIEuEB8i7+7bdL9L+f2ReHb8lJFMbUmo4mLRIu9dI09is7NH5voHgYS5bTxvVa6Yli/2YS+wNSYStBNobntqkOIZMKA2h1leTEFR/plVs9VpQLe+yQxtp/L0+qgKT94krYuanwZ3vVVnLcbeN1lPmdjl5OMn3jF9yOanoFMAhTOj+DxbVlZAdlTHw7ysuUaYHnDvYCrDJT26yxz+9JknUalY6TF78KmyObKe+W9wdqHBJKYTyIG2fiCA7cXGNtVQKraVyMgtL5KmFYJCRkMHQZKWKEDCWGT+h8/gcVr7Bu0KrCaPQbMoQamrR3cGWgJkLUFGSi3lbgPrI23ywQZOT3SBAGvjyYzpuaWfxqcYpwOvCUlGRLSToM/GdcsL4bpL0d/n//cP6R5cNn4UWqkLa0M2Pma/+pYPOrPQT8Xsvtxu9ZxWsINa1cZB9C5eyZcW3OF9Jm1NukUf/uhwdM31VAYGRm8qn2KYEZfQdcWHaPx/slbztraU76toYFTXZXk45T8B9kl8EmooXq+3b0aAQQwFpkk06iZK/+St9Bxc2wZfm+BCWdlbonMVs8YGuMzyJ9iaJSR55mzLWFDkb4nXqky5SOL/FucHyn3uIhA/DEr//Wezl0gKO0yDvwAEzONzDQ0GRK/NEG0Sc+Vvl6/aUehAxqp81ckvVjBL6XD0zI8E4nvPRy0BjKGa+/ALrDF4RnzwWRfYtTDCkedThZ9lF2ws28GfaUEXaKsv5jIYjg4Dikd7E9OlMWySdG+PDfUq8E2V1h2knq1dk4vGox24D5HcKAohdk8Sa+JLREdhuu0NrHjT+eEW8l3WQd/yjlJ8H5DWfnhyzJ732qvLMfpfQkr7otus9hA+aIQufUJj9f8NXMr6QR9v3LmuL9MC8URuqhnlwlgj3DZm3y84SKXqYg4ahXTmnfj/0+wDrJg1nmr/pEg2SfWANIXFxZcgdMy+E+bCaL+pEe7hRyqZRV77jVvKPdlkRGN+h3hZH0LRE3+kfJahyd4P854aYnVN1s8thyXSaW4BDwvnGrwGq0MSqRB8bZIaetpfKjBqXcG6ZlwXgOtZOVTEHDBkFuAp9GRL9EIlye+0juxpOlO1voHFQ013A1zFdJYpH6gCw1BVmVmI8dPqEuaQ/3ORvjS2wYmSTnQPg+oOmkyZ/FurUMkzhBuVEdZUwVaIiH4Oo5yTiOENP4pliCxqYrDmcTbWyT2Q5eAI5p5pL/l3oVGBy8ctaaKIgprttd65fAXM7X5OJkJTqo1cY6uDb06t/N2QfmT7Btnj5ho7oKkYrOkOC7wsIRuX709LedZ7ABisUFBaSBYz8HfGqXI1OUir7LcAsAjotI3mPpocVYtKj4FJ2gdVk7cegjUEB99yApDuBtyUPsykHiWXqC8BJLbPSp8yoppp3QR2UBORAXfcs6UJmy5zLkTGn5oQ/IZ+Hs2jKQxSBlzIRWeX7mzd3Xi00p96tLRPE1xQeDa8x+0xBdgKktso62jEIxv3Sf67B/CZNQveV6WBTpZEHh7vuDl8RPJ9J9b0Fq0sMxrvb9qxPwi2jhqEkE8ZsNLiXRVi6aAmHaQO/CAgFW3W+9mu7gq8nB2aCpMnXM3T0iZdQEtvVB5NlZTCpXF6jac4jbMkJDRh5tD7TJYqcYwQRtLcjBNCS9+YNglVM7gmXVbESWENWIeApJt+cJS5natXAkot5+xu+8AjbWsyh85fwACzzyQCyb0/n4VHi7HKFmHEbxlQ22en82M9L1VSVqwsaM9E0UDlSFqTNVMDrYlYATfB6q6884sym/sl26dibgG7LL02//WUfub1RVW6wrkX+FcsLcAb4/yrVqL18r3lFcplitQt/eKlfXaaAVWGJ15wk0sEeMW/QowP/pyG+bNw86iX7QroZ8niuufhDda8wsmNXsxpjzFII43QOlcsRtVmon9FVYK5HU5ll8mpbnfDGP4lahVsC16D6l46URjg66lZ4MoA7GAHaIUu38IIjAbh79cRQIyWjhYrim6W7vNyW7vAHgGQ4U3xE1fBvvrZ7qOpzYuK/4mg39wfPni3M6W85gQMgRRVrnWEEa0KF7exOUuCweD2JeZGwy/BoDYnVnRdo50GSXHE/F+Pwt2n+lBuxbhy/oRYHWn4iOJWnE6ZDn6fJJlZWQlvG4rcAN97nJJ/BYlKnhLZ48qOYmoNjdp28Nbc12w4Dv0ZO6dmq3hox8k6hINf41sIgvWDmODzH1cThp1g/4wXWxnREIK4Lxj96bWvDdhGvreWMtRSqWye5cRe/gOA2zMi/y+Ghzg69L9te33EIxjCYv5ok5Bpd6E+EAfcOhz4cnuN8vpdt4t5jQmwgui2ZIWqiiHcj5EpAvBwUmPt7dOGZBdmTL9mvyuHmeu6816UkzbBp8t0mcyqvT2Ss0DXIRnUbyIk5ztRnWPdVZKsW7+c5uBO2eAGpldaMlkT/4Btzrfgc+TmJ/oCyRqH635YbS3Ay7KaSZ+F/sGMLBpwbPVA04gcW3Qky1Hk1BijxrWos2yOYAE9cSxhJ74dgdpuIhVjHcWtJ8Af346YNC54hfzr6Tn/gidZS0QKj9tSGxCn3gXC63nxVx9Y/FWJQYDX/txmntaga/19U3dIt8QNbtH+M4Qm7dNMRIXZtebbqq8n7OLBBsLClgWFnTYWyjGEEFnXOpGswgS3IB2Tt9oJdLVjdZjM/RqSsTuMzUQf/yg+MugzKxtB8sP851Ci3/Yb/2X99QNDPJMNL39ZpAPhMypV96HHfYGDnRCScKVqBSxpjiQqLzxYq1sG2zLCK0qLcsqM7RnPJNj61u+HIPog1shdk+jb3hnKsXCOrJ7CV0/HTOZ7gAg6ausnroKYyxJZ+6m0RQxVzi9cGVho0RYmyE2Oak0lfOjZZJH8twDGveh53/+h9Z2IPcMZGlnM+u8ibblgvvFm/rKLy9TsCwZEQFSj7bl+8qCXHURt5U9yJhUL0UKuI7a03vIeU0Rp5I+qd5tOPA9jFP3nIE0kAcpwBnUVC1rl6cq3S1g7/zJypdYqXgHFENuX4RbS3GXh+8wSJiTKdb/L3Xv62j7EcB+e78+8XFcG0trbHUqqnGMWBi4Emn9lNK37MAURW0Eu+88ltXnibsxUuWdf+IkTofVNdL147WSm4aPZE/a2v1Bq6hXyY1RgF0MWZt5/wnfOar7c7JZwn0/giGCVK0BQ5683mbiNtqFWrbSosWnsL4j0yZm5Fy38haBBla40RV891V1tj+p0pRgmljCVo0tDiPsUsX75BYTzI5OFw6CoC4lN4aZRTp3gKh/7kgje6WjtHjDANqdMJIph/yaX3kgbWYNtxSvuMzwlreh2h34JQCufAOKt8+ZX+FvHleBU33zFl47MvhlcRU4+1Xmzh3BBFqs3z9qd9dlwgMRk22Pb8Ur252hECIJn0JwYS2DwIDo8RpjPXZ6hibX2hMHzdMXZfzqGfM/+bjJXoSOFffE/jWXu3zj+nCFTWg+/xG2NSvRwvXRM2yAg7TLRHESDqvlUNXe3SeRULn1MokPkvlQ5Q8VDMIqekAHok8IzucZHYWhRK5VPn5uCNjipw7ilLO7QqydrIqSCgSl0rcyEftVjT9yzSfdn2/66IsYlj/FbKkvkTBoZ+oLrLzPpmoTQFmLK7qnpS0uxB7+rsmo+yDhyaKkFAHcDm0QEkcElhq+5wppg3DSe/kP7yht/b295s0yoQ0PpTzAfDD5WPvGGJrKGYnIExZCYHAjP2WzQWS3BDDc3S9g7JeSK0GJEmjWrkuWk4vhRFNMB+Q2FyVyJESL2nJ+omEhUU2AMAfXRaKYcltuvRnEq6KamrRJF5ObGYa3CYDAxawymiQvJXci0DOTruROyCbLsqlGvikLvlUd9WhVOjMwnv0i3YDDUul/ZO0QJCKe3WLeL/3okemvJw0f7/GP8qekB7Lbg4MWxPbm8oXkV6in+dHfVYaNxYFxMur9gg8fkhduhPzNcjWriUcDKayDh0+fVNB3kZVscWgMqpCoAzumLOcNRUZVQMaAwiHv0L+4bFGVpNy+OTJOLH88237iH7B/ur1WK1cDPj8D+UOE3N5UHqWDqvFG4Xvf/TXBM73mC0+FMQG56gXcZRPriXSdeEfrr6sFneDrcismqwbp7Bkg+zr4lN/yh4Rac1mrIuMF166xz7MnUej0QiwmTD4beJR22P19xCNkJj6bAvGH1QfsrjYlAj4mtDjM1SCY4l+oWRxIFE0I5YJW+ocvBg6rzJP/Hg1k9utIF4paSCIjLVCK61CcUs6sKqHKZkzMTu9IuIqaJKpfUaDkm9eIJd9o9aJj+GFWNNlr5vw1SKTXooeC+/tI9smvuR8TvyhKIOv6oJfVnpeXqX8/yg7b1ii1jTReKjZGvyUusD+6dm7EwF0ZAw95f+gomoAwgAsRangYfeuTHc9fn6vFkW7EmlQ0Px7Kg2/dGeFp4zRtQafcFJ0pLe8BmYZq1+P3pw9jgkw/Vzf2AByTuQa6Elt7xUic1KLAiSIRX3SmSnvWcxPK7ySrHcVoKnroWqm2Qfkvh+VHB6FEBrQuMPKxMD+k6+qc1lH+ECLsqTjmUf7p5nrROdxu5Ne7DXgoTTx9N89zq3VsvSfeSc1PxGegn+5JK0TUwl6KAC4XVbrZeRwuzBEhsWCEiW94cLrx6v6ng5HpYwYe2+ooEBnv/lxw2hcrL1hU/OXdFFgiMyEWnjS9ax8bGLnLTFqdIEyWIt27QbvDCx6iMqpKLOdslS5HNEiOfkJO1Jg9E/Oi19CGQw3I29lrh6mb3f3gQPvbLREChVaiu08Jx1QvE4V/jzrn0vV75vaHo2Qfw09kE59tiuAbcJ0KYz5Wf4Ziuv9xbApuX6VqLuuJWA1ylUb/GRmwRAem3R/ynxndIyY8BkNDlTLalbC/3tHk6dIcN62apeqNz9poDddRebHw28kbYF3dXNr1HWbZVvr6ybmOQxh49pvh8J6dQ+b1cdreCti48WNb18V9cLbn0RbjnGiCiFLaAM1afemFzJKna5FCUpOrNkJo2Mb/LSi9zizkDlXgFa8eWeVwU0BhaFmULpcIyxymbnxjHXTSxpXK8bcf22xyYBWUzFYMvtzch/SgdwKZz/zgiKV6wpgXBzmuhYyCFTwdMXATg5TEzsGyjEtrNEPK+jsSpeZuZrl6JLraiKQUBDHfmcSeyaIRSiIEkUwddZx/PP2EOhqaVJus6g8Su9KPX0P24mEuKxfxhEjOoNGOq3fw8cqA2emKRIrqOwU1BBrNL+ZTdI9e6ACOK5lNhLpfnK7qtuGFo3cO9PuBuNnNUojZ3ajWRONem6Z1yCjOfAEVR3bW1Kzpl1jh5e+kq4ImTCRJi2r4M/fhvjrHpE6wlz5vxP6FuERL9Q6JFeu3cABzwpab6q4jyT00qfvE3ejjV6GAdQsw7thw4ZYm/+QieQDqHTBGqdBi7uacWnpI2K06Fj4QYGz5T8gkTkuWxcpEO9OO1T7EDoeieR6srr5TkG0P24oCtQlOyH7jlUW8D4Vz63KPt9mYXQFa6Iy+U6XrACsx67xxITBPL6t4RZbIG+68UTay/yF9zvul8SNBaNAI+R1uad5Ahsmf7FL9X1VK5VjZq9I0f0N6D8RbmXz4kr6RRsJQWYLkx4gONt4QnWV5w3CX3GrBXMbblzvYlDYK+PZ8I1+csauy+JXY4j5CfX6QwNJhdD/QDsz3vn/Iq+cTSHiCQrCZyN6sZB1uNx20GLtGslXbp0XPuP4EwMmC8sVyBrsRACxe93dmZJRyja8gob0Ic9o0yxHG9HzGvSQ7BrGLRhYBu4BfkNpbwIEkAUA2BoG2h7wZ9F1jXeoqiIw+daoZLC7nzvcVfK5g+NzT0IjgrXJkC1IjycxqvgCCYQe4Bz4SzGrd73oDynJ83WKIXCCEX6G9rTLcSR9vO1AChB8ILJc8Eoi7+Wyh0tpeTIHRxSDjECyxWG/hzyyY7yxwPWmICKrt9l2C98761RgJo0w==";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "FireChibi2";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"en\",\"name\": \"Fire Chibi 2\"}," +
                    "{\"lang\": \"th\",\"name\": \"เปลวไฟแห่งความร่ำรวย 2\"}," +
                    "{\"lang\": \"ko\",\"name\": \"파이어 치비 2\"}," +
                    "{\"lang\": \"zh-cn\",\"name\": \"火烧连环船2\"}]";
            }
        }
        #endregion

        public FireChibi2GameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.FireChibi2;
            GameName                = "FireChibi2";
        }
    }
}
