namespace FrontNode.HTTPService.Models
{
    public class StatsResponse
    {
        public int      error       { get; set; }
        public string   description { get; set; }

        public StatsResponse(int errorCode, string strDescription)
        {
            error       = errorCode;
            description = strDescription;
        }
    }
}
