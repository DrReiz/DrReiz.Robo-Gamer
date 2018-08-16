using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using NitroBolt.Functional;
using NitroBolt.Immutable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DrReiz.GumballGamer
{
    partial class Room
    {
        public Room(string id, string name, string shortName, ImmutableArray<RoomRef>? references = null)
        {
            this.id = id;
            this.name = name;
            this.shortName = shortName;
            this.references = references.OrEmpty();
        }

        public Room With(string id = null, string name = null, string shortName = null, ImmutableArray<RoomRef>? references = null)
        {
            return new Room(id ?? this.id, name ?? this.name, shortName ?? this.shortName, references ?? this.references);
        }
    }

    public static partial class RoomHelper
    {
        public static Room By(this IEnumerable<Room> items, string id = null, string name = null, string shortName = null)
        {
            if (id != null)
                return items.FirstOrDefault(_item => _item.id == id);
            if (name != null)
                return items.FirstOrDefault(_item => _item.name == name);
            if (shortName != null)
                return items.FirstOrDefault(_item => _item.shortName == shortName);
            return null;
        }
    }

    partial class RoomRef
    {
        public RoomRef(string id, TargetRoom target)
        {
            this.id = id;
            this.target = target;
        }

        public RoomRef With(string id = null, TargetRoom target = null)
        {
            return new RoomRef(id ?? this.id, target ?? this.target);
        }
    }

    public static partial class RoomRefHelper
    {
        public static RoomRef By(this IEnumerable<RoomRef> items, string id = null, TargetRoom target = null)
        {
            if (id != null)
                return items.FirstOrDefault(_item => _item.id == id);
            if (target != null)
                return items.FirstOrDefault(_item => _item.target == target);
            return null;
        }
    }

    partial class TargetRoom
    {
        public TargetRoom(string id, string name, string shortName)
        {
            this.id = id;
            this.name = name;
            this.shortName = shortName;
        }

        public TargetRoom With(string id = null, string name = null, string shortName = null)
        {
            return new TargetRoom(id ?? this.id, name ?? this.name, shortName ?? this.shortName);
        }
    }

    public static partial class TargetRoomHelper
    {
        public static TargetRoom By(this IEnumerable<TargetRoom> items, string id = null, string name = null, string shortName = null)
        {
            if (id != null)
                return items.FirstOrDefault(_item => _item.id == id);
            if (name != null)
                return items.FirstOrDefault(_item => _item.name == name);
            if (shortName != null)
                return items.FirstOrDefault(_item => _item.shortName == shortName);
            return null;
        }
    }
}