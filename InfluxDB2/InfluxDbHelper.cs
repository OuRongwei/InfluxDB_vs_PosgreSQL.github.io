using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
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

        public async void WriteObjectData(int k, int j)
        {
            using (var writeApi = client.GetWriteApi())
            {
                for (var i = 1; i <= k; i++)
                {
                    var mem = new LoopStatusDetail
                    {
                        Id = j * k + i,
                        Name = j * k,
                        Time = DateTime.UtcNow
                    };
                    writeApi.WriteMeasurement(mem, WritePrecision.Ns, bucket, org);
                }
            }
        }

        public async void WriteData(int k, int j)
        {
            List<PointData> list = new List<PointData>();
            for (int i = 1; i <= k; i++)
            {
                var point = PointData.Measurement(LoopStatusDetail.Measurement)
                    .Tag("loopid", (j * k + i).ToString())
                    .Field("name", j * k)
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ms);

                list.Add(point);
            }

            using (var writeApi = client.GetWriteApi())
            {
                writeApi.WritePoints(list, bucket, org);
            }
        }

        public void QueryData()
        {
            var query = $"from(bucket: \"{ bucket}\") " +
                        $"|> range(start: -10s) ";
            //$"|> sort(columns:[\"loopid\"]) " +
            //$"|> limit(n:1) ";
            //$"|> yield()";

            var fluxTables = client.GetQueryApi().QueryAsync(query, org);
            var result = fluxTables.GetAwaiter().GetResult();
            foreach (FluxTable item in result)
            {
                item.Records.ForEach(item =>
                {
                    string ans = $"{item.GetTime()} : {item.GetValueByKey("loopid")} ------------- {item.GetValue()}";
                    //Console.WriteLine(ans);
                });
            }
            //Console.WriteLine(result.Count);
            result = null;
            return;
        }

    }
}
