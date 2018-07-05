using System;
using System.Threading.Tasks;

namespace DrReiz.GumballGamer.Messages
{

    public interface IGumballPing : Orleans.IGrainWithStringKey
    {
        Task<string> Ping(string msg);
        Task<byte[]> CaptureScreenshot();
    }

    //public class OrleansGrain : Orleans.Grain, IGumballPing
    //{
    //    public Task<string> Ping(string msg)
    //    {
    //        return Task.FromResult($"Gumball ping response: {DateTime.UtcNow.Ticks}");
    //    }
    //}
}
