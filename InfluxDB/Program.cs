using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static InfluxDB.InfluxField;

namespace InfluxDB
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello InfluxDB Start!");
            //InfluxdbClient client = new InfluxdbClient("192.168.31.185", "8086");
            var influxDbClient = new InfluxDbClient("http://192.168.31.185:8086/", "root", "", InfluxDbVersion.v_1_3);

            #region 初始化数据库
            //client.Query("mydb", "drop measurement test");
            //Console.WriteLine("数据库初始化成功!");
            #endregion

            Console.WriteLine("\n插入数据：\n");

            #region 插入数据
            //for (int k = 1; k <= 100000; k *= 10)
            //{
            //    Stopwatch stopwatch = new Stopwatch();
            //    stopwatch.Restart();
            //    for (int i = 1; i <= k; i++)
            //    {
            //        // 插入数据
            //        client.Write("mydb", $"test,name={k} id={i}");
            //        //client.Query("mydb", $"insert test,name={k} id={i}");
            //    }
            //    stopwatch.Stop();
            //    Console.WriteLine($"插入{k}条数据耗时{stopwatch.ElapsedTicks} * 10^-4 ms");
            //    Console.WriteLine("--------------------------");
            //}
            #endregion

            #region 插入数据2
            for (int k = 1; k <= 100000; k *= 10)
            {

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();

                for (int i = 1; i <= k; ++i)
                {
                    var pointToWrite = new Point()
                    {
                        Name = "test",
                        Tags = new Dictionary<string, object>()
                        {
                            { "name", $"{k}" }
                        },
                        Fields = new Dictionary<string, object>()
                        {
                            { "id", $"{i}" }
                        },
                        Timestamp = DateTime.UtcNow
                    };
                    //var response = await influxDbClient.Client.WriteAsync(pointsToWrite, "mydb");
                    await influxDbClient.Client.WriteAsync(pointToWrite, "mydb");
                }

                stopwatch.Stop();
                Console.WriteLine($"插入{k}条数据耗时{stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"插入{k}条数据耗时{stopwatch.ElapsedTicks} * 10^-4 ms");
                Console.WriteLine("-------------------------------------------");
            }
            #endregion

            Console.WriteLine("\n***************************************\n\n查询数据：\n");

            #region 查询数据
            //for (int k = 1; k <= 100000; k *= 10)
            //{
            //    Stopwatch stopwatch = new Stopwatch();
            //    stopwatch.Restart();
            //    for (int i = 1; i <= k; i++)
            //    {
            //        // 查询数据
            //        //var result = client.Query("mydb", "select * from test");
            //        client.Query("mydb", $"select * from test limit k");
            //    }
            //    stopwatch.Stop();
            //    //Console.WriteLine(result);
            //    Console.WriteLine($"查询{k}条数据耗时{stopwatch.ElapsedTicks} * 10^-4 ms\n");
            //    Console.WriteLine("--------------------------");
            //}
            #endregion

            #region 查询数据2
            for (int k = 1; k <= 100000; k *= 10)
            {
                //// 提前建立连接
                //var queryTest = $"SELECT * FROM test limit 10";
                //influxDbClient.Client.QueryAsync(queryTest, "mydb");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                var query = $"SELECT * FROM test limit {k}";
                //var response = await influxDbClient.Client.QueryAsync(query, "mydb");
                await influxDbClient.Client.QueryAsync(query, "mydb");
                stopwatch.Stop();
                Console.WriteLine($"查询{k}条数据耗时{stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"查询{k}条数据耗时{stopwatch.ElapsedTicks} * 10^-3 ms");
                Console.WriteLine("------------------------------------------");
                //Console.WriteLine(response);
            }
            #endregion

            Console.WriteLine("Hello InfluxDB End!");
            Console.ReadLine();
            Console.ReadKey();

            //string measurement = "test";
            //// 首先定义tag、field、time
            //List<InfluxField> fields = new List<InfluxField>();
            //fields.Add(new InfluxField("name", "hyn", ValueTypeEnum.Field));
            //fields.Add(new InfluxField("id", "60", ValueTypeEnum.Tag));
            //string insertSql = InfluxdbClient.CombineInsertSql(measurement, fields);
            //client.Write("mydb", insertSql);
        }
    }

    class InfluxdbClient
    {
        private static string host_;
        private static string port_;
        private static string user_;
        private static string password_;
        private static bool https_;
        private static string InitUrl()
        {
            var http = "http://";
            if (https_)
            {
                http = "https://";
            }
            var url = http + host_ + ":" + port_;
            return url;
        }

        public InfluxdbClient(string host, string port = "8086", string user = null, string password = null, bool https = false)
        {
            host_ = host;
            port_ = port;
            user_ = user;
            password_ = password;
            https_ = https;
        }

        private string AddAuthToUrl(string url)
        {
            if (string.IsNullOrEmpty(user_))
            {
                return url;
            }
            url += $"&u={user_}&p={password_}";
            return url;
        }
        public string Query(string dbName, string sql)
        {

            var url = InitUrl() + $"/query?db={dbName}&q={sql}";
            url = AddAuthToUrl(url);
            string resultStr = HttpRequestUtil.Get(url);
            return resultStr;
        }

        public void CreateDB(string dbName)
        {
            string sql = $"create database {dbName}";
            var url = InitUrl() + $"/query?q={sql}";
            HttpRequestUtil.Get(url);
        }

        public void Write(string dbName, string sql)
        {
            var url = InitUrl() + $"/write?db={dbName}";
            url = AddAuthToUrl(url);
            HttpRequestUtil.Post(url, sql);
        }

        public static string CombineInsertSql(string measurement, List<InfluxField> fileds)
        {
            StringBuilder sqlBuilder = new StringBuilder(measurement);
            var timeObj = fileds.Find(x => x.Name == "time");
            var time = timeObj == null ? null : timeObj.ToString();
            var meases = fileds.Where(x => x.ValueType == ValueTypeEnum.Field);
            var tags = fileds.Where(x => x.ValueType == ValueTypeEnum.Tag);
            // 先拼接tag
            foreach (var tag in tags)
            {
                AddTagTo(sqlBuilder, tag);
            }
            sqlBuilder.Append(" ");
            foreach (var field in meases)
            {
                AddFieldTo(sqlBuilder, field);
            }
            // field肯定会有，这里要删除最后的一个逗号
            sqlBuilder = sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            if (!string.IsNullOrEmpty(time))
            {
                sqlBuilder.Append(" ");
                sqlBuilder.Append(time);
            }
            return sqlBuilder.ToString();
        }

        private static void AddFieldTo(StringBuilder sqlBuilder, InfluxField field)
        {
            if (field.Value != null)
            {
                string value = field.Value;
                // 所有的字段全部转换为字符串存入，field如果是字符串型，则需要加双引号，JsonConvert.SerializeObject会自动给字符串加上双引号
                value = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                sqlBuilder.Append($"{field.Name}=" + value);
                sqlBuilder.Append(",");
            }
        }

        private static void AddTagTo(StringBuilder sqlBuilder, InfluxField field)
        {
            if (field.Value != null)
            {
                sqlBuilder.Append(",");
                // tag始终作为字符串存入
                string value = field.Value.ToString();
                sqlBuilder.Append($"{field.Name}=" + value);
            }
        }
    }
    public class HttpRequestUtil
    {
        private static WebRequest GetRequest(string url, string method, int timeout = 10000, string contentType = "text/html;charset=UTF-8")
        {
            WebRequest request = WebRequest.Create(@url);
            request.Method = method;
            request.ContentType = contentType;
            request.Timeout = timeout;
            return request;
        }
        public static string Get(string url)
        {
            var request = GetRequest(url, "GET");
            Stream responseStream = request.GetResponse().GetResponseStream();
            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            return retString;
        }

        public static string Post(string url, string param)
        {
            var request = GetRequest(url, "POST", contentType: "application/json");
            if (!string.IsNullOrEmpty(param))
            {
                byte[] bs = Encoding.UTF8.GetBytes(param);
                request.ContentLength = bs.Length;
                Stream newStream = request.GetRequestStream();
                newStream.Write(bs, 0, bs.Length);
                newStream.Close();
            }
            Stream responseStream = request.GetResponse().GetResponseStream();
            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            responseStream.Close();
            //var respopse = request.GetResponse();
            //respopse.Close();

            return retString;
        }
    }
    public class InfluxField
    {
        public string Name { get; set; }
        /// <summary>
        /// Value始终为字符型，取用时根据需要解析
        /// </summary>
        public string Value { get; set; }

        public ValueTypeEnum ValueType { get; set; }
        public InfluxField(string name, string value, ValueTypeEnum type = ValueTypeEnum.Field)
        {
            Name = name;
            Value = value;
            ValueType = type;
        }

        public enum ValueTypeEnum
        {
            Field = 0,
            Tag = 1,
            Time = 2
        }

    }
}