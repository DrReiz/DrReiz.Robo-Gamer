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
            this.Height = 1600;
            this.Width = 900;
        }
        public readonly string Game;
        public readonly string StorageDir;
        public readonly int Width;
        public readonly int Height;

        public static readonly GameContext Gumball = new GameContext("gumball", @"t:\Data\Gumball\Screenshots");
        public static readonly GameContext Jewel = new GameContext("jewel", @"t:\Data\Jewel\Screenshots");
        public static readonly GameContext Other = new GameContext("other-game", @"t:\Data\Other-Game\Screenshots");
        public static readonly GameContext[] All = new[] { Gumball, Jewel, Other };

        public static GameContext Get(string game)
            => All.First(context => context.Game == game);
    }

    public interface IGamer : Orleans.IGrainWithStringKey
    {
        Task<string> CaptureScreenshot(string game);
        Task Tap(string game, int x, int y);
    }
    public interface IAndroid : Orleans.IGrainWithStringKey
    {
        Task<byte[]> CaptureScreenshot();
    }

    public interface ISample: Orleans.IGrainWithStringKey
    {
        Task<string> Ping(string msg);
    }

    public static class GameContextHlp
    {
        public static string ImageFullPath(this GameContext context, string name) 
            => System.IO.Path.Combine(context.StorageDir, name + ".png");
    }
    //public class OrleansGrain : Orleans.Grain, IGumballPing
    //{
    //    public Task<string> Ping(string msg)
    //    {
    //        return Task.FromResult($"Gumball ping response: {DateTime.UtcNow.Ticks}");
    //    }
    //}
}
