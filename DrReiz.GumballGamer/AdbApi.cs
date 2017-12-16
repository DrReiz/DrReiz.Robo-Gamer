using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.GumballGamer
{
    public static class AdbApi
    {
        public static byte[] ReplaceDAToA(byte[] bytes)
        {
            var result = new List<byte>();
            var isD = false;
            foreach (var b in bytes)
            {
                if (isD)
                {
                    isD = false;
                    if (b == 0x0A)
                    {
                        result.Add(b);
                        continue;
                    }
                    result.Add(0x0D);
                }

                if (b == 0x0D)
                    isD = true;
                else
                    result.Add(b);

            }
            if (isD)
                result.Add(0x0D);
            return result.ToArray();
        }
        public static byte[] ReadAll(System.IO.Stream stream, int? bufferLength = null)
        {
            var bytes = new List<byte>();
            for (; ; )
            {
                var buffer = new byte[bufferLength ?? 40960];
                var readLen = stream.Read(buffer, 0, buffer.Length);
                if (readLen == 0)
                    return bytes.ToArray();

                bytes.AddRange(buffer.Take(readLen));
            }
        }

        public static void SendCommandToAdb(System.IO.Stream stream, string command)
        {
            var fullCommand = $"{command.Length:x4}{command}";
            Console.WriteLine(fullCommand);
            var buf = Encoding.UTF8.GetBytes(fullCommand);
            stream.Write(buf, 0, buf.Length);
            var okay = ReceiveFromAdb(stream, length: 4);
            Console.WriteLine($">>{okay.Length} {okay}");
            if (okay.Substring(0, 4) != "OKAY")
                throw new Exception($"error adb receive: {okay}");
        }
        public static string RunCommand(System.IO.Stream stream, string command)
        {
            SendCommandToAdb(stream, command);
            //var result = ReadAllText(stream);
            //Console.WriteLine($">>{result}");
            //return result;
            return "";
        }
        public static string RunShellCommand(System.IO.Stream stream, string command)
        {
            SendCommandToAdb(stream, $"shell:{command}");
            var result = ReadAllText(stream);
            Console.WriteLine($">>{result}");
            return result;
        }

        static string ReceiveFromAdb(System.IO.Stream stream, int? length = null)
        {
            var answerBuf = new byte[length ?? 1000];
            int len = stream.Read(answerBuf, 0, answerBuf.Length);
            return Encoding.UTF8.GetString(answerBuf, 0, len);
        }
        public static string ReadAllText(System.IO.Stream stream)
        {
            return Encoding.UTF8.GetString(ReadAll(stream));
        }
        public static byte[] CaptureScreenshot(System.IO.Stream stream)
        {
            SendCommandToAdb(stream, "shell:screencap -p");
            return ReplaceDAToA(ReadAll(stream));
        }
    }
}
