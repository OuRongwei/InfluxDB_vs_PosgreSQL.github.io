using InfluxDB.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace InfluxDB2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello InfluxDB Start!");

            InfluxDbHelper influxDbHelper = new InfluxDbHelper();

            Console.WriteLine("\n插入数据：\n");

            for (int k = 1000; k <= 10000; k *= 10)
            {
                for (int j = 1; j <= 5; j++)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Restart();

                    influxDbHelper.WriteObjectData(k, j);

                    stopwatch.Stop();
                    Console.WriteLine($"插入{k}条数据耗时:{stopwatch.ElapsedMilliseconds} ms");
                    Console.WriteLine("-----------------------------------");

                    stopwatch.Restart();
                    influxDbHelper.QueryData();
                    stopwatch.Stop();
                    Console.WriteLine($"查询{k}条数据耗时:{stopwatch.ElapsedMilliseconds} ms");
                    Console.WriteLine("\n*********************************************\n");

                    Thread.Sleep(15000);
                }
                Console.WriteLine();
            }


            Console.WriteLine("Hello InfluxDB End!");
            Console.ReadLine();
            Console.ReadKey();

        }
    }
}
