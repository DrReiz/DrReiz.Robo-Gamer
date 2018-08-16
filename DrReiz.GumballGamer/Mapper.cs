using DrReiz.GumballGamer.Messages;
using Newtonsoft.Json.Linq;
using NitroBolt.CommandLine;
using NitroBolt.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.GumballGamer
{
    public class Mapper
    {
        [CommandLine("save-map")]
        public static void Execute()
        {
            var gameContext = GameContext.Jewel;
            using (var context = new GamerDataContext())
            {
                var shotHs = context.Hsvs.Select(hsv => new { hsv.Shot, hsv.H }).ToDictionary(hsv => hsv.Shot, hsv => hsv.H);

                var steps = context.Steps.Where(step => step.Game == gameContext.Game).Select(step => new { step.Source, step.Target, step.Action }).AsEnumerable();

                var hSteps = steps.Select(step => new { SourceH = shotHs.FindValue(step.Source), TargetH = shotHs.FindValue(step.Target), step.Action })
                    .Where(step => step.SourceH != null && step.TargetH != null)
                    .Select(step => new { TargetH = step.TargetH.Value, SourceH = step.SourceH.Value, step.Action });

                Func<int, string> toName = h => $"h-{h:D3}";

                var shots = hSteps.GroupBy(step => step.SourceH)
                    .Select(group => { var name = toName(group.Key);
                        return new { id = name, name = name, shortName = name,
                            references = group
                             .Where(step => step.SourceH != step.TargetH)
                             .GroupBy(step => new { step.Action, step.TargetH })
                             .Select(refGroup => new { id = $"{name}-{toName(refGroup.Key.TargetH)}", name = refGroup.Key.Action,  target = new { id = toName(refGroup.Key.TargetH), name = toName(refGroup.Key.TargetH), shotName = toName(refGroup.Key.TargetH),  data = new { count = refGroup.Count() } } }) };
                    });

                var jshots = JArray.FromObject(shots);
                System.IO.File.WriteAllText("rooms.json", jshots.ToString());

            }
        }
    }
}
