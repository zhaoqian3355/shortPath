using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace shortPath
{
    public class Train
    {
        public int Id{get;set;}
        public string station_train_code{get;set;}
        public string train_no{get;set;}
        public string train_type{get;set;}
        public string train_code{get;set;}
        public string from_station{get;set;}
        public string from_station_name{get;set;}
        public string to_station{get;set;}
        public string to_station_name{get;set;}
        public string from_station_telecode{get;set;}
        public string to_station_telecode{get;set;}
        public string IsSelect{get;set;}
    }
}