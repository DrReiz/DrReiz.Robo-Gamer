using System;
using System.Linq;
using System.Threading.Tasks;

namespace DrReiz.GumballGamer.Messages
{

    public class GameContext
    {
        public GameContext(string game, string storageDir)
        {
            this.Game = game;
            this.StorageDir = storageDir;
        }
        public readonly string Game;
        public readonly string StorageDir;

        public static readonly GameContext Gumball = new GameContext("gumball", @"t:\Data\Gumball\Screenshots");
        public static readonly GameContext Jewel = new GameContext("jewel", @"t:\Data\Jewel\Screenshots");
        public static readonly GameContext[] All = new[] { Gumball, Jewel };

        public static GameContext Get(string game)
            => All.First(context => context.Game == game);
    }

    public interface IGamer : Orleans.IGrainWithStringKey
    {
        Task<string> CaptureScreenshot(string game);
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
