using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlotGamesNode.GameLogics;

namespace SlotGamesNode
{
    public class CQ9Config
    {
        private static CQ9Config _sInstance = new CQ9Config();
        public static CQ9Config Instance
        {
            get
            {
                return _sInstance;
            }
        }
        public string                       HistoryURL          { get; set; }
        public CQ9PromotionItem[]           Promotion           { get; set; }
        public CQ9RecommendItem             RecommendList       { get; set; }
        public List<CQ9GameDBItem>          CQ9GameList         { get; set; }
    }
}
