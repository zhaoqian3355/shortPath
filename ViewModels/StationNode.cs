using System.Collections.Generic;

namespace shortPath.ViewModels
{
    public class StationNode
    {
        public string station_name { get; set; }
        public string train_code { get; set; }


        public List<StationNode> next_stations { get; set; }
    }
}