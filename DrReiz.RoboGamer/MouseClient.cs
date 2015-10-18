using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.RoboGamer
{
    public class MouseClient:IDisposable
    {
        public MouseClient(string ip, int port = 7001)
        {
            this.Ip = ip;
            this.Port = port;

            var client = new System.Net.Sockets.TcpClient(ip, port);
            client.NoDelay = true;
            var clientStream = client.GetStream();
            var clientWriter = new System.IO.BinaryWriter(clientStream);

            this.TcpClient = client;
            this.ClientWriter = clientWriter;
        }
        public readonly string Ip;
        public readonly int Port;

        public readonly System.Net.Sockets.TcpClient TcpClient;
        public readonly System.IO.BinaryWriter ClientWriter;

        public void SendMessage(byte[] message)
        {
            ClientWriter.Write(message.Length);
            ClientWriter.Write(message);
            ClientWriter.Flush();
        }

        public void MouseEvent(MouseEventFlags flags, int x, int y)
        {
            var messageStream = new System.IO.MemoryStream();
            var messageWriter = new System.IO.BinaryWriter(messageStream);
            messageWriter.Write(0);
            messageWriter.Write((uint)flags);
            messageWriter.Write(x);
            messageWriter.Write(y);
            messageWriter.Write(0);

            SendMessage(messageStream.ToArray());
        }
        public void MoveTo(int x, int y, int width, int height)
        {
            MouseEvent(MouseEventFlags.MOVE | MouseEventFlags.ABSOLUTE, x * 65536 / width, y * 65536 / height);
        }
        public void Reset()
        {
            var messageStream = new System.IO.MemoryStream();
            var messageWriter = new System.IO.BinaryWriter(messageStream);
            messageWriter.Write(1);
            SendMessage(messageStream.ToArray());
        }
        public void Dispose()
        {
            this.ClientWriter.Dispose();
            this.TcpClient.Close();
        }
    }
}
