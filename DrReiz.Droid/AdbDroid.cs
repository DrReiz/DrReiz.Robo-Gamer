using Confluent.Kafka;
using Newtonsoft.Json;
using NitroBolt.CommandLine;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace DrReiz.Droid
{
    public class AdbDroid
    {
        public static string CaptureScreenshot(string game)
        {
            var context = GameContext.Get(game);

            using (var client = new AdbClient("emulator-5554"))
            {
                var bitmap = client.CaptureScreenshot();
                return ImageStorage.Save(context, bitmap);
            }
        }
        public static void Tap(string game, int x, int y)
        {
            using (var client = new AdbClient("emulator-5554"))
            {
                client.Tap(x, y);
            }
        }

        [CommandLine("--adb-droid")]
        public static void Execute()
        {
            var config = new ConsumerConfig
            {
                GroupId = "adb-droid",
                BootstrapServers = "velomobile-01.srvs.cloudkafka.com:9094,velomobile-02.srvs.cloudkafka.com:9094,velomobile-03.srvs.cloudkafka.com:9094",
                SaslUsername = "fb69vbgp",
                SaslPassword = "h6GYYEvURKBsshRdGeVQXN7-1tUIu9rE",

                SaslMechanism = SaslMechanism.ScramSha256,
                SecurityProtocol = SecurityProtocol.SaslSsl,

                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                //AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe($"{config.SaslUsername}-adb-droid");

                var cancel = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cancel.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var kafkaMessage = consumer.Consume(cancel.Token);
                            Console.WriteLine($"Consumed message '{kafkaMessage.Value}' at: '{kafkaMessage.TopicPartitionOffset}'.");

                            var message = JsonConvert.DeserializeObject<AdbDroidMessage>(kafkaMessage.Value);
                            switch (message.Command)
                            {
                                case "capture":
                                    CaptureScreenshot(message.Game);
                                    break;
                                case "tap":
                                    Tap(message.Game, message.X, message.Y);
                                    break;
                            }

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
                    consumer.Close();
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
