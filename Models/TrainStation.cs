using System.ComponentModel.DataAnnotations.Schema;

namespace shortPath
{
    public class TrainStation
    {
        public string arrive_time { get; set; }
        public string end_station_name { get; set; }
        public bool isEnabled { get; set; }
        public string service_type { get; set; }
        public string start_station_name { get; set; }
        public string start_time { get; set; }
        public string station_name { get; set; }
        public string station_no { get; set; }
        public string station_train_code { get; set; }
        public string stopover_time { get; set; }
        public string train_class_name { get; set; }
        public int Id { get; set; }
        public bool is_select{get;set;}
    }
}