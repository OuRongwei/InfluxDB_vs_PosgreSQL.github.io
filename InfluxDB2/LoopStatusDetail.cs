using InfluxDB.Client.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfluxDB2
{
    [Measurement("loop_status_detail")]
    public class LoopStatusDetail
    {
        public static string Name = "loop_status_detail";
        [Column("loopid", IsTag = true)] public string LoopId { get; set; }
        [Column("deviceid")] public string DeviceId { get; set; }
        [Column("teamid")] public string TeamId { get; set; }
        [Column("looptypeid")] public string LoopTypeId { get; set; }
        [Column("ignore")] public bool Ignore { get; set; }
        [Column("status")] public int Status { get; set; }
        [Column("stable")] public double Stable { get; set; }
        [Column(IsTimestamp = true)] public DateTime Time { get; set; }

    }
}
