using Confluent.Kafka;
using NitroBolt.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrReiz.Gamer.DevConsole
{
    class Kafka
    {
        [CommandLine("--kafka-produce")]
        public static async Task Product()
        {
            var config = new ProducerConfig
            {
                //BootstrapServers = "ark-01.srvs.cloudkafka.com:9094,ark-02.srvs.cloudkafka.com:9094,ark-03.srvs.cloudkafka.com:9094",
                //SaslUsername = "micpzjb7",
                //SaslPassword = "BOeeyf5OBt9qCnWwoZQcQew_Ej8Yg6fd",
                BootstrapServers = "velomobile-01.srvs.cloudkafka.com:9094,velomobile-02.srvs.cloudkafka.com:9094,velomobile-03.srvs.cloudkafka.com:9094",
                SaslUsername = "fb69vbgp",
                SaslPassword = "h6GYYEvURKBsshRdGeVQXN7-1tUIu9rE",

                SaslMechanism = SaslMechanism.ScramSha256,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                
            };
            //var topicPrefix = "micpzjb7-";
            var topicPrefix = "fb69vbgp-";

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    //var dr = await p.ProduceAsync(new TopicPartition(topicPrefix + "x1111212xx", new Partition(1)), new Message<Null, string> { Value = "test" });
                    //var dr = await p.ProduceAsync(topicPrefix + "default", new Message<Null, string> { Value = "test" });
                    var dr = await p.ProduceAsync(topicPrefix + "default", new Message<Null, string> { Value = "test" });
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
        [CommandLine("--kafka-consume")]
        public static void Consume()
        {
            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = "velomobile-01.srvs.cloudkafka.com:9094,velomobile-02.srvs.cloudkafka.com:9094,velomobile-03.srvs.cloudkafka.com:9094",
                SaslUsername = "fb69vbgp",
                SaslPassword = "h6GYYEvURKBsshRdGeVQXN7-1tUIu9rE",
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                //AutoOffsetReset = AutoOffsetReset.Earliest,
                AutoOffsetReset = AutoOffsetReset.Latest,
            };

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                c.Subscribe("fb69vbgp-default");

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
        }
        [CommandLine("--kafka-produce-consume")]
        public static async Task ProduceAndConsume()
        {
            //var servers = "omnibus-01.srvs.cloudkafka.com:9094,omnibus-02.srvs.cloudkafka.com:9094,omnibus-03.srvs.cloudkafka.com:9094";
            //var user = "923hw6lb";
            //var password = "bc089-1Tfv0SR1ZoubIb4fKz5YCwvEfy";
            //var topicPrefix = $"{user}-";
            //var topic = topicPrefix + "default";
            var servers = "velomobile-01.srvs.cloudkafka.com:9094,velomobile-02.srvs.cloudkafka.com:9094,velomobile-03.srvs.cloudkafka.com:9094 ";
            var user = "fb69vbgp";
            var password = "h6GYYEvURKBsshRdGeVQXN7-1tUIu9rE";
            var topicPrefix = $"{user}-";
            var topic = topicPrefix + "default";


            var config = new ProducerConfig
            {
                BootstrapServers = servers,
                SaslUsername = user,
                SaslPassword = password,

                SaslMechanism = SaslMechanism.ScramSha256,
                SecurityProtocol = SecurityProtocol.SaslSsl,

            };

            var conf = new ConsumerConfig
            {
                //GroupId = $"{user}-consumer",
                GroupId = "cloudkarafka-example-3",
                BootstrapServers = servers,
                SaslUsername = user,
                SaslPassword = password,

                SaslMechanism = SaslMechanism.ScramSha256,
                SecurityProtocol = SecurityProtocol.SaslSsl,

                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest,
                //AutoOffsetReset = AutoOffsetReset.Latest,
                //Debug = "all",
                //ApiVersionRequest = false,
                SessionTimeoutMs = 6000,
            };
            //var topicPrefix = "micpzjb7-";

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                c.Subscribe(topic);
                //c.Assign(new TopicPartition("fb69vbgp-default", new Partition(1)));
                ////foreach (var part in c.Assignment)
                ////    Console.WriteLine(part.Partition.Value);
                //////c.Seek(new TopicPartitionOffset(new TopicPartition("fb69vbgp-default", new Partition(1)), Offset.Beginning));
                //var pos = c.Position(new TopicPartition(topic, new Partition(1)));
                //Console.WriteLine(pos);

                ////var offsets = c.GetWatermarkOffsets(new TopicPartition(topic, new Partition(1)));
                //var offsets = c.QueryWatermarkOffsets(new TopicPartition(topic, new Partition(1)), TimeSpan.FromSeconds(10));
                //Console.WriteLine($"{offsets.Low} {offsets.High}");




                //using (var p = new ProducerBuilder<Null, string>(config).Build())
                //{
                //    for (var i = 0; i < 10; ++i)
                //    {
                //        try
                //        {
                //            var dr = await p.ProduceAsync(topic, new Message<Null, string> { Value = "test" });
                //            Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                //        }
                //        catch (ProduceException<Null, string> e)
                //        {
                //            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                //        }
                //    }
                //}


                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }

        }
    }
}
