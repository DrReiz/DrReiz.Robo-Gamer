using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using NitroBolt.Functional;
using NitroBolt.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrReiz.GumballGamer.Messages;

namespace DrReiz.GumballGamer
{
    partial class Action
    {
        public Action(int x, int y, bool isBack)
        {
            X = x;
            Y = y;
            IsBack = isBack;
        }

        public Action With(int ? x = null, int ? y = null, bool ? isBack = null)
        {
            return new Action(x ?? X, y ?? Y, isBack ?? IsBack);
        }
    }

    public static partial class ActionHelper
    {
        public static Action By(this IEnumerable<Action> items, int ? x = null, int ? y = null, bool ? isBack = null)
        {
            if (x != null)
                return items.FirstOrDefault(_item => _item.X == x);
            if (y != null)
                return items.FirstOrDefault(_item => _item.Y == y);
            if (isBack != null)
                return items.FirstOrDefault(_item => _item.IsBack == isBack);
            return null;
        }
    }
}