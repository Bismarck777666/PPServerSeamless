using Akka.Actor;
using Akka.Event;
using GITProtocol;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class BookOfTutRespinGameLogic : BaseSelFreePPSlotGame
    {       
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10tut";
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
            get
            {
                return 10;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
        }

        protected override int ROWS
        {
            get
            {
                return 3;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=6,8,5,6,8,5,11,3,3,7,4,8,10,4,10&cfgs=6442&ver=3&def_sb=2,7,6,4,11&reel_set_size=24&def_sa=5,10,2,6,11&scatters=1~200,20,2,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"10638297\",max_rnd_win:\"4000\"}}&wl_i=tbm~4000&reel_set10=11,7,9,1,7,11,10,5,8,11,7,6,10,9,4,8,9,11,7,3,4,8,10,7,9,3,10,9,7,10,6,8,5~5,4,3,8,10,7,1,10,7,11,8,4,8,7,5,9,3,9,6,4,8,10,11,7,10,8,6,11,7,11,9,6,4,10~9,5,6,11,5,7,10,8,10,9,4,1,7,9,8,11,6,10,5,11,4,8,11,3,4,9,6,10,3,5,6~5,3,7,11,10,8,6,9,11,6,9,7,9,11,5,11,9,3,10,8,10,4,10,4,9,4,3,1,8,11,8~3,11,5,7,5,11,7,11,9,11,5,4,7,11,10,1,10,7,10,9,6,11,8,6,8,10,7,9,7,4,8&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&reel_set11=11,3,11,3,9,7,3,7,9,5,7,11,7,5,9,5,11~6,4,8,6,4,8,10,8,6,8,6,4,8,4,10,4,10,6,10,6,10,8~3,1,3,9,3,1,9,5,1,5,1,5,3,7,1,7,9,7,5,9,7,1~7,1,7,11,6,10,4,1,4,1,6,11,10,11,8,1,8,1,6,1,10,7,8,6,1,4,1,10~7,1,3,1,9,7,5,3,1,5,1,5,7,1,9,1,7,1,3,9&reel_set12=10,8,5,8,9,3,6,5,7,11,7,11,9,11,8,9,7,10,8,7,6,10,4,8,10,1,10,4,9~4,8,7,5,7,9,7,8,11,4,10,11,8,3,6,9,8,3,11,6,7,3,5,9,1,3,6,11,7,4,6,10,8,5,6,10,11,5,9,8,10,6,8,4,10,7,9,6,10,7,10,8,1,10,7~11,9,6,7,3,10,6,5,11,9,1,4,3,10,8,6,10,11,5,8,9,5,7,10,9,4,8,5,11,4,9~8,10,11,5,11,7,9,11,5,10,9,4,3,10,11,8,11,9,6,3,1,3,4,6,8,9,7~6,7,8,11,3,10,9,8,11,8,4,11,5,4,10,9,11,10,7,4,6,5,1,10,9,5,7,11,9,7&purInit_e=1&reel_set13=5,9,7,6,4,10,8,11,9,8,6,9,10,8,11,7,3,10,5,4,1,7,9,10,3,10~7,11,9,5,3,7,4,5,6,10,8,10,1,8,11,6,11,7,6,8,9,8,9,3,1,8,11,9,10,4,5,10,9,6,8,7,6,4,11,7,10,4~6,4,5,10,8,9,4,5,3,5,3,8,9,7,11,9,7,10,8,10,5,1,3,11,5,6,5,10,6,7,10,8,6,9,4,9,11,9,1,11,9,4,9,3,6,11~3,1,4,6,9,10,7,8,11,8,5,11,8,11,6,9,4,7,8,11,6,8,1,10,9,5,11,3,9,4,10,5,7,4,11,3,9,3,5,9~10,11,7,11,10,5,10,5,7,6,4,5,4,9,7,1,9,6,3,11,4,8,11,7,8,10,8,9,10,11,10,7&wilds=1~200,20,2,0,0~1,1,1,1,1;2~0,0,0,0,0~1,1,1,1,1&bonuses=0&reel_set18=6,11,7,9,11,6,11,4,5,6,11,8,5,1,10,9,8,10,3,9,7,10,9,7,8,9,4,9,8,10,3,7,10,4,5,11,8~7,11,10,5,8,9,1,3,9,6,10,9,10,7,4,6,4,3,10,8,5,7,11,9,5,8,10,3,7,1,10,9,5,6,8,9,4,11,10,11,6,7,9,5,7,8,9,6,11,3,7~6,7,5,11,8,7,10,1,6,10,9,4,3,6,8,3,4,8,6,8,4,3,11,7,11,10,1,5,9,7,9,10,9,7,9,5,8,11~10,8,1,9,8,5,10,5,3,9,5,3,9,5,8,9,3,6,11,8,11,4,9,11,10,11,4,5,11,7,6,11,4,1,9,6,7,8,3,6,11,9,7,4,9,7,10,11~10,3,9,1,7,9,7,10,11,10,9,8,4,5,4,11,7,8,6,5,9,7,10,9,6,11,7,11&reel_set19=9,11,6,5,3,7,8,6,5,9,8,1,3,6,11,9,7,10,7,5,10,9,11,9,4,10,3,7,8,9,10,6,7,4,8,11,4,10,5~10,6,11,9,10,11,4,10,11,5,4,8,10,7,5,10,11,3,11,7,11,5,7,9,8,7,6,11,10,7,9,8,9,6,5,6,9,7,8,6,1,6,8,3,9,10,8,11,4,3,10,9,3,4~9,10,3,8,10,7,9,11,4,11,9,5,1,7,9,11,8,7,11,8,4,11,10,6,4,5,10,3,5,8,3,10,6~5,4,10,7,4,3,9,8,6,10,3,11,5,10,7,1,9,10,11,8,11,7,6,9,5,8,10~10,9,10,11,9,11,7,5,7,5,3,10,7,4,10,4,7,9,11,6,11,9,4,8,11,10,6,8,1,8,7,10,4,3,5,6,7,11,8,6,5&reel_set14=9,7,4,10,6,8,7,3,7,9,11,9,10,1,10,8,11,9,11,6,5,10,8,5,4,10,8~4,8,10,11,10,9,5,1,6,1,10,9,6,11,7,6,11,10,5,9,5,8,4,3,6,7,8,7,8,7,9,6,10,5,3,11,9,10,4,3~9,4,9,8,10,7,6,3,5,9,6,4,1,10,4,11,10,5,10,3,11,8,7,11,3,9,6,11,9,4,9,11,6,5,7,11,7,5,8,3,11,5,7,1,11~5,9,6,5,10,9,4,3,11,8,3,11,8,11,9,10,9,10,5,11,4,10,4,10,9,6,11,1,3,9,8,4,8,7,1,9,8,7,6,11~4,7,5,7,4,8,6,10,8,4,3,8,7,10,1,11,10,7,5,11,9,5,9,10,9,6,7,8,11,3,11,7,11,8,11,4,9,6,9,10,7,10&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,300,100,0,0;500,150,40,0,0;300,100,30,0,0;300,100,30,0,0;100,30,10,0,0;100,30,10,0,0;50,15,5,0,0;50,15,5,0,0;50,15,5,0,0&reel_set15=7,8,11,4,9,8,9,8,1,10,3,10,9,8,4,5,7,5,10,3,9,10,6,10,6,11,7,11,7,4,9,10,9,7,11,5,8,7,6,3,10,9,8~10,7,5,9,6,11,7,3,9,4,8,11,8,10,11,6,4,7,5,6,4,9,6,8,9,8,10,7,8,11,6,7,4,8,1,3,10,1,5,3,9,10,11,7,5~10,4,11,1,5,8,9,5,8,6,11,3,11,7,10,5,9,10,9,11,9,6,4,7,11,7,9,3,7,6,4~1,8,4,11,9,6,8,11,10,1,4,3,5,11,3,9,7,6,8,3,10,9,5,9,6,10,7,5,4,11,4,11,10,8,9~11,7,10,5,10,8,5,7,9,1,6,8,10,6,4,11,10,9,7,9,11,7,3,11,8,3,4,7,8,10,11,7,4,10,11,9,7&reel_set16=7,10,9,11,9,8,10,9,10,11,1,10,7,8,4,6,3,9,4,10,6,8,4,7,6,10,7,8,11,10,8,5,9,3,11,5,9,11,7,9,5~7,9,5,11,9,11,10,6,9,11,10,7,4,8,9,10,6,3,7,8,4,3,6,5,4,8,10,1,7,5,8,10,8,1,6,10,8~10,4,1,3,5,9,11,9,4,10,8,1,3,5,8,11,5,6,7,11,7,9,6,3,8,6,4,10,6,5,11,9,11,7,8,11,9,7~7,4,11,4,9,4,1,5,9,11,9,8,5,9,11,7,8,10,11,8,6,3,9,6,4,5,1,10,8,3,9,10,11,10,9,10,8,6,11,3,7,5,9,3~3,5,10,5,11,9,10,5,8,7,11,7,3,9,5,7,11,10,6,8,10,9,4,7,4,11,7,4,11,8,9,10,11,7,6,1,7,4,10,6&reel_set17=8,1,5,9,11,10,11,10,5,11,10,7,4,9,3,9,10,9,5,9,4,3,6,4,10,5,11,7,8,7,6,8,7,8,11,3,9,10,8,3,4,9,7,10,8,6,7~9,7,6,5,10,8,7,6,8,6,3,9,11,7,8,1,5,9,5,9,8,11,4,9,11,6,11,10,4,10,5,7,5,4,6,7,5,3,8,10,3,9,4,6,7,10,4,8,9,11,1,11,7,9,6~8,9,7,6,5,11,4,1,11,9,4,10,3,7,5,9,10,3,6,5,11,8,7,8,6,8,11,10~9,10,11,7,8,5,10,5,7,8,9,3,6,11,9,4,10,11,9,3,1,9,8,4,10,4,6,11,7,8,6,3,10,9,11,5,1,3~9,6,7,10,7,9,11,8,3,11,8,10,8,7,9,10,11,4,8,10,7,9,5,10,9,7,11,9,11,7,4,11,5,11,4,7,10,7,4,1,5,11,7,6,3,10,8,6,5&total_bet_max=10,000,000.00&reel_set21=8,7,5,10,7,6,9,5,10,11,9,10,8,7,10,7,5,9,4,7,10,9,4,10,9,4,11,8,7,9,8,6,7,6,8,11,9,1,11,8~4,8,9,4,6,8,7,5,7,6,5,10,1,4,10,7,5,6,8,11,9,4,6,8,4,11,7,11,5,9,7,10,9,10,6,11,10,7,8,11,10,7,1,8,6,10~6,5,4,11,9,4,8,10,9,4,9,8,1,8,6,9,11,4,10,6,11,9,5,7,10,4,6,11,7,5,4,9,5,9,10,6,11,10,5,8,6,10,1,11,9,11,5~8,9,4,7,9,6,11,5,8,1,9,11,5,10,9,10,8,11,10,8,5,4,5,7,9,4,10,4,11,6~8,10,9,4,7,9,11,7,10,4,5,11,9,8,5,7,8,1,11,7,10,6,9,7,10,7,5,10,6,11,4&reel_set22=6,9,5,1,3,10,9,1,8,7,1,11,8,11,10,3,10,1,7,5,1,4,1,4,6,11~4,10,1,6,1,6,9,3,5,1,10,1,11,9,5,1,11,1,7,8,10,11,7,3,8,1~1,10,1,5,7,9,1,9,5,1,8,6,4,10,4,8,11,6,7,3,1,3,1,10,11,1,11~1,4,6,10,1,11,7,9,1,6,4,5,1,7,1,8,11,1,5,1,3,10,8,1,9,10,3,1~6,1,11,5,3,11,1,8,10,1,3,11,1,8,1,4,1,7,4,1,9,5,6,1,10,9,10&reel_set0=8,5,7,10,3,10,1,6,3,4,10,1,7,5,8,7,8,9,11,10,11,8,5,11,7,9,11,7,8,9,4,8,6,11,5,6,9,7,8,9,11,7,9,11,8,10,9,10,11,7,6~3,4,5,11,9,11,10,7,11,8,6,11,9,11,10,11,7,4,8,9,10,8,1,7,9,10,5,8,6,9,7,9,10~9,6,10,11,8,9,10,6,9,6,8,9,8,10,11,7,8,7,11,4,9,10,9,11,8,7,5,10,9,11,8,5,8,5,11,7,3,9,7,4,7,1,11,9,10,8,1,4,11,6,7,4,10,3,5,10,7,11~5,7,4,10,9,6,11,7,9,10,9,10,3,9,7,9,10,8,3,5,7,10,11,8,11,8,11,8,11,1,9,6,7,6,10,3,6,7,4,9,10,9,8,3,11,10,5,3,9,4,5,10,11,8,10~9,4,11,9,4,10,3,8,5,4,8,10,11,10,7,9,5,7,8,7,8,9,10,7,10,8,1,10,9,8,9,11,10,11,3,11,6,3,10,7,8,9,11,8,11,5,6,7,9,10,3,7,9,7,10,8,3,6,7,11,5,8,9,1,7,11,5,11,10,6,9,3,7,8,4,7,10,5,6&reel_set23=10,11,3,8,11,7,9,6,3,7,11,8,5,11,7,8,11,9,10,4,5,4,10,9,10,8,10,5,9,5,10,8,6,8,7,9,7,10,8,7,8,9,7,11~7,4,10,8,11,5,9,8,9,10,5,9,7,8,10,7,6,11,5,10,6,11,8,9,7,9,4,9,3,6,8,11,3,4,10,11~4,9,7,8,4,8,9,7,3,8,9,11,9,10,7,5,11,4,7,9,11,6,10,11,9,6,8,11,8,6,7,11,10,5,3,6,5,10,8~9,3,9,11,10,8,10,11,9,7,11,3,8,4,5,6,11,9,6,11,7,5,7,11,10,7,8,5,4,3,11,4,10,8,6,9,10,9~5,10,8,4,6,11,9,11,6,10,9,5,3,4,9,11,7,9,7,3,7,8,9,10,7,9,8,3,11,8,10,8,7,3,8,11,6,8,4,8,9,10,11,8,6,7,8,5,3,9,11,4,7,10,11,6,10,4,7,10,5,9,7,5,7,8,10,11,10,7,11,9,6,7,9,3,5,10,9,8&reel_set2=11,7,8,11,9,8,11,4,6,3,6,8,11,10,7,8,6,8,11,6,11,5,7,11,8,11,8,9,7,11,8,7,3,1,4,7,8,10,7,5~8,9,10,7,9,4,11,4,7,10,7,8,5,8,6,5,9,7,6,7,8,10,9,7,10,9,11,9,10,4,1,8,3,10,8,10,9,7,6,8,7,6~9,5,11,5,3,6,4,10,11,10,7,11,3,1,11,9,6,9,10,11,5,7,6,4,9,6,9,1,8,10,11,10,9,10,6,10,5,11,5,9,6,9,6,5,4,5,8,10,11~6,11,5,9,11,10,8,5,11,6,11,5,4,5,3,9,11,6,10,11,5,6,5,9,6,5,1,10,9,11,9,5,6,10,1,11,10,9,5,11,10,7,4,10,9,3,8,7,10,4,10,9,6~8,7,10,8,1,6,10,5,7,3,7,10,5,7,6,11,8,9,5,6,10,5,6,5,7,8,10,8,6,10,4,5,6&reel_set1=7,4,11,4,11,4,8,11,8,5,7,9,10,4,8,3,7,10,9,5,6,11,5,7,5,4,8,4,1,8~3,8,5,10,9,5,9,10,11,8,5,9,7,6,9,10,7,9,7,8,7,4,11,10,1,3,8,7,10,4,7,8,10,8~6,9,11,5,9,5,11,10,8,6,11,7,8,6,5,7,1,6,3,11,10,6,5,8,9,3,10,11,3,10,11,5,9,4,9,5,6,5,9,4,10,6,10,9,1~4,9,6,9,11,9,6,10,11,5,9,7,11,10,8,4,11,5,8,10,4,5,9,11,3,8,10,11,5,6,11,6,10,11,6,9,5,6,5,6,7,1,10,6,3,10,11,4,6,9,6,9,7,5,11,5,11,10,5,10,6,1~10,11,4,5,6,5,3,10,5,8,10,7,8,5,7,5,8,10,4,6,5,7,11,6,8,5,6,7,5,6,8,6,7,11,9,6,10,3,8,7,10,1,7,8,10,5,8,7,10,6,7&reel_set4=7,10,7,4,7,9,8,5,6,3,8,1,4,11,7,3,10,7,11,8,11,8,11,7,5,8,11,9~10,8,6,7,8,7,3,10,9,10,9,3,11,7,5,7,6,8,4,9,11,7,10,3,10,8,6,8,7,4,8,9,10,9,10,4,9,8,9,5,1,10,8,9,7,10,9~10,4,3,4,6,7,9,10,6,4,8,4,11,10,11,9,11,9,4,11,9,10,4,9,10,3,5,11,10,11,9,4,9,3,11,4,11,9,5,10,4,3,10,6,1,7~5,1,3,10,11,4,10,9,4,10,4,10,9,11,9,11,4,9,4,8,4,7,9,11,1,9,6,11,9,6,4,10,11,7,11,5,10,11,9,4,8,10,3,9,6,10,3,10~8,10,8,11,3,7,10,7,4,5,7,9,7,10,5,4,8,6,8,7,10,8,10,1&purInit=[{bet:1000,type:\"default\"}]&reel_set3=11,4,1,9,8,7,8,6,7,9,8,6,7,11,10,7,11,8,4,3,7,11,8,10,8,3,5,6,7~3,5,9,7,4,6,8,10,8,10,9,6,9,10,7,9,8,7,8,11,9,8,10,8,10,9,7,9,10,7,5,8,11,7,4,3,10,5,6,7,8,1~4,10,6,9,11,10,3,9,11,6,10,9,11,4,1,6,5,6,4,11,10,5,4,9,11,3,5,11,1,8,9,10,6,4,10,9,6,8,4,11,9,11,9,4,7,10,7,4,10,6,4,10,6~5,10,11,6,9,10,1,4,6,4,3,11,5,10,11,6,11,10,9,11,10,7,6,9,4,11,4,11,6,9,6,9,4,6,10,4,11,9,3,10,9,4,8~4,8,7,8,6,7,8,10,5,8,7,10,9,6,7,10,5,8,6,7,4,7,1,10,4,8,7,11,6,8,6,5,6,7,10,6,8,3,10,11,10,8,9,10,8,10,6,3,7,6,7,10,7&reel_set20=9,7,11,10,11,10,6,10,5,4,10,5,11,10,7,3,8,6,5,9,7,4,11,9,11,9,8,11,6,8,11,7,6,8,1,9,4,5,10,3,8,3,6,8,7,11,4~11,4,5,11,6,11,7,4,8,11,6,5,9,8,10,8,9,8,9,11,5,6,3,7,10,8,6,3,9,1,10,4,1,8,11,7,3,7,10,11,10,9,5,7~9,10,6,11,3,9,10,11,1,8,5,1,4,9,5,7,4,10,11,3,9,7,6,8,5,7,10,11,7,3,4,11,8,9,7,6,4,5,11,10,8,10,3~8,6,11,5,10,6,10,9,11,4,3,11,4,8,7,9,3,7,3,9,7,6,11,10,11,10,7,8,1,5,9,7,5,8,11,1,10,8,10,5,6,9,10,5,4,10~1,7,8,10,11,9,4,8,9,11,7,11,6,7,10,8,5,9,10,6,7,11,7,11,9,10,5,8,9,5,3,4,10,4,6,3,10,7,11,10&reel_set6=11,6,7,11,4,6,9,10,7,9,8,11,8,3,7,9,5,11,9,11,6,9,7,1,9,6,3,5,6,9,7,10,7,6,3,11,8,6,11,9,11,6,7,11,4,11~10,3,9,8,10,5,9,6,11,1,4,10,9,10,8,4,5,3,7,5,7,8,9~11,10,8,4,1,4,7,4,10,11,6,11,7,11,4,10,7,3,11,8,10,9,11,4,9,4,3,9,4,8,10,5,4,10,1,10,5,4,5,11,8,10,4,11,10,7,10,4,11,4~7,10,7,10,11,6,4,1,4,11,10,6,9,4,6,10,8,11,3,8,7,5,9,11,4,1,8,11,6,11,4,3,10,6,11,10,11,6,4,10,5,11,10,7~10,8,6,3,6,7,9,6,1,9,6,7,9,11,8,4,9,10,9,6,5,9,5,3,9,8,11,9,4,6,7&reel_set5=11,8,11,3,11,8,6,4,6,10,11,9,8,9,5,7,5,6,3,8,6,3,9,6,9,6,4,6,9,8,11,5,9,7,8,9,11,4,1,8~8,9,10,9,4,10,8,5,9,1,8,3,9,8,10,7,10,11,9,5,10,9,7,9,3,5,8,10,8,6,9,3,8,10,4,6,4,8,9,10,4,10,8,5,10,5,10,9,8,3,9,7,8~5,7,10,8,11,7,10,11,4,11,1,4,10,4,11,4,11,10,5,6,10,7,4,6,4,9,4,10,11,3,10,11,5,1,4,7,3~11,6,10,6,10,6,1,9,7,6,1,8,6,4,11,4,11,4,6,10,5,10,4,7,11,10,3,4,10,5,3,10,11,7,4,11,6,11~7,8,6,8,6,8,9,6,9,7,8,7,6,10,4,6,11,9,5,3,6,8,3,9,4,6,9,8,1&reel_set8=9,6,7,11,7,6,8,6,7,11,4,9,5,10,6,8,7,4,11,9,10,7,3,8,11,1,9,7,9,5,11,5,8,6,8,6,8,7,3,8,6,11,3,9,4,7,9,10,7,11~8,3,9,8,5,6,5,7,9,1,9,5,6,9,4,11,10,8,7,9,7,4,7,11,7,3,10,5,10,11,6,3,4,5,7~11,3,4,8,4,7,5,9,4,11,8,11,10,4,10,9,7,5,8,9,11,4,11,5,1,10,7,6,8,4,10,11,4,3~6,9,6,10,4,3,11,5,8,7,1,10,4,10,5,9,1,10,4,8,7,9,11,3,4,7,6,9,8,11,8,4,11,9,3,8,11,8,11,5,7,6,9,6,11,10,11,9,11,8,6,11~7,6,11,8,5,9,6,9,4,3,4,1,6,11,8,9,8,3,7,10,9,5,1,6,7,9,10&reel_set7=7,3,7,3,9,11,9,8,10,6,7,11,4,11,8,4,7,5,6,8,10,9,11,3,6,9,6,7,8,11,8,11,6,11,8,7,8,7,4,8,1,10,11,8,6,7,6,7,5~11,1,4,6,4,10,11,10,3,4,7,10,8,9,7,5,7,9,10,6,5,10,9,10,11,3,5,8,5~4,1,10,5,7,8,10,9,11,10,11,4,11,6,8,11,4,7,10,8,5,4,10,11,3,4,9,10,4~8,6,4,8,4,10,11,10,11,4,11,6,9,7,8,10,11,7,1,11,6,10,9,6,11,8,10,7,5,9,11,9,3,8,10,11,9,11,4,5,3,10,8,7,8,6,3,1,7~8,11,7,9,3,10,7,4,10,11,8,9,8,5,6,8,6,7,6,4,6,1,6,4,6,7,8,6,8,6,3,9,6,1,10,9,7,8&reel_set9=7,10,8,9,4,6,8,11,3,7,3,10,7,6,10,4,9,10,6,4,8,7,5,9,5,6,8,10,9,10,6,8,3,11,7,10,7,1,9,7,9,8,9,5,11~7,6,5,3,7,3,1,6,4,5,9,11,9,5,8,4,10,7,9,8,5,8,11,5,6,7,5,3,7,11,5,7,9,6,9,7,9~7,4,5,1,10,6,10,8,10,4,11,4,7,4,8,9,10,3,8,9,11,4,11,10,8,5,4,10,4,10,9,5,7,10,3,10,9,4~5,10,8,4,7,3,8,6,9,10,9,8,11,8,10,11,9,3,9,6,1,7,11,10,6,7,10,4,8,6,4,11,9,4~9,6,7,10,7,8,5,6,10,6,9,11,10,9,6,9,6,3,8,9,11,8,11,4,1,6,9,7,4,11,9,1,9,7,9,7,4,8&total_bet_min=20.00";
            }
        }
        #endregion

        protected override int FreeSpinTypeCount
        {
            get { return 9; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204, 205, 206, 207, 208 };
        }
        public BookOfTutRespinGameLogic()
        {
            _gameID  = GAMEID.BookOfTutRespin;
            GameName = "BookOfTutRespin";
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BookOfTutRespinGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BookOfTutRespinGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }        
        protected override async Task<BasePPSlotSpinData> selectRangeSpinData(int websiteID, double minOdd, double maxOdd, BasePPSlotBetInfo betInfo)
        {
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequest(GameName, -1, minOdd, maxOdd, -1, true), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;

            return convertBsonToSpinData(spinDataDocument);
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
            dicParams["st"] = "rect";
            dicParams["sw"] = "5";
            dicParams["g"]  = "{bb:{reel_set:\"23\"}}";

        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int) message.Pop();
                int counter = (int) message.Pop();
                int ind     = (int) message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo       betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPSlotSpinResult    result  = _dicUserResultInfos[strGlobalUserID];
                    if ((result.NextAction != ActionTypes.DOBONUS) || (betInfo.SpinData == null) || !(betInfo.SpinData is BasePPSlotStartSpinData))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                        if (ind >= startSpinData.FreeSpins.Count)
                        {
                            responseMessage.Append("unlogged");
                        }
                        else
                        {
                            BasePPSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[ind]);
                            preprocessSelectedFreeSpin(freeSpinData, betInfo);

                            betInfo.SpinData                = freeSpinData;
                            List<string> freeSpinStrings    = new List<string>();
                            for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                                freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                            string strSpinResponse = freeSpinStrings[0];
                            if (freeSpinStrings.Count > 1)
                                betInfo.RemainReponses = buildResponseList(freeSpinStrings);

                            double selectedWin  = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                            double maxWin       = startSpinData.MaxOdd * betInfo.TotalBet;

                            //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                            sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                            
                            Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                            convertWinsByBet(dicParams, betInfo.TotalBet);
                            convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                            if (SupportMoreBet)
                            {
                                if (betInfo.MoreBet)
                                    dicParams["bl"] = "1";
                                else
                                    dicParams["bl"] = "0";
                            }
                            result.BonusResultString = convertKeyValuesToString(dicParams);
                            addDefaultParams(dicParams, userBalance, index, counter);
                            ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                            string strResponse = convertKeyValuesToString(dicParams);

                            responseMessage.Append(strResponse);

                            //히스토리보관 및 초기화
                            if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                                addDoBonusActionHistory(strGlobalUserID, ind, strResponse, index, counter);

                            result.NextAction = nextAction;
                        }
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        saveBetResultInfo(strGlobalUserID);
                    }
                }
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BookOfTutRespinGameLogic::onFSOption {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }

            if (!resultParams.ContainsKey("g") && spinParams.ContainsKey("g"))
                resultParams["g"] = spinParams["g"];
            return resultParams;
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if(dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["ep"] != null)
                {
                    int winLineID = 0;
                    do
                    {
                        string strKey = string.Format("l{0}", winLineID);
                        if (gParam["ep"][strKey] == null)
                            break;

                        string[] strParts = gParam["ep"][strKey].ToString().Split(new string[] { "~" }, StringSplitOptions.None);
                        if (strParts.Length >= 2)
                        {
                            strParts[1] = convertWinByBet(strParts[1], currentBet);
                            gParam["ep"][strKey] = string.Join("~", strParts);
                        }
                        winLineID++;
                    } while (true);
                    gParam["ep"] = serializeJsonSpecial(gParam);
                }
            }
            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);

        }
    }
}
