using System;
using System.Threading.Tasks;

namespace DrReiz.GumballGamer.Messages
{

    public interface IGumball : Orleans.IGrainWithStringKey
    {
        Task<string> CaptureScreenshot();
    }
    public interface IAndroid : Orleans.IGrainWithStringKey
    {
        Task<byte[]> CaptureScreenshot();
    }

    public interface ISample: Orleans.IGrainWithStringKey
    {
        Task<string> Ping(string msg);
    }

    //public class OrleansGrain : Orleans.Grain, IGumballPing
    //{
    //    public Task<string> Ping(string msg)
    //    {
    //        return Task.FromResult($"Gumball ping response: {DateTime.UtcNow.Ticks}");
    //    }
    //}
}
