using Npgsql;
using System;
using System.Data;
using System.Diagnostics;

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
            //string initSQL = "delete from t1";
            string initSQL = "delete from test";
            ExecNonQuery(initSQL, conn);
            Console.WriteLine("数据库初始化成功！");
            //Console.WriteLine();
            #endregion

            #region 查询数据量
            //// 查询数据量
            //var cnt = new NpgsqlCommand("select count(id) from t1", conn);
            //var readcnt = cnt.ExecuteReader();
            //if (readcnt.Read())
            //{
            //    Console.WriteLine($"数据库共有{readcnt.GetInt32(0)}条数据");
            //    Console.WriteLine("-------------------------------------------");
            //    readcnt.Close();
            //}
            #endregion

            Console.WriteLine("\n插入数据：\n");

            #region 插入数据
            for (int k = 1; k <= 100000; k *= 10)
            {
                //DateTime start = DateTime.Now;
                stopwatch.Restart();

                for (int i = 1; i <= k; i++)
                {
                    // 插入数据
                    //string inSQL = "insert into t1 values((random()*10000000)::integer)";
                    //string inSQL = $"insert into t1 values({i})";
                    string inSQL = $"insert into test values({k+i})";
                    ExecNonQuery(inSQL, conn);
                    //Console.WriteLine("变更行数:" + ExecNonQuery(SQL, conn));
                }
                //DateTime end = DateTime.Now;
                //TimeSpan abs = end - start;
                stopwatch.Stop();

                //Console.WriteLine(string.Format($"程序执行时间：{abs.TotalMilliseconds}ms"));
                Console.WriteLine(string.Format($"插入{k}条数据耗时：{stopwatch.ElapsedMilliseconds} ms"));
                Console.WriteLine(string.Format($"插入{k}条数据耗时：{stopwatch.ElapsedTicks} * 10^-4 ms"));
                Console.WriteLine("-------------------");
            }

            //stopwatch.Restart();

            //for (int i = 1; i <= 1000; i++)
            //{
            //    // 插入数据
            //    string inSQL = $"insert into test values({i})";
            //    ExecNonQuery(inSQL, conn);
            //}
            //stopwatch.Stop();
            //Console.WriteLine(string.Format($"插入1000条数据耗时：{stopwatch.ElapsedMilliseconds} ms"));
            //Console.WriteLine(string.Format($"插入1000条数据耗时：{stopwatch.ElapsedTicks} * 10^-4 ms"));
            //Console.WriteLine("-------------------");
            #endregion

            Console.WriteLine("\n***************************************\n\n查询数据：\n");

            #region 查询数据

            for (int k = 1; k <= 10; ++k)
            {
                // 提前建立连接
                var tSQL = new NpgsqlCommand("select * from test where id = 1", conn);
                var tReader = tSQL.ExecuteReader();
                if (tReader.Read())
                    tReader.GetInt32(0);
                tReader.Close();

                //DateTime start = DateTime.Now;
                stopwatch.Restart();

                var seSQL = new NpgsqlCommand($"select * from t1 limit 1000", conn);
                var reader = seSQL.ExecuteReader();
                while (reader.Read())
                    reader.GetInt32(0);
                reader.Close();

                //for (int i = 1; i <= 1000; i++)
                //{
                //    var seSQL = new NpgsqlCommand($"select * from test where id = {i}", conn);
                //    var reader = seSQL.ExecuteReader();

                //    if (reader.Read())
                //        reader.GetInt32(0);
                //    reader.Close();
                //}

                //DateTime end = DateTime.Now;
                //TimeSpan abs = end - start;
                //Console.WriteLine(string.Format($"程序执行时间：{abs.TotalMilliseconds}ms"));

                stopwatch.Stop();
                Console.WriteLine(string.Format($"第{k}次查询1000条数据耗时：{stopwatch.ElapsedMilliseconds} ms"));
                Console.WriteLine(string.Format($"第{k}次查询1000条数据耗时：{stopwatch.ElapsedTicks} * 10^-4 ms"));
                Console.WriteLine("-------------------");
            }
            #endregion

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
