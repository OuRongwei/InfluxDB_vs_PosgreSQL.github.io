using Npgsql;
using System;
using System.Data;
using System.Diagnostics;
using System.Threading;

namespace PostgreSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello PostgreSQL Start!");
            string connString = "Host=192.168.31.185;Port=5433;Username=postgres;Database=Test";
            var conn = new NpgsqlConnection(connString);
            conn.Open();
            Stopwatch stopwatch = new Stopwatch();

            #region 初始化数据库
            string initSQL = "delete from test";
            ExecNonQuery(initSQL, conn);
            Console.WriteLine("数据库初始化成功！");
            #endregion

            Console.WriteLine("\n插入数据：\n");

            for (int k = 1000; k <= 10000; k *= 10)
            {
                for (int j = 1; j <= 5; j++)
                {
                    stopwatch.Restart();

                    for (int i = 1; i <= k; i++)
                    {
                        // 插入数据
                        DateTime date = DateTime.UtcNow;
                        string inSQL = $"insert into test values('{date}',{j * k + i},{j * k})";
                        ExecNonQuery(inSQL, conn);
                    }
                    stopwatch.Stop();

                    Console.WriteLine(string.Format($"插入{k}条数据耗时：{stopwatch.ElapsedMilliseconds} ms"));
                    Console.WriteLine("-------------------");

                    //string SQL2 = $"select count(id) from test where insert_time > '{DateTime.UtcNow - (DateTime.UtcNow.AddSeconds(10) - DateTime.UtcNow)}'";
                    //var cnt = new NpgsqlCommand(SQL2, conn);
                    //var readcnt = cnt.ExecuteReader();
                    //if (readcnt.Read())
                    //{
                    //    Console.WriteLine($"数据库前十秒共有{readcnt.GetInt32(0)}条数据");
                    //    Console.WriteLine("-------------------------------------------");
                    //    readcnt.Close();
                    //}

                    stopwatch.Restart();

                    //Console.WriteLine(DateTime.UtcNow);
                    //Console.WriteLine(DateTime.UtcNow - (DateTime.UtcNow.AddSeconds(10) - DateTime.UtcNow));

                    string SQL = $"select * from test where insert_time > '{DateTime.UtcNow - (DateTime.UtcNow.AddSeconds(10) - DateTime.UtcNow)}'";
                    var seSQL = new NpgsqlCommand(SQL, conn);

                    var reader = seSQL.ExecuteReader();

                    while (reader.Read())
                        reader.GetValue(0);
                    reader.Close();

                    stopwatch.Stop();
                    Console.WriteLine(string.Format($"查询{k}条数据耗时：{stopwatch.ElapsedMilliseconds} ms"));
                    Console.WriteLine("\n***************************************\n");

                    Thread.Sleep(15000);
                }
                Console.WriteLine();
            }

            Console.WriteLine("Hello PostgreSQL End!");
            Console.ReadLine();
            Console.ReadKey();
            conn.Close();
        }
        static int ExecNonQuery(string _SQLCommand, NpgsqlConnection _conn)
        {
            int result = 0;
            NpgsqlCommand cmd = new NpgsqlCommand(_SQLCommand, _conn);
            cmd.CommandType = CommandType.Text;
            result = cmd.ExecuteNonQuery();  //执行SQL语句；Insert,Update,Delete方式都可以
            cmd.Dispose();  //释放资源
            return result;
        }
    }
}