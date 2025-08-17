﻿using Akka.Actor;
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
        public static string    GameFrontUrl        { get; set; }
        public static string    FrontTokenKey       { get; set; }
        public static string    GameFrontImageUrl   { get; set; }
    }
}
