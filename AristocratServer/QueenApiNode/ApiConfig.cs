using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueenApiNode
{
    public class ApiConfig
    {
        public static IActorRef WorkActorGroup      { get; set; }
        public static string    Domain              { get; set; }
        public static string    SUbAdvent           { get; set; }
        public static string    SubClientB          { get; set; }
        public static string    SubGame             { get; set; }
        public static string Local                  { get; set; }
    }
}
