using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using NitroBolt.Functional;
using NitroBolt.Immutable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.GumballGamer.Messages
{
    partial class Shot
    {
        public Shot(string shotName, Area[] areas = null)
        {
            this.shotName = shotName;
            this.areas = areas ?? this.areas;
        }

        public Shot With(string shotName = null, Area[] areas = null)
        {
            return new Shot(shotName ?? this.shotName, areas ?? this.areas);
        }

        public Shot With_areas(params Area[] areas)
        {
            return With(areas: areas);
        }
    }

    public static partial class ShotHelper
    {
        public static Shot By(this IEnumerable<Shot> items, string shotName = null)
        {
            if (shotName != null)
                return items.FirstOrDefault(_item => _item.shotName == shotName);
            return null;
        }
    }

    partial class Area
    {
        public Area(int x, int y, int width, int height, string name, string value)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.name = name;
            this.value = value;
        }

        public Area With(int ? x = null, int ? y = null, int ? width = null, int ? height = null, string name = null, string value = null)
        {
            return new Area(x ?? this.x, y ?? this.y, width ?? this.width, height ?? this.height, name ?? this.name, value ?? this.value);
        }
    }

    public static partial class AreaHelper
    {
        public static Area By(this IEnumerable<Area> items, int ? x = null, int ? y = null, int ? width = null, int ? height = null, string name = null, string value = null)
        {
            if (x != null)
                return items.FirstOrDefault(_item => _item.x == x);
            if (y != null)
                return items.FirstOrDefault(_item => _item.y == y);
            if (width != null)
                return items.FirstOrDefault(_item => _item.width == width);
            if (height != null)
                return items.FirstOrDefault(_item => _item.height == height);
            if (name != null)
                return items.FirstOrDefault(_item => _item.name == name);
            if (value != null)
                return items.FirstOrDefault(_item => _item.value == value);
            return null;
        }
    }
}