using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrReiz.AndroidGamer.Wui.Controllers
{
    public class DroidController : Controller
    {

        [HttpPost("api/droid/{game}/capture")]
        public async Task<object> Capture(string game)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "velomobile-01.srvs.cloudkafka.com:9094,velomobile-02.srvs.cloudkafka.com:9094,velomobile-03.srvs.cloudkafka.com:9094",
                SaslUsername = "fb69vbgp",
                SaslPassword = "h6GYYEvURKBsshRdGeVQXN7-1tUIu9rE",

                SaslMechanism = SaslMechanism.ScramSha256,
                SecurityProtocol = SecurityProtocol.SaslSsl,

            };
            var topicPrefix = "fb69vbgp-";

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var captureMessage = new AdbDroidMessage { Command = "capture", Game = game };

                    var dr = await p.ProduceAsync(topicPrefix + "adb-droid", new Message<Null, string> { Value = JsonConvert.SerializeObject(captureMessage) });
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                    return new { kafkaPartition = dr.TopicPartition, kafkaOffset = dr.TopicPartitionOffset };
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                    throw;
                }
            }
        }

    }

    public class AdbDroidMessage
    {
        public Guid Id;
        public string Command;
        public string Game;
        public int X;
        public int Y;
    }
}
