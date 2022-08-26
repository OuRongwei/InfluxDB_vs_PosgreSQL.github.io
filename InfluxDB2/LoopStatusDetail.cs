using InfluxDB.Client.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfluxDB2
{
    [Measurement("test")]
    public class LoopStatusDetail
    {
        public static string Measurement = "test";
        [Column("loopid", IsTag = true)] public int Id { get; set; }
        [Column("name")] public int Name { get; set; }
        [Column(IsTimestamp = true)] public DateTime Time { get; set; }

    }
}
