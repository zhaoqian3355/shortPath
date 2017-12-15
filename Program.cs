using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;
using shortPath.ViewModels;
using System.Text;
using System.Diagnostics;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace shortPath
{
    class Program
    {
        public static PaperProjectContext dbContext = new PaperProjectContext();

        public static List<string> selectTrainCode = new List<string>();
        public static List<TrainStation> allStations = null;
        public static int changeTimes = 0;
        public static StringBuilder stringBuilder = new StringBuilder();

        static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<TrainStation, TrainStationView>());

            var  list=new List<SearchView>()
            {
                new SearchView(){start_station="大连",end_station="嘉兴"},
                new SearchView(){start_station="郑州",end_station="汕头"},
                new SearchView(){start_station="呼和浩特",end_station="厦门"},
                new SearchView(){start_station="白河",end_station="武汉"},
                 new SearchView(){start_station="贵阳",end_station="银川"},
                new SearchView(){start_station="赤峰",end_station="佳木斯"},
                new SearchView(){start_station="昭通",end_station="宁波"},
                new SearchView(){start_station="长春",end_station="西宁"},
                new SearchView(){start_station="佛山",end_station="兰州"},
                new SearchView(){start_station="天津",end_station="融水"},
                new SearchView(){start_station="南通",end_station="吉安"},
                new SearchView(){start_station="辽阳",end_station="合肥"},
                new SearchView(){start_station="苏州",end_station="白城"},
                new SearchView(){start_station="灵宝",end_station="宁波"},
                new SearchView(){start_station="包头",end_station="安庆"},
                new SearchView(){start_station="白狼",end_station="宝鸡"},
                new SearchView(){start_station="安顺",end_station="阿勒泰"}
            };

            list.ForEach(k=>
            {
                search(k);
                Console.WriteLine(k.start_station+","+k.end_station+","+k.direct_seconds+","+k.change_seconds);
            });


            Console.WriteLine("End......");
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
                var sameTrain = Mapper.Map<TrainStationView>(allStartStaions.Where(p => p.station_train_code == k.station_train_code && Convert.ToInt32(p.station_no) > Convert.ToInt32(k.station_no)).OrderBy(p => p.station_no).FirstOrDefault());
                if (sameTrain != null)
                {
                    nextStations.Add(sameTrain);
                    sameTrain.is_select = true;
                    k.TrainStationViews = Recursive(sameTrain.station_name, end);
                }
            }
            //startStations.

            return startStations;
        }

        public static void search(SearchView view)
        {
            allStations = dbContext.TrainStation.ToList();

            Stopwatch stopWatch = new Stopwatch();
            Stopwatch stopWatch1 = new Stopwatch();
            stopWatch.Start();
            stopWatch1.Start();
            var startStations = Mapper.Map<List<TrainStationView>>(allStations.Where(k => k.station_name.Contains(view.start_station)).ToList());
            // 获取经过起点的所有站点
            var startTrainCodes = startStations.Select(k => k.station_train_code).Distinct().ToList();
            var allStartStaions = allStations.Where(k => startTrainCodes.Contains(k.station_train_code)).ToList();

            var endStations = Mapper.Map<List<TrainStationView>>(allStations.Where(k => k.station_name.Contains(view.end_station)).ToList());
            // 获取经过终点的所有站点
            var endTrainCodes = endStations.Select(k => k.station_train_code).Distinct().ToList();
            var allEndStaions = allStations.Where(k => endTrainCodes.Contains(k.station_train_code)).ToList();

            var sameLines = startTrainCodes.Intersect(endTrainCodes).ToList();
            if (sameLines != null && sameLines.Count() > 0)
            {
                // sameLines.ForEach(k =>
                // {
                //     Console.WriteLine(k);
                // });

                TimeSpan ts = stopWatch.Elapsed;
                // Console.WriteLine("first RunTime " + Math.Round(ts.TotalSeconds, 3));
                view.direct_seconds=Math.Round(ts.TotalSeconds, 3);

            }

            stopWatch.Restart();
            
            var nextStations = new List<TrainStationView>();
            foreach (var k in startStations)
            {
                var sameTrains = Mapper.Map<List<TrainStationView>>(allStartStaions.Where(p => p.station_train_code == k.station_train_code && Convert.ToInt32(p.station_no) > Convert.ToInt32(k.station_no)).OrderBy(p => p.station_no).ToList());
                if (sameTrains != null && sameTrains.Count > 0)
                {
                    nextStations.AddRange(sameTrains);
                }
            }

            var sameLineViews = new List<TrainStationView>();
            nextStations.ForEach(k =>
            {
                var startStationsChange1 = Mapper.Map<List<TrainStationView>>(allStations.Where(p => p.station_name.Contains(k.station_name)).ToList());
                var startTrainCodesChange1 = startStationsChange1.Select(p => p.station_train_code).Distinct().ToList();
                var sameLinesChanges1 = startTrainCodesChange1.Intersect(endTrainCodes).ToList();
                if (sameLinesChanges1 != null && sameLinesChanges1.Count() > 0)
                {
                    k.TrainCodes = new List<string>();
                    k.TrainCodes.AddRange(sameLinesChanges1);
                    sameLineViews.Add(k);
                }

            });

            if (sameLineViews != null && sameLineViews.Count() > 0)
            {
                // sameLineViews.ForEach(k =>
                // {
                //     Console.Write(k.station_name + "_");
                //     k.TrainCodes.ForEach(p =>
                //     {
                //         Console.Write(p + "_");
                //     });
                //     Console.WriteLine();
                // });

                TimeSpan ts1 = stopWatch1.Elapsed;
                // Console.WriteLine("Second RunTime " + Math.Round(ts1.TotalSeconds, 3));
                view.change_seconds=Math.Round(ts1.TotalSeconds, 3);
            }
        }
    }
}