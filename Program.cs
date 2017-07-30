using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReadingCDR
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory() + "\\data.csv";
System.Console.WriteLine(path);
            Console.WriteLine("Start executing in Parallel mode");
            Console.WriteLine("=================================");

            Stopwatch stopWatch2 = new Stopwatch();

             var data2 = ReadData.ReadFile(path);
             stopWatch2.Start();
             Task<string> task1 = Task.Factory.StartNew(() => ReadData.GetPeakMin(data2));
             Task<string> task2 = Task.Factory.StartNew(() => ReadData.GetPeakCallMin(data2));
             Task<string> task3 = Task.Factory.StartNew(() => ReadData.GetRelationsip(data2));
             Task<string> task4 = Task.Factory.StartNew(() => ReadData.MostProductiveEmployee(data2));
             Task<string> task5 = Task.Factory.StartNew(() => ReadData.LeastProductiveEmployee(data2));
             Task<string> task6 = Task.Factory.StartNew(() => ReadData.ClientWithLongestTalkTime(data2));
             Task<string> task7 = Task.Factory.StartNew(() => ReadData.ClientWithMostFrequentCalls(data2));

             Console.WriteLine(task1.Result);
             Console.WriteLine(task2.Result);
             Console.WriteLine(task3.Result);
             Console.WriteLine(task4.Result);
             Console.WriteLine(task5.Result);
             Console.WriteLine(task6.Result);
             Console.WriteLine(task7.Result);
             stopWatch2.Stop();
             TimeSpan ts2 = stopWatch2.Elapsed;

             string elapsedTime2 = $"{ts2.Hours:00}:{ts2.Minutes:00}:{ts2.Seconds:00}.{ts2.Milliseconds / 10:00}";
             Console.WriteLine("RunTime " + elapsedTime2);
            Console.ReadKey();
        }
    }

    public class CallData
    {
        public DateTime DataTime { get; set; }
        public string CallerId { get; set; }
        public string EmployeeId { get; set; }
        public int DurationOfCallSinceRinging { get; set; }
        public int TalkTime { get; set; }
        public CallStutus Status { get; set; }
    }

    public enum CallStutus
    {
        Answered,
        NoAnswer,
        Busy
    }
    public static class ReadData
    {
        public static List<CallData> ReadFile(string path)
        {
         FileStream fileStream = new FileStream(path, FileMode.Open);

            using (var reader = new StreamReader(fileStream))
            {
                var listA = new List<CallData>();
                while (!reader.EndOfStream)
                {
                    var redLine = reader.ReadLine();
                    var values = redLine.Split(',');
                    CallData line = new CallData
                    {
                        DataTime = Convert.ToDateTime(values[0]),
                        CallerId = values[1],
                        EmployeeId = values[2],
                        DurationOfCallSinceRinging = Convert.ToInt32(values[3]),
                        TalkTime = Convert.ToInt32(values[4])
                    };
                    switch (values[5])
                    {
                        case "ANSWERED":
                            line.Status = CallStutus.Answered;
                            break;
                        case "NO ANSWER":
                            line.Status = CallStutus.NoAnswer;
                            break;
                        default:
                            line.Status = CallStutus.Busy;
                            break;
                    }
                    listA.Add(line);

                }
                return listA;
            }
        }
        private static string ShowResults(Dictionary<string, int> peakMin, int mx)
        {
            foreach (var dict in peakMin)
            {
                if (dict.Value == mx)
                {
                    return dict.Key + " : " + mx;
                }
            }
            return "";
        }
        private static string ShowResults(ConcurrentDictionary<string, int> peakMin, int mx)
        {
            foreach (var dict in peakMin)
            {
                if (dict.Value == mx)
                    return dict.Key + " : " + mx;
            }
            return "";
        }
        private static void InsertKeyAndValue(Dictionary<string, int> peakMin, string dt, int count = 1)
        {
            if (peakMin.ContainsKey(dt))
            {
                peakMin[dt] += count;
            }
            else
            {
                peakMin.Add(dt, count);
            }
        }
        private static void InsertKeyAndValue(ConcurrentDictionary<string, int> peakMin, string dt, int count = 1)
        {
            peakMin.AddOrUpdate(dt, count, (key, oldValue) => oldValue + count);
        }

        public static string GetPeakMin(List<CallData> data)
        {
            ConcurrentDictionary<string, int> peakMin = new ConcurrentDictionary<string, int>();
            Parallel.ForEach(data, (da) =>
            {
                InsertKeyAndValue(peakMin, da.DataTime.ToString("g"));
            });
            return "1: Peak minute of incoming phonecalls:" + ShowResults(peakMin, peakMin.Values.Max());
        }
        public static string GetPeakCallMin(List<CallData> data)
        {
            ConcurrentDictionary<string, int> peakMin = new ConcurrentDictionary<string, int>();

            Parallel.ForEach(data, (da) =>
            {
                if (da.TalkTime > 0 && da.Status == CallStutus.Answered)
                {
                    Parallel.For(da.DurationOfCallSinceRinging - da.TalkTime, da.TalkTime + (da.DurationOfCallSinceRinging - da.TalkTime),(i) =>
                        //for (int i = da.DurationOfCallSinceRinging - da.TalkTime; i < da.TalkTime + (da.DurationOfCallSinceRinging - da.TalkTime); i++)
                    {
                        InsertKeyAndValue(peakMin, da.DataTime.AddSeconds(i).ToString("G"));
                    });
                }
            });

            return "2: Peak time(minute) of simultaneous phonecalls: " + ShowResults(peakMin, peakMin.Values.Max());
        }

        public static string GetRelationsip(List<CallData> data)
        {
            ConcurrentDictionary<string, int> peakMin = new ConcurrentDictionary<string, int>();

            Parallel.ForEach(data, (da) =>
            {
                InsertKeyAndValue(peakMin, da.CallerId + ";" + da.EmployeeId);
            });
            return "3: Relationship between a client and an employee:" + ShowResults(peakMin, peakMin.Values.Max());

        }

        public static string MostProductiveEmployee(List<CallData> data)
        {
            ConcurrentDictionary<string, int> peakMin = new ConcurrentDictionary<string, int>();

            Parallel.ForEach(data, (da) =>
            {
                if (da.Status != CallStutus.NoAnswer)
                    InsertKeyAndValue(peakMin, da.EmployeeId);
            });
            return "4: Most productive employee:" + ShowResults(peakMin, peakMin.Values.Max());
        }
        public static string LeastProductiveEmployee(List<CallData> data)
        {
            ConcurrentDictionary<string, int> peakMin = new ConcurrentDictionary<string, int>();
            Parallel.ForEach(data, (da) =>
            {

                if (da.Status != CallStutus.NoAnswer && da.Status != CallStutus.Busy)
                    InsertKeyAndValue(peakMin, da.EmployeeId);
            });
            return "5: Least productive employee:" + ShowResults(peakMin, peakMin.Values.Min());
        }

        public static string ClientWithLongestTalkTime(List<CallData> data)
        {
            ConcurrentDictionary<string, int> peakMin = new ConcurrentDictionary<string, int>();
            Parallel.ForEach(data, (da) =>
            {
                if (da.Status != CallStutus.NoAnswer)
                    InsertKeyAndValue(peakMin, da.CallerId, da.TalkTime);
            });
            return "6: Client with longest talk time:" + ShowResults(peakMin, peakMin.Values.Max());

        }
        public static string ClientWithMostFrequentCalls(List<CallData> data)
        {
            ConcurrentDictionary<string, int> peakMin = new ConcurrentDictionary<string, int>();
            Parallel.ForEach(data, (da) =>
            {
                InsertKeyAndValue(peakMin, da.CallerId);
            });
            return "7: Client with most frequent calls:" + ShowResults(peakMin, peakMin.Values.Max());
        }
    }
}
