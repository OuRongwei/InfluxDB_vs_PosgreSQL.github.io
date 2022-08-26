using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfluxDB2
{
    public class InfluxDbHelper
    {
        private InfluxDBClient client { get; }
        private string token;
        private string bucket;
        private string org;

        public InfluxDbHelper()
        {
            token = "qij464DNJmQ-dgO_BfRytK66iYM9N2MZPd7RRlu8X1i7OKxrEq43bN5QQa_Zk9FEnFprGHir_RnnEc5TVADCYA==";
            bucket = "tronpi";
            org = "No";
            client = InfluxDBClientFactory.Create("http://192.168.31.185:8086", token.ToCharArray());
        }

        public async void WriteObjectData(int k)
        {
            List<LoopStatusDetail> list = new List<LoopStatusDetail>();
            var random = new Random();
            for (var i = 1; i <= k; i++)
            {
                var mem = new LoopStatusDetail
                {
                    LoopId = "loop" + i,
                    DeviceId = "10939",
                    TeamId = "-1",
                    LoopTypeId = "0486565c-e138-4dff-8b82-6c818331aa45",
                    Ignore = false,
                    Stable = random.NextDouble(),
                    Status = 1,
                    Time = DateTime.UtcNow
                };

                list.Add(mem);
            }

            using (var writeApi = client.GetWriteApi())
            {
                writeApi.WriteMeasurements(list, WritePrecision.Ns, bucket, org);
            }
        }

        public async void WriteData(int k)
        {
            List<PointData> list = new List<PointData>();
            for (var i = 1; i <= k; i++)
            {
                double value = new Random().Next(1, k);
                var point = PointData.Measurement(LoopStatusDetail.Name)
                .Tag("loopid", "loop" + i)
                .Field("deviceid", "10939")
                .Field("teamid", "-1")
                .Field("looptypeid", "0486565c-e138-4dff-8b82-6c818331aa45")
                .Field("ignore", false)
                .Field("status", 1)
                //.Field("stable", -99999.99999)
                .Field("stable", value)
                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                list.Add(point);
            }

            using (var writeApi = client.GetWriteApi())
            {
                writeApi.WritePoints(list, bucket, org);
            }
        }

        public async Task<List<LoopStatusDetail>> QueryData(List<string> loops)
        {
            List<LoopStatusDetail> list = new List<LoopStatusDetail>();

            int pageSize = 1;
            var pageCount = Math.Ceiling((double)loops.Count);

            for (var j = 1; j <= pageCount; j++)
            {
                var temp = loops.Skip((j - 1) * pageSize).Take(pageSize).ToList();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var loop in temp)
                {
                    stringBuilder.Append($" r[\"loopid\"] == \"{loop}\" or");
                }
                stringBuilder.Remove(stringBuilder.Length - 2, 2);

                var filter = stringBuilder.ToString();

                var query = $"from(bucket: \"{ bucket}\") |> range(start: {DateTime.UtcNow.AddMinutes(-30).ToString("yyyy-MM-ddTHH:mm:ssZ")},stop: {DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")}) |> filter(fn: (r) =>{filter} )";

                var fluxTables = await client.GetQueryApi().QueryAsync(query, org);
                fluxTables.ForEach(fluxTable =>
                {
                    var key = fluxTable.GetGroupKey();
                    var fluxRecords = fluxTable.Records;

                    fluxRecords.ForEach(fluxRecord =>
                    {
                        string loopStatusDetail = $"{fluxRecord.GetValueByKey("loopid")}/{fluxRecord.GetField()}====>{fluxRecord.GetTime()}: {fluxRecord.GetValue()}";
                        //Console.WriteLine(loopStatusDetail);
                    });
                });
            }
            return list;
        }

    }
}
