using System.Collections.Generic;

namespace shortPath.ViewModels
{
    public class SearchView
    {
        public int top{get;set;}
        public string start_station { get; set; }
        public string end_station { get; set; }

        public double direct_seconds{get;set;}

        public double change_seconds{get;set;}
    }
}