using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InfluxDB2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello InfluxDB Start!");

            InfluxDbHelper influxDbHelper = new InfluxDbHelper();

            Console.WriteLine("\n插入数据：\n");

            for (int k = 1; k <= 100000; k *= 10)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();

                influxDbHelper.WriteData(k);
                //influxDbHelper.WriteObjectData(k);

                stopwatch.Stop();
                Console.WriteLine($"插入{k}条数据耗时:{stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"插入{k}条数据耗时:{stopwatch.ElapsedTicks} * 10^-4 ms");
                //List<string> vs = new List<string>();
                //vs.Add("loop1");
                //influxDbHelper.QueryData(vs);
                Console.WriteLine("-----------------------------------");
            }

            Console.WriteLine("\n*********************************************");
            Console.WriteLine("\n查询数据：\n");

            List<string> loops = new List<string>();

            for (int k = 1; k <= 100000; k *= 10)
            {
                loops.Clear();
                for (int i = 1; i <= k; i++)
                {
                    loops.Add("loop" + i);
                }
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                influxDbHelper.QueryData(loops);
                stopwatch.Stop();
                Console.WriteLine($"查询{k}条数据耗时:{stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"查询{k}条数据耗时:{stopwatch.ElapsedTicks} * 10^-4 ms");
                Console.WriteLine("-----------------------------------");
            }

            Console.WriteLine("Hello InfluxDB End!");
            Console.ReadLine();
            Console.ReadKey();

        }
    }
}
