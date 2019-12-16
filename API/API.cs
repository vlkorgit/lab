using System;
using System.Collections.Generic;
namespace API
{
    public class ServerInput
    {
        public string Filename { get; set; }
        public byte[] Bytes { get; set; }
    }
    public class ServerOutput
    {
        public string Filename { get; set; }
        public string Class { get; set; }
        public string Probability { get; set; }
    }
    public class ServerStat : List<KeyValuePair<string, int>> { }

}
