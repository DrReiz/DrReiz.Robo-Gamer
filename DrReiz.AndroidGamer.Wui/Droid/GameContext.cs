using System;
using System.Linq;
using System.Threading.Tasks;

namespace DrReiz.AndroidGamer.Wui
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
        public static readonly GameContext Jewel = new GameContext("jewel", @"t:\Data\Jewel\Screenshots");//Jewel Legend
        public static readonly GameContext Empire = new GameContext("empire", @"t:\Data\Empire\Screenshots");//Empire & Puzzles
        public static readonly GameContext JCross = new GameContext("com.ucdevs.jcross", @"t:\Data\com.ucdevs.jcross\Screenshots");
        public static readonly GameContext Other = new GameContext("other-game", @"t:\Data\Other-Game\Screenshots");
        public static readonly GameContext[] All = new[] { Gumball, Jewel, Empire, JCross, Other };

        public static GameContext Get(string game)
            => All.First(context => context.Game == game);
    }

    public static class GameContextHlp
    {
        public static string ImageFullPath(this GameContext context, string name) 
            => System.IO.Path.Combine(context.StorageDir, name + ".png");
    }
}
