namespace UserNode
{
    public class PragmaticConfig
    {
        private static  PragmaticConfig _sInstance  = new PragmaticConfig();
        public static   PragmaticConfig Instance    => _sInstance;
        public string   ReplayURL { get; set; }
    }
}
