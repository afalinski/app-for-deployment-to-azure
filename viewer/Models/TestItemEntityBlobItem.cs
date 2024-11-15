using Newtonsoft.Json;
using System;

namespace viewer.Models;

public class TestItemEntityBlobItem
{
    [JsonProperty("Message")]
    public string Message { get; set; }

    [JsonProperty("CallbackUri")]
    public string CallbackUri { get; set; }

    [JsonProperty("IsSynced")]
    public bool IsSynced { get; set; }

    [JsonProperty("PartitionKey")]
    public string PartitionKey { get; set; }

    [JsonProperty("RowKey")]
    public string RowKey { get; set; }

    [JsonProperty("Timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonProperty("ETag")]
    public string ETag { get; set; }
}
