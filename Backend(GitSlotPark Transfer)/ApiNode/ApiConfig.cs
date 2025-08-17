using Akka.Actor;

namespace QueenApiNode
{
    public class ApiConfig
    {
        public static IActorRef     WorkActorGroup  { get; set; }
        public static string        GameFrontUrl    { get; set; }
        public static string        FrontTokenKey   { get; set; }
        public static string        AdminTokenKey   { get; set; }
    }
}
