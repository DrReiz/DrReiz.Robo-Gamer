using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrReiz.GumballGamer.Messages;

namespace DrReiz.GumballGamer
{
    class Processor
    {
        public static void ToHsv()
        {
            using (var db = new GamerDataContext())
            {
                for (; ; )
                {
                    var steps = db.Steps.Where(step => !db.Hsvs.Select(hsv => hsv.Shot).Contains(step.Target)).OrderBy(step => step.Time).Take(10).ToArray();
                    if (!steps.Any())
                        break;
                    foreach (var step in steps)
                    {
                        Console.WriteLine($"{step.Source}");
                        var context = GameContext.Get(step.Game);
                        if (!db.Hsvs.Select(hsv => hsv.Shot).Contains(step.Source))
                        {
                            var (h, s, v) = Imager.ToHsv(context.ImageFullPath(step.Source));
                            db.Hsvs.InsertOnSubmit(new Hsv { Shot = step.Source, H = h, S = s, V = v });
                            db.SubmitChanges();
                        }
                        if (!db.Hsvs.Select(hsv => hsv.Shot).Contains(step.Target))
                        {
                            var (h, s, v) = Imager.ToHsv(context.ImageFullPath(step.Target));
                            db.Hsvs.InsertOnSubmit(new Hsv { Shot = step.Target, H = h, S = s, V = v });
                            db.SubmitChanges();
                        }
                    }
                }
            }
        }
    }
}
