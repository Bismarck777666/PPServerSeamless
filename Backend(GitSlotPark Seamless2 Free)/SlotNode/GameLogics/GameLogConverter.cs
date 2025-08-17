using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlotGamesNode.GameLogics
{
    class GameLogConverter
    {
        public static string produceSlotGameDetailLog(int rows, int cols, int lineCount, int betLineCount, float betPerLine, int[][] reelStatus, float[] lineWins, int scatterNum, float scatterWin, float jackpotTotalWin, float fTotalBet, float fTotalWin)
        {

            List<int> reelArray = new List<int>();
            for(int i = 0; i < reelStatus.Length; i++)
            {
                for (int j = 0; j < reelStatus[i].Length; j++)
                    reelArray.Add(reelStatus[i][j]);
            }

            SlotGameLogDetail logDetail = new SlotGameLogDetail();
            logDetail.rows          = rows;
            logDetail.cols          = cols;
            logDetail.coin          = betPerLine;
            logDetail.linecount     = lineCount;
            logDetail.bet           = fTotalBet;
            logDetail.win           = fTotalWin;
            logDetail.reelarray     = reelArray.ToArray();
            logDetail.linewins      = lineWins;
            logDetail.scattercount  = scatterNum;
            logDetail.scatterwin    = scatterWin;
            logDetail.jackpotwin    = jackpotTotalWin;

            return JsonConvert.SerializeObject(logDetail);
        }
    }

    public class SlotGameLogDetail
    {
        public int      rows            { get; set; }
        public int      cols            { get; set; }
        public float    coin            { get; set; }
        public int      linecount       { get; set; }
        public float    bet             { get; set; }
        public float    win             { get; set; }
        public int[]    reelarray       { get; set; }
        public float[]  linewins        { get; set; }
        public int      scattercount    { get; set; }
        public float    scatterwin      { get; set; }
        public float    jackpotwin      { get; set; }
        public int      freespincount   { get; set; }

    }
}
