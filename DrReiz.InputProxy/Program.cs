using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace InputProxy
{
  class Program
  {
    static void Main(string[] args)
    {
      var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 7001));
      socket.Listen(10);
      for (; ; )
      {
        var client = socket.Accept();
        Console.WriteLine("connected..");
        var thread = new System.Threading.Thread(() =>
          {
            try
            {
              var clientReader = new System.IO.BinaryReader(new NetworkStream(client));
              for (; ; )
              {
                if (client.Poll(1, SelectMode.SelectRead) && client.Available == 0)
                {
                  Console.WriteLine("disconnected..");
                  break;
                }
                if (client.Available > 0)
                {
                  var msgSize = clientReader.ReadInt32();
                  var message = clientReader.ReadBytes(msgSize);
                  var messageReader = new System.IO.BinaryReader(new System.IO.MemoryStream(message));
                  var msgKind = messageReader.ReadInt32();
                  Console.WriteLine("message: kind:{0}, len:{1}", msgKind, message.Length);
                  switch (msgKind)
                  {
                    case 0:
                      {
                        var flags = messageReader.ReadUInt32();
                        var x = messageReader.ReadInt32();
                        var y = messageReader.ReadInt32();
                        var data = messageReader.ReadUInt32();
                        mouse_event(flags, x, y, data, UIntPtr.Zero);
                      }
                      break;
                  }
                }
                else
                  System.Threading.Thread.Sleep(10);
              }
            }
            catch (Exception exc)
            {
              Console.WriteLine(exc);
            }
          }) { IsBackground = true };
        thread.Start();
      }

    }
    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
  }


}
