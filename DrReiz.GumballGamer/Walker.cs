using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrReiz.GumballGamer.Messages;

namespace DrReiz.GumballGamer
{
    class Walker
    {
        public static void Execute()
        {
            var context = GameContext.Get("jewel");

            using (var dataContext = new GamerDataContext())
            {
                var zeroImage = Adb(adb => adb.CaptureScreenshot());
                var stateName = ImageStorage.Save(context, zeroImage);

                var rnd = new Random();

                for (; ; )
                {
                    //var x = rnd.Next(context.Width);
                    //var y = rnd.Next(context.Height);
                    //Console.WriteLine($"{x}, {y}");
                    //Adb(adb => adb.Tap(x, y));
                    var action = GetAction(rnd, context);
                    Adb(adb => action.ToAdb(adb));

                    var image = Adb(adb => adb.CaptureScreenshot());
                    var newStateName = ImageStorage.Save(context, image);

                    dataContext.Steps.InsertOnSubmit(new Step() {Game = context.Game, Source = stateName, Target = newStateName, Action = action.Text() });
                    dataContext.SubmitChanges();

                    stateName = newStateName;
                }
            }
        }

        static Action GetAction(Random rnd, GameContext context) 
            => 
            rnd.Next(10) == 0 
            ? new Action(0, 0, true) 
            : new Action(rnd.Next(context.Width), rnd.Next(context.Height), false);
        

        static T Adb<T>(Func<AdbClient, T> f)
        {
            using (var adb = new AdbClient("emulator-5554"))
                return f(adb);
        }
        static void Adb(Action<AdbClient> f)
        {
            using (var adb = new AdbClient("emulator-5554"))
                f(adb);
        }
    }
    public partial class Action
    {
        public readonly int X;
        public readonly int Y;
        public readonly bool IsBack;

        public string Text() => IsBack ? "back" : $"{X};{Y}";

        public void ToAdb(AdbClient adb)
        {
            if (IsBack)
                adb.Back();
            else
                adb.Tap(X, Y);
        }
    }
}
