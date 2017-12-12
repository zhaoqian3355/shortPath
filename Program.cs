using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;
using shortPath.ViewModels;
using System.Text;
using System.Diagnostics;

namespace shortPath
{
    class Program
    {
        public static PaperProjectContext dbContext=new PaperProjectContext();

        public static List<string> selectTrainCode=new List<string>();
        public static List<TrainStation> allStations = null;
        public static int changeTimes = 0;
        public static StringBuilder stringBuilder = new StringBuilder();

        static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<TrainStation, TrainStationView>());
            var start = "大连";
            var end = "嘉兴";
            var startNode = new StationNode
            {
                station_name = start
            };

            var endNode = new StationNode
            {
                station_name = end
            };
            allStations = dbContext.TrainStation.ToList();

            var startStations = Mapper.Map<List<TrainStationView>>(allStations.Where(k => k.station_name == start).ToList());
            // 获取经过起点的所有站点
            var startTrainCodes = startStations.Select(k => k.station_train_code).Distinct().ToList();
            var allStartStaions = allStations.Where(k => startTrainCodes.Contains(k.station_train_code)).ToList();

            var endStations = Mapper.Map<List<TrainStationView>>(allStations.Where(k => k.station_name == end).ToList());
            // 获取经过终点的所有站点
            var endTrainCodes = endStations.Select(k => k.station_train_code).Distinct().ToList();
            var allEndStaions = allStations.Where(k => endTrainCodes.Contains(k.station_train_code)).ToList();

            var sameLines = startTrainCodes.Intersect(endTrainCodes).ToList();
            if (sameLines != null && sameLines.Count() > 0)
            {
                sameLines.ForEach(k =>
                {
                    Console.WriteLine(k);
                });

                Console.Read();

                return;
            }

            var nextStations = new List<TrainStationView>();
            foreach (var k in startStations)
            {
                var sameTrains = Mapper.Map<List<TrainStationView>>(allStartStaions.Where(p => p.station_train_code == k.station_train_code && Convert.ToInt32(p.station_no) > Convert.ToInt32(k.station_no)).OrderBy(p => p.station_no).ToList());
                if (sameTrains != null && sameTrains.Count > 0)
                {
                    nextStations.AddRange(sameTrains);
                }
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

           

            var sameLineViews = new List<TrainStationView>();
            nextStations.ForEach(k =>
            {
                var startStationsChange1 = Mapper.Map<List<TrainStationView>>(allStations.Where(p => p.station_name == k.station_name).ToList());
                var startTrainCodesChange1 = startStationsChange1.Select(p => p.station_train_code).Distinct().ToList();
                var sameLinesChanges1 = startTrainCodesChange1.Intersect(endTrainCodes).ToList();
                if (sameLinesChanges1 != null && sameLinesChanges1.Count() > 0)
                {
                    k.TrainCodes = new List<string>();
                    k.TrainCodes.AddRange(sameLinesChanges1);
                    sameLineViews.Add(k);
                }

            });

            if (sameLineViews != null && sameLineViews.Count()>0)
            {
                sameLineViews.ForEach(k =>
                {
                    Console.Write(k.station_name+"_");
                    k.TrainCodes.ForEach(p=>
                    {
                        Console.Write(p + "_");
                    });
                    Console.WriteLine();
                });
            }

            Console.WriteLine("RunTime " + ts.TotalSeconds);

            Console.Read();
        }

        public static List<TrainStationView> Recursive(string start, string end)
        {
            changeTimes++;
            var startStations = Mapper.Map<List<TrainStationView>>(allStations.Where(k => k.station_name == start).ToList());
            var startTrainCodes = startStations.Select(k => k.station_train_code).Distinct().ToList();
            var allStartStaions = allStations.Where(k => startTrainCodes.Contains(k.station_train_code) && k.is_select == false).ToList();
            //var allStartStaions = allStations.Where(k => startTrainCodes.Contains(k.station_train_code)).ToList();

            if (allStartStaions == null || allStartStaions.Count == 0)
            {
                //Console.WriteLine("|");
                return null;
            }
            
            // 设置已经选择过的点
            //allStartStaions.ForEach(k => { k.is_select = true; });

            var nextStations = new List<TrainStationView>();
            foreach (var k in startStations)
            {
                if (k.station_name == end)
                {
                    Console.WriteLine(k.station_train_code + "_" + k.station_name + "_" + k.station_no + "_");
                    continue;
                }
                else
                {
                    Console.WriteLine(k.station_train_code + "_" + k.station_name + "_" + k.station_no + "_");
                }
                var sameTrain =Mapper.Map<TrainStationView>(allStartStaions.Where(p => p.station_train_code == k.station_train_code && Convert.ToInt32(p.station_no) > Convert.ToInt32(k.station_no)).OrderBy(p => p.station_no).FirstOrDefault());
                if (sameTrain != null)
                {
                    nextStations.Add(sameTrain);
                    sameTrain.is_select = true;
                    k.TrainStationViews= Recursive(sameTrain.station_name, end);
                }
            }
            //startStations.

            return startStations;
        }

        //public static TrainStationView Recursive(string start, string end)
        //{
        //    changeTimes++;
        //    var startStation = Mapper.Map<TrainStationView>(allStations.FirstOrDefault(k => k.station_name == start && k.is_select == false));
        //    var nextStation = Mapper.Map<TrainStationView>(allStations.Where(k => startStation.station_train_code==k.station_train_code&&k.is_select == false && Convert.ToInt32(k.station_no) > Convert.ToInt32(startStation.station_no)).OrderBy(p => p.station_no).FirstOrDefault()) ;

        //    if (nextStation == null)
        //    {
        //        //Console.WriteLine("|");
        //        return null;
        //    }
        //    Console.WriteLine(startStation.station_train_code + "_" + startStation.station_name + "_" + startStation.station_no + "_");
        //    startStation.NextStation = nextStation;
        //    startStation.NextStation = Recursive(nextStation.station_name, end);
        //    if (startStation.station_name == end)
        //    {
        //        return null;
        //    }

        //    return startStation;
        //}

        public static void Recursive(StationNode start, StationNode end)
        {
            changeTimes++;
            var startStations = Mapper.Map<List<TrainStationView>>(allStations.Where(k => k.station_name == start.station_name).ToList());
            var startTrainCodes = startStations.Select(k => k.station_train_code).Distinct().ToList();
            var allStartStaions = allStations.Where(k => startTrainCodes.Contains(k.station_train_code) && k.is_select == false).ToList();

            stringBuilder.Append(start.station_name + "_" + start.train_code);
            //Console.WriteLine(start.station_name + "_" + start.train_code);

            if (allStartStaions == null || allStartStaions.Count == 0||start.station_name==end.station_name||changeTimes>5)
            {
                return;
            }

            var nextStations = new List<StationNode>();
            foreach (var k in startStations)
            {
                var sameTrain = Mapper.Map<TrainStationView>(allStartStaions.Where(p => p.station_train_code == k.station_train_code && Convert.ToInt32(p.station_no) > Convert.ToInt32(k.station_no)).OrderBy(p => p.station_no).FirstOrDefault());
                if (sameTrain != null)
                {
                    sameTrain.is_select = true;
                    var nextNode = new StationNode();
                    nextNode.station_name = sameTrain.station_name;
                    nextNode.train_code = sameTrain.station_train_code;
                    nextStations.Add(nextNode);
                }
            }
            start.next_stations = nextStations;
            start.next_stations.ForEach(k =>
            {
                Recursive(k, end);
            });
        }
    }
}