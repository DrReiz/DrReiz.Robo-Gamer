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
    public class MapController: ApiController
    {

        static object GetObject(string id)
        {
            return GetRooms().By(id:id);
        }

        [Route("gamer/search")]
        [HttpGet]
        public object Search(string q, bool isForce = false)
        {
            return GetRooms();
        }

        //Получение всех данных по объекту, которые есть в хранилище. Включая карточку, ссылки на другие объекты и возможные запросы.
        [Route("gamer/object/{objectId}")]
        [HttpGet]
        public object FullObject(string objectId)
        {
            return GetObject(objectId);
        }

        //Карточка объекта
        [Route("gamer/object/{objectId}/Card")]
        [HttpGet]
        public object Card(string objectId)
        {
            return GetObject(objectId);
        }

        //Запросить недостающие данные из внешнего источника
        //isForce=true - перезапросить имеющиеся данные
        [Route("gamer/object/{objectId}/expand")]
        [HttpPost]
        public object Expand(string objectId, bool isForce = false)
        {
            return GetObject(objectId);
        }

        [Route("ping")]
        [HttpGet]
        public string Ping()
        {
            return "gamer.web-service";
        }

        static Room[] GetRooms()
        {
            var jsonRooms = System.IO.File.ReadAllText(@"p:\Projects\DrReiz.Robo-Gamer\DrReiz.GumballGamer\bin\Debug\rooms.json");
            var rooms = JsonConvert.DeserializeObject<Room[]>(jsonRooms);
            return rooms;
        } 


        [NitroBolt.CommandLine.CommandLine("load-rooms")]
        public static void Execute()
        {
            var rooms = GetRooms();
            Console.WriteLine(JArray.FromObject(rooms).ToString());
        }
    }

    public partial class Room
    {
        public readonly string id;
        public readonly string name;
        public readonly string shortName;

        public readonly ImmutableArray<RoomRef> references;
    }
    public partial class RoomRef
    {
        public readonly string id;
        public readonly TargetRoom target;
    }
    public partial class TargetRoom
    {
        public readonly string id;
        public readonly string name;
        public readonly string shortName;
    }
}
