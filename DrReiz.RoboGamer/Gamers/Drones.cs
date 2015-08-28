using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gamering
{
  class Drones
  {
    public static void Execute()
    {
      var zones = new Point[3];
      var playerCount = 3;
      var droneCount = 4;
      var myId = 0;
      var drones = new Point[playerCount][];
      
    }

    static DronePosition Move(DronePosition pos, int turns = 1)
    {
      return new DronePosition(pos.Point, pos.Radius + turns * 100, zone:null);
    }
    static IEnumerable<DronePosition> VariateByZones(DronePosition pos, Point[] zones)
    {
      for(var i = 0; i < zones.Length; ++i)
      {
        var zone = zones[i];
        if (Dist(pos.Point, zone) <= pos.Radius + 100)
          yield return new DronePosition(zone, 100, zone: i);
      }
      yield return new DronePosition(pos.Point, pos.Radius, -1);
    }


    static double Dist2(Point p1, Point p2)
    {
      var dx = p2.X - p1.X;
      var dy = p2.Y - p1.Y;
      return dx * dx + dy * dy;
    }
    static double Dist(Point p1, Point p2)
    {
      var dx = p2.X - p1.X;
      var dy = p2.Y - p1.Y;
      return Math.Sqrt(dx * dx + dy * dy);
    }
  }
  class DronePosition
  {
    public DronePosition(Point point, int radius = 0, int? zone = null)
    {
      this.Point = point;
      this.Radius = radius;
      this.Zone = zone;
    }
    public Point Point;
    public int Radius;
    public int? Zone;
  }

  class Point
  {
    public Point(int x, int y)
    {
      this.X = x;
      this.Y = y;
    }
    public readonly int X;
    public readonly int Y;
  }
}
