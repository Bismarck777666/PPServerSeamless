using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class QueenOfAtlantisBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 50.0f; }
        }
    }

    class QueenOfAtlantisGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs1024atlantis";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return true;
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 1024; }
        }
        protected override int ServerResLineCount
        {
            get { return 50; }
        }
        protected override int ROWS
        {
            get
            {
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=3,5,4,8,1,10,6,10,5,7,8,9,6,9,8,6,7,8,9,10&cfgs=1288&ver=2&reel_set_size=3&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1;14~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.01&pos=0,0,0,0,0&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&n_reel_set=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;100,50,25,5,0;75,50,25,0,0;75,50,25,0,0;75,30,15,0,0;75,30,15,0,0;50,25,10,0,0;50,25,10,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,2,0;0,0,0,0,0&rtp=94.91&reel_set0=7,6,9,8,12,6,3,3,3,3,3,3,3,3,7,4,11,13,7,13,13,8,10,10,10,8,6,11,9,6,12,11,4,7,8,13,4,11,13,5,13,13,6,7,1,9,7,6,8,8,9,11,12,5,11,1,10,10,5,5,10,12,7,4,6,13,4,8,4,5,9,13,11,13,13,12,12,10,9,13,10,9,10,8,8,10,11,13,10,8,13,12,12,6,4,7,6,6,4,9,8,1,8,5,13,13,10,13,7,10,10,7,9,4,13,13,10,13,8,4,6,4,11,5,9,6,5,5,7,11,8,9,1,9,11,11,10,11,13,6,7,8,11,11,7,11,12,10,7,10,7,5,5,13,10,13,1,8,5,10,4,7,5,6,10,5,8,12,9,12,12,10,4,11,13,10,13,7,3,3,3,3,3,3,3,3,9,8,4,8,11,13,10,6,5,4,11,1,7,6,8,8,12,6,12,5,10,7,10,6,8,13,6,9,4,8,12,11,4,6,10,5,7,9,11,5,6,5,7,1,4,8,11,11,6,7,5,8,4,5,11,8,8,10,7,4,9,8,5,1,4,12,10,8,4,5,4,13,8,13,10,4,10,12,8,9,11,9,4,8,11,5,13,4,11,11,12,7,10,13~6,11,6,8,4,11,3,3,3,3,3,3,3,3,11,6,7,9,6,11,8,2,6,10,10,13,6,6,13,7,13,6,10,12,2,5,13,7,4,5,6,5,5,4,1,5,7,9,4,10,9,9,8,5,12,1,12,5,13,13,8,7,9,13,6,6,11,8,2,5,8,7,6,11,8,4,11,4,8,12,7,11,10,13,13,10,2,10,9,5,12,7,8,9,5,9,12,7,9,6,6,1,8,12,12,10,10,5,4,11,4,4,11,11,12,5,2,12,4,7,4,8,9,12,10,12,5,8,11,5,12,6,1,11,6,13,13,12,13,7,5,10,12,2,12,7,4,10,5,11,9,6,11,6,9,11,1,9,11,13,5,7,4,13,5,13,13,2,8,5,7,13,11,11,9,13,10,10,3,3,3,3,3,3,3,3,6,4,10,8,5,13,4,6,13,8,13,1,12,13,13,12,11,2,11,5,7,6,9,2,10,8,12,10,4,12,8,11,13,13,11,2,9,9,8,13,9,4,10,1,5,9,10,12,5,4,2,9,7,13,6,13,7,9,5,10,5,6,11,1,12,11,9,11,6,11,5,2,4,4,9,8,4,11,11,9,5,4,12,2,6,9,13,4,6,10,4,7,5,13~10,8,13,10,12,4,3,3,3,3,3,3,3,3,12,6,8,13,13,12,12,2,7,5,10,13,9,5,9,6,13,5,10,8,2,13,13,4,11,4,8,12,12,13,1,12,8,9,13,6,7,5,9,13,7,1,11,12,10,10,12,11,11,6,9,4,6,6,12,11,5,13,13,9,11,9,7,4,12,5,10,4,10,11,11,13,2,12,13,12,7,11,7,13,8,7,13,13,13,8,5,1,12,12,4,9,9,9,7,11,6,4,6,8,4,7,2,11,5,10,8,11,12,12,6,6,5,9,9,10,7,5,1,6,10,6,9,12,7,13,12,4,5,2,5,10,9,4,5,4,7,5,12,10,12,11,1,11,12,6,6,12,6,6,4,6,11,2,9,10,8,7,11,11,13,9,10,13,3,3,3,3,3,3,3,3,7,6,13,12,11,13,4,7,11,13,13,1,4,13,4,7,12,2,9,7,10,12,7,2,5,10,12,5,10,12,6,11,7,11,6,2,7,7,9,12,5,8,5,1,13,11,6,12,9,6,2,8,10,8,10,11,12,10,8,8,12,7,10,1,13,5,4,12,6,11,12,2,12,10,5,8,9,9,7,6,8,7,5,2,7,10,12,12,6,9,8,11,10,13~13,9,8,13,12,6,3,3,3,3,3,3,3,3,7,7,8,7,10,10,4,2,11,11,9,10,13,10,5,7,4,10,8,13,2,4,11,6,5,11,8,12,10,12,1,9,11,8,8,12,11,7,9,9,10,1,6,7,13,13,7,8,4,12,5,8,13,13,8,7,9,6,9,9,11,11,12,6,6,12,8,4,11,7,12,6,2,5,11,12,12,13,8,8,5,10,6,6,12,8,10,1,8,11,9,9,9,13,10,9,13,12,11,7,13,13,2,11,7,6,6,11,5,12,11,10,8,4,4,12,4,8,1,8,10,7,6,7,9,7,6,5,4,2,7,8,7,5,12,11,9,7,4,6,6,9,1,11,10,8,6,12,12,10,10,4,10,2,13,12,7,4,10,6,13,13,4,13,3,3,3,3,3,3,3,3,10,11,4,5,9,13,5,6,5,4,5,1,13,9,6,12,12,2,9,9,13,13,5,2,13,13,8,11,10,9,12,10,9,13,7,2,5,5,8,10,7,9,13,1,13,5,12,10,7,4,2,4,13,5,4,12,8,7,10,10,5,11,12,1,11,11,5,13,12,11,12,2,8,13,6,13,11,11,4,10,5,4,5,2,6,10,13,12,6,5,4,11,10,13~13,4,9,6,6,12,3,3,3,3,3,3,3,3,5,7,13,4,7,11,10,12,12,5,6,5,5,8,12,11,6,5,4,8,5,7,12,7,9,6,11,9,11,12,9,13,8,8,13,10,7,13,6,5,9,14,7,12,6,7,4,6,8,10,12,13,7,12,11,12,9,13,12,10,6,7,10,4,5,8,8,9,13,5,5,10,13,11,6,11,9,7,4,13,7,10,12,8,12,13,13,8,11,13,5,6,6,6,13,12,10,9,11,12,13,8,12,7,12,10,6,13,5,8,9,12,9,12,12,6,10,5,13,13,4,12,6,4,8,9,6,12,10,11,11,10,10,12,6,5,13,11,7,8,13,11,1,10,6,12,9,10,10,9,4,5,11,13,10,6,8,5,5,10,12,8,4,9,3,3,3,3,3,3,3,3,9,9,6,13,12,12,9,11,10,4,13,12,11,9,13,10,13,6,13,8,10,11,5,11,13,7,8,8,4,8,12,8,13,11,8,9,6,4,9,6,12,5,13,7,7,11,8,13,13,9,8,12,11,12,7,7,9,10,8,12,7,11,5,1,9,10,7,4,11,5,10,4,12,11,8,12,13,10,7,13,10,11,13,8,11,9,13,4,11,10,4,11,10,13&reel_set2=7,6,9,8,12,6,3,3,3,3,3,3,3,3,3,4,11,5,7,4,13,3,3,3,3,3,3,8,5,10,4,8,5,11,4,6,12,11,4,7,8,13,4,3,3,3,3,3,3,3,3,3,3,11,13,5,13,13,6,7,1,9,3,3,3,3,3,3,3,5,3,1,10,10,5,5,10,12,7,4,6,13,4,8,4,5,9,13,11,13,13,12,12,10,9,13,10,9,10,8,8,10,3,11,13,10,8,13,12,12,6,4,7,6,6,4,9,8,1,8,5,13,13,3,3,3,3,3,3,3,10,10,7,9,4,13,13,10,13,8,4,6,4,11,5,3,3,3,3,3,7,11,8,9,1,9,11,11,10,11,13,6,7,8,3,11,11,7,11,12,10,7,10,7,5,5,13,10,13,1,8,5,10,4,7,5,6,10,3,5,8,12,9,12,12,10,4,11,13,10,13,7,3,3,3,3,3,3,3,3,3,9,8,4,8,11,13,10,6,5,4,3,3,11,3,3,3,1,3,3,7,6,8,8,12,6,12,5,10,7,10,6,8,13,6,3,3,3,3,3,3,3,3,3,4,6,10,5,7,9,11,5,6,5,7,1,4,8,11,11,6,7,5,8,4,5,11,8,3,3,3,3,3,3,3,3,3,3,8,3,10,3,7,4,9,8,5,1,4,12,10,8,4,5,4,13,8,13,10,4~6,11,6,8,4,11,3,3,3,3,3,3,3,3,11,3,3,9,6,11,8,3,3,3,3,3,3,3,6,10,10,13,6,6,13,7,13,6,10,12,7,3,5,3,3,3,3,13,3,3,3,3,7,3,4,5,6,5,5,4,1,3,3,3,3,3,3,9,3,3,8,5,12,1,12,5,13,13,8,7,9,3,13,3,3,6,11,8,13,5,8,7,6,11,8,4,11,4,8,12,7,11,10,13,13,10,13,10,9,5,12,7,8,9,5,9,12,7,9,6,6,1,3,3,3,3,3,3,3,4,11,4,4,11,11,12,5,3,12,4,7,4,8,3,3,3,3,3,3,5,8,11,5,12,6,1,11,6,13,13,12,13,7,5,10,12,7,12,7,4,10,5,11,9,6,11,6,9,11,1,9,11,13,5,7,4,13,5,13,13,3,8,5,7,13,11,11,9,13,10,10,3,3,3,3,3,3,3,3,3,3,3,3,6,4,10,8,5,13,4,6,13,8,3,13,1,12,3,13,13,12,11,3,11,5,7,6,9,3,10,8,12,10,4,12,8,3,3,3,3,3,3,3,3,9,9,8,13,9,4,10,3,1,5,9,10,3,3,3,3,3,3,3,7,13,6,13,3,7,3,3,3,3,3,3,3,3,3,3,9,5,10,5,6,11,1,12,11,9,11,6,11,5,2,5,4,9,8,4,11~10,8,13,10,12,4,3,3,3,3,3,3,3,3,3,6,8,13,13,12,12,3,3,3,3,3,3,8,7,5,10,13,9,5,9,6,13,5,10,8,6,13,3,3,3,3,3,13,3,3,3,3,4,3,11,4,8,1,12,13,3,3,3,3,3,3,9,13,6,7,5,9,13,7,1,11,12,10,10,12,11,6,9,11,6,9,4,6,6,13,11,5,13,13,9,11,9,7,4,12,5,3,10,4,10,3,3,3,3,3,6,12,13,12,7,11,7,13,8,7,3,13,3,3,3,3,3,3,3,3,4,9,9,9,7,11,6,4,6,8,4,11,3,3,3,3,3,3,11,12,12,6,6,5,9,9,10,7,5,1,6,10,6,9,12,7,3,3,3,3,13,12,4,5,5,5,10,9,4,5,4,3,7,5,12,3,10,12,11,1,11,12,6,6,12,6,6,4,6,3,3,9,10,8,7,11,11,13,9,10,13,3,3,3,3,3,3,3,3,7,6,13,12,11,13,3,4,7,11,13,13,1,4,13,4,7,12,9,9,7,10,12,3,3,5,10,12,5,10,3,3,3,3,3,3,3,7,7,9,3,3,3,3,3,5,1,13,11,6,12,9,6,7,8,10,8,3,10,3,3,3,3,3,3,3,3,3,3,11,3,3,12,10,8,8,12,7,10,1,13,5,4,12,6,11,12,2,12,3,5~13,9,8,13,12,6,3,3,3,3,3,3,3,3,7,3,8,7,10,10,4,3,3,3,3,3,3,7,11,11,9,10,13,10,5,7,4,10,8,13,3,3,7,3,3,3,3,4,3,3,3,3,3,11,6,5,11,8,12,10,12,1,9,11,3,3,3,3,3,3,11,7,9,9,10,1,6,7,13,13,7,8,4,12,6,5,8,13,13,3,8,7,9,6,9,9,11,11,12,6,6,12,8,4,11,7,5,12,6,5,5,11,12,12,13,8,8,5,10,6,6,12,8,10,1,8,11,9,9,9,13,10,9,13,12,11,7,13,13,11,3,3,3,3,3,3,3,12,11,10,8,4,4,12,4,8,1,8,10,7,6,7,9,7,6,5,4,3,7,7,8,7,5,12,11,9,7,4,6,6,3,9,1,11,10,8,6,12,12,10,10,4,10,6,13,12,7,4,10,6,13,13,4,13,3,3,3,3,3,3,3,3,3,10,11,4,5,9,13,5,6,5,4,5,1,13,3,3,3,9,6,12,12,8,9,9,13,13,5,3,3,13,8,11,10,9,12,10,9,13,7,9,5,5,8,10,7,3,3,3,3,3,3,3,12,10,3,3,7,4,8,4,13,5,4,12,8,3,7,3,3,3,3,3,3,3,3,3,3,10,3,10,5,11,12,1,11,11,5,13,12,11,3,2,8,13,6,13,11,11~13,4,9,6,6,12,3,3,3,3,3,3,3,3,3,7,13,4,7,11,10,3,3,3,3,3,3,12,12,5,6,5,6,8,12,11,6,5,4,8,5,7,3,3,3,3,3,3,3,3,3,3,12,7,9,1,11,9,11,12,9,3,3,3,3,3,3,3,7,13,6,5,9,4,7,12,6,7,4,6,8,10,12,13,7,12,7,12,9,13,12,10,6,7,10,4,5,8,8,9,13,5,3,3,3,3,3,11,6,11,9,7,4,13,7,10,12,8,12,13,13,8,11,13,5,5,10,13,11,6,13,12,10,9,11,12,13,8,12,7,12,10,6,13,6,5,8,9,3,12,9,12,12,6,10,5,13,13,4,12,3,3,6,4,8,9,6,12,10,11,11,10,10,12,6,5,13,11,7,8,13,11,3,3,3,1,10,6,3,12,9,10,10,9,4,5,11,13,10,6,8,5,5,3,10,3,3,12,3,8,4,9,3,3,3,3,3,3,3,3,9,3,9,3,6,13,3,12,12,9,11,10,4,13,12,11,9,13,10,13,6,13,8,10,11,5,11,13,7,8,8,4,8,12,8,13,11,8,9,6,4,9,6,12,5,13,7,3,3,11,8,13,13,9,8,12,11,12,3,3,3,3,3,3,3,3,3,3,7,3,7,9,10,8,12,7,11,5,1,9,10,7,4,11,5,10,4,12,11,8&t=243&reel_set1=7,6,9,8,12,6,3,3,3,3,3,3,3,3,7,4,11,13,7,13,13,8,10,10,10,8,6,11,9,6,12,11,4,7,8,13,4,11,13,5,13,13,6,7,1,9,7,6,8,8,9,11,12,5,11,1,10,10,5,5,10,12,7,4,6,13,4,8,4,5,9,13,11,13,13,12,12,10,9,13,10,9,10,8,8,10,11,13,10,8,13,12,12,6,4,7,6,6,4,9,8,1,8,5,13,13,10,13,7,10,10,7,9,4,13,13,10,13,8,4,6,4,11,5,9,6,5,5,7,11,8,9,1,9,11,11,10,11,13,6,7,8,11,11,7,11,12,10,7,10,7,5,5,13,10,13,1,8,5,10,4,7,5,6,10,5,8,12,9,12,12,10,4,11,13,10,13,7,3,3,3,3,3,3,3,3,9,8,4,8,11,13,10,6,5,4,11,1,7,6,8,8,12,6,12,5,10,7,10,6,8,13,6,9,4,8,12,11,4,6,10,5,7,9,11,5,6,5,7,1,4,8,11,11,6,7,5,8,4,5,11,8,8,10,7,4,9,8,5,1,4,12,10,8,4,5,4,13,8,13,10,4,10,12,8,9,11,9,4,8,11,5,13,4,11,11,12,7,10,13~6,11,6,8,4,11,3,3,3,3,3,3,3,3,11,6,7,9,6,11,8,2,6,10,10,13,6,6,13,7,13,6,10,12,2,5,13,7,4,5,6,5,5,4,1,5,7,9,4,10,9,9,8,5,12,1,12,5,13,13,8,7,9,13,6,6,11,8,13,5,8,7,6,11,8,4,11,4,8,12,7,11,10,13,13,10,13,10,9,5,12,7,8,9,5,9,12,7,9,6,6,1,8,12,12,10,10,5,4,11,4,4,11,11,12,5,2,12,4,7,4,8,9,12,10,12,5,8,11,5,12,6,1,11,6,13,13,12,13,7,5,10,12,7,12,7,4,10,5,11,9,6,11,6,9,11,1,9,11,13,5,7,4,13,5,13,13,2,8,5,7,13,11,11,9,13,10,10,3,3,3,3,3,3,3,3,6,4,10,8,5,13,4,6,13,8,13,1,12,13,13,12,11,2,11,5,7,6,9,2,10,8,12,10,4,12,8,11,13,13,11,2,9,9,8,13,9,4,10,1,5,9,10,12,5,4,2,9,7,13,6,13,7,9,5,10,5,6,11,1,12,11,9,11,6,11,5,2,4,4,9,8,4,11,11,9,5,4,12,2,6,9,13,4,6,10,4,7,5,13~10,8,13,10,12,4,3,3,3,3,3,3,3,3,12,6,8,13,13,12,12,2,7,5,10,13,9,5,9,6,13,5,10,8,6,13,13,4,11,4,8,12,12,13,1,12,8,9,13,6,7,5,9,13,7,1,11,12,10,10,12,11,11,6,9,4,6,6,13,11,5,13,13,9,11,9,7,4,12,5,10,4,10,11,11,13,6,12,13,12,7,11,7,13,8,7,13,13,13,8,5,1,12,12,4,9,9,9,7,11,6,4,6,8,4,7,2,11,5,10,8,11,12,12,6,6,5,9,9,10,7,5,1,6,10,6,9,12,7,13,12,4,5,5,5,10,9,4,5,4,7,5,12,10,12,11,1,11,12,6,6,12,6,6,4,6,11,2,9,10,8,7,11,11,13,9,10,13,3,3,3,3,3,3,3,3,7,6,13,12,11,13,4,7,11,13,13,1,4,13,4,7,12,2,9,7,10,12,7,2,5,10,12,5,10,12,6,11,7,11,6,2,7,7,9,12,5,8,5,1,13,11,6,12,9,6,7,8,10,8,10,11,12,10,8,8,12,7,10,1,13,5,4,12,6,11,12,2,12,10,5,8,9,9,7,6,8,7,5,2,7,10,12,12,6,9,8,11,10,13~13,9,8,13,12,6,3,3,3,3,3,3,3,3,7,7,8,7,10,10,4,8,11,11,9,10,13,10,5,7,4,10,8,13,7,4,11,6,5,11,8,12,10,12,1,9,11,8,8,12,11,7,9,9,10,1,6,7,13,13,7,8,4,12,5,8,13,13,8,7,9,6,9,9,11,11,12,6,6,12,8,4,11,7,12,6,5,5,11,12,12,13,8,8,5,10,6,6,12,8,10,1,8,11,9,9,9,13,10,9,13,12,11,7,13,13,2,11,7,6,6,11,5,12,11,10,8,4,4,12,4,8,1,8,10,7,6,7,9,7,6,5,4,7,7,8,7,5,12,11,9,7,4,6,6,9,1,11,10,8,6,12,12,10,10,4,10,2,13,12,7,4,10,6,13,13,4,13,3,3,3,3,3,3,3,3,10,11,4,5,9,13,5,6,5,4,5,1,13,9,6,12,12,2,9,9,13,13,5,2,13,13,8,11,10,9,12,10,9,13,7,2,5,5,8,10,7,9,13,1,13,5,12,10,7,4,2,4,13,5,4,12,8,7,10,10,5,11,12,1,11,11,5,13,12,11,12,2,8,13,6,13,11,11,4,10,5,4,5,2,6,10,13,12,6,5,4,11,10,13~13,4,9,6,6,12,3,3,3,3,3,3,3,3,5,7,13,4,7,11,10,12,12,5,6,5,5,8,12,11,6,5,4,8,5,7,12,7,9,6,11,9,11,12,9,13,8,8,13,10,7,13,6,5,9,1,7,12,6,7,4,6,8,10,12,13,7,12,7,12,9,13,12,10,6,7,10,4,5,8,8,9,13,5,5,10,13,11,6,11,9,7,4,13,7,10,12,8,12,13,13,8,11,13,5,6,6,6,13,12,10,9,11,12,13,8,12,7,12,10,6,13,5,8,9,12,9,12,12,6,10,5,13,13,4,12,6,4,8,9,6,12,10,11,11,10,10,12,6,5,13,11,7,8,13,11,1,10,6,12,9,10,10,9,4,5,11,13,10,6,8,5,5,10,12,8,4,9,3,3,3,3,3,3,3,3,9,9,6,13,12,12,9,11,10,4,13,12,11,9,13,10,13,6,13,8,10,11,5,11,13,7,8,8,4,8,12,8,13,11,8,9,6,4,9,6,12,5,13,7,7,11,8,13,13,9,8,12,11,12,7,7,9,10,8,12,7,11,5,1,9,10,7,4,11,5,10,4,12,11,8,12,13,10,7,13,10,11,13,8,11,9,13,4,11,10,4,11,10,13";
            }
        }
	
	
        #endregion
        public QueenOfAtlantisGameLogic()
        {
            _gameID = GAMEID.QueenOfAtlantis;
            GameName = "QueenOfAtlantis";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string strInitString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, strInitString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            QueenOfAtlantisBetInfo betInfo = new QueenOfAtlantisBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new QueenOfAtlantisBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                QueenOfAtlantisBetInfo betInfo  = new QueenOfAtlantisBetInfo();
                betInfo.BetPerLine              = (float)message.Pop();
                betInfo.LineCount               = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BasePPSlotGame::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }

                if (!isNotIntergerMultipleBetPerLine(betInfo.BetPerLine, minChip))
                {
                    _logger.Error("{0} betInfo.BetPerLine is illegual: {1} != {2} * integer", strGlobalUserID, betInfo.BetPerLine, minChip);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1} != {2}", strGlobalUserID, betInfo.LineCount, this.ClientReqLineCount);
                    return;
                }
                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount  = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                int index   = (int) message.Pop();
                int counter = (int) message.Pop();
                int ind     = (int) message.Pop();
                GITMessage responseMessage  = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin              = 0.0;
                string strGameLog           = "";
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotSpinResult    result          = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo       betInfo         = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse  actionResponse  = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        if (dicParams.ContainsKey("status"))
                        {
                            string[] strParts = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if(ind != 0)
                            {
                                string strTemp  = strParts[ind];
                                strParts[ind]   = strParts[0];
                                strParts[0]     = strTemp;
                            }
                            dicParams["status"] = string.Join(",", strParts);
                        }
                        if (dicParams.ContainsKey("wins"))
                        {
                            string[] strParts = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if (ind != 0)
                            {
                                string strTemp  = strParts[ind];
                                strParts[ind]   = strParts[0];
                                strParts[0]     = strTemp;
                            }
                            dicParams["wins"] = string.Join(",", strParts);
                        }
                        if (dicParams.ContainsKey("wins_mask"))
                        {
                            string[] strParts = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if (ind != 0)
                            {
                                string strTemp  = strParts[ind];
                                strParts[ind]   = strParts[0];
                                strParts[0]     = strTemp;
                            }
                            dicParams["wins_mask"] = string.Join(",", strParts);
                        }
                        result.BonusResultString = convertKeyValuesToString(dicParams);

                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS || 
                            (nextAction == ActionTypes.DOSPIN && !betInfo.HasRemainResponse))
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win = realWin;
                            }
                            resultMsg                   = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID  = betInfo.BetTransactionID;
                            resultMsg.RoundID           = betInfo.RoundID;
                            resultMsg.TransactionID     = createTransactionID();
                            if (_dicUserFreeSpinInfos.ContainsKey(strGlobalUserID) && !_dicUserFreeSpinInfos[strGlobalUserID].Pending)
                            {
                                resultMsg.FreeSpinID = _dicUserFreeSpinInfos[strGlobalUserID].FreeSpinID;
                                addFreeSpinBonusParams(responseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID, realWin);
                            }

                            if (nextAction == ActionTypes.DOSPIN)
                            {
                                saveHistory(agentID, strUserID, index, counter, userBalance, currency);
                                if (_dicUserFreeSpinInfos.ContainsKey(strGlobalUserID) && !_dicUserFreeSpinInfos[strGlobalUserID].Pending)
                                    checkFreeSpinCompletion(responseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID);
                            }
                        }
                        copyBonusParamsToResult(dicParams, result);
                        result.NextAction = nextAction;
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;

                    saveBetResultInfo(strGlobalUserID);
                }
                if (resultMsg == null)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg, Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in QueenOfAtlantisGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "n_reel_set", "s" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]) || resultParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            return resultParams;
        }
    }
}
