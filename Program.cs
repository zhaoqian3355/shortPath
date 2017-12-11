using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace shortPath
{
    class Program
    {
        public static PaperProjectContext dbContext=new PaperProjectContext();

        public static List<string> selectTrainCode=new List<string>();

        static void Main(string[] args)
        {
            var start="北京";
            var end="上海";
            var startStations=dbContext.TrainStation.Where(k=>k.station_name==start).ToList();
            
            var startTrainCodes=dbContext.TrainStation.Where(k=>k.station_name==).Select(k=>k.station_train_code).Distinct().ToList();
            var endPaths=dbContext.TrainStation.Where(k=>k.station_name==end).ToList();
            var endTrainCodes=endPaths.Select(k=>k.station_train_code).Distinct().ToList();

            selectTrainCode.AddRange(startTrainCodes);
            selectTrainCode=selectTrainCode.Distinct().ToList();

            // 找到直达
            var directPaths=startTrainCodes.Intersect(endTrainCodes);
            if(directPaths!=null&&directPaths.Count()>0)
            {
                directPaths.ToList().ForEach(k=>{
                    Console.WriteLine(k);
                });

                return;
            }

            startTrainCodes.ForEach(k=>{
                var stations=dbContext.TrainStation.Where(p=>p.station_train_code==k
                &&p.station_name!=start
                &&!selectTrainCode.Contains(k)).ToList();

            });
        }

        public void GetPath(string start, List<TrainStation> endPaths)
        {

        }
    }
}
